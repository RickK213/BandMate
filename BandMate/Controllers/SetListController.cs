using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BandMate.Controllers
{
    public class SetListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SetList
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int bandId, string setListName)
        {
            SetList setList = new SetList();
            setList.BandId = bandId;
            setList.Name = setListName;
            db.SetLists.Add(setList);
            db.SaveChanges();
            return RedirectToAction("Edit", "SetList", new { setListId = setList.SetListId });
        }

        [HttpGet]
        public ActionResult Edit(int setListId)
        {
            SetList setList = db.SetLists
                .Include(s => s.SetListSongs)
                .Where(s => s.SetListId == setListId)
                .FirstOrDefault();
            var songs = db.Songs
                .Where(s => s.BandId == setList.BandId)
                .ToList();
            SetListEditViewModel viewModel = new SetListEditViewModel();
            viewModel.SetList = setList;
            viewModel.Songs = songs;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int setListId, string setListName, string[] songs )
        {
            return View();
        }

        [HttpGet]
        public ActionResult delete(int setListId, int bandId)
        {
            //check if it is used in any Tour Dates
            var tourDates = db.TourDates
                .Include(t => t.SetList)
                .OrderBy(t => t.EventDate)
                .ToList();
            List<TourDate> tourDatesUsingSetList = new List<TourDate>();
            foreach ( var tourDate in tourDates )
            {
                if ( tourDate.SetList.SetListId == setListId )
                {
                    tourDatesUsingSetList.Add(tourDate);
                }
            }
            if ( tourDatesUsingSetList.Count>0 )
            {
                StringBuilder tourDateList = new StringBuilder();
                int count = 0;
                foreach ( var tourDate in tourDatesUsingSetList )
                {
                    tourDateList.Append(String.Format("{0:MM/dd/yy}", tourDate.EventDate));
                    if ( count < tourDatesUsingSetList.Count-1)
                    {
                        tourDateList.Append(", ");
                    }
                    count++;
                }
                TempData["dangerMessage"] = "You cannot delete this set list because it is in use on tour dates on the following dates: " + tourDateList.ToString() + ". Please remove the set list from those tour dates first.";
                return RedirectToAction("SetLists", "Band", new { bandId = bandId });
            }

            var setList = db.SetLists
                .Include(s => s.SetListSongs)
                .Where(s => s.SetListId == setListId)
                .FirstOrDefault();
            List<SetListSong> setListSongsToDelete = setList.SetListSongs.ToList();
            for (int i = setListSongsToDelete.Count - 1; i >= 0; i--)
            {
                db.SetListSongs.Remove(setListSongsToDelete[i]);
            }
            db.SetLists.Remove(setList);
            db.SaveChanges();
            TempData["infoMessage"] = "You have removed the set list: " + setList.Name;
            return RedirectToAction("SetLists", "Band", new { bandId = bandId });
        }

        [HttpPost]
        public ActionResult SaveName(string setListName, int setListId)
        {
            SetList setList = db.SetLists.Find(setListId);
            setList.Name = setListName;
            db.SaveChanges();
            return Json(setList.Name, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Reorder(string jsonString, int setListId)
        {
            JArray songArray = JArray.Parse(jsonString);
            List<SetListSong> setListSongs = new List<SetListSong>();
            int count = 0;
            foreach (var item in songArray.Children())
            {
                var itemProperties = item.Children<JProperty>();
                var element = itemProperties.First();
                int songId = element.Value.ToObject<int>();
                Song song = db.Songs.Find(songId);
                SetListSong setListSong = new SetListSong();
                setListSong.SetListOrder = count;
                setListSong.Song = song;
                setListSongs.Add(setListSong);
                count++;
            }
            SetList setList = db.SetLists
                .Include(s => s.SetListSongs)
                .Where(s => s.SetListId == setListId)
                .FirstOrDefault();
            //Remove old set list songs from db
            List<SetListSong> setListSongsToDelete = setList.SetListSongs.ToList();
            for(int i= setListSongsToDelete.Count-1; i>=0; i--)
            {
                int setListSongId = setListSongsToDelete[i].SetListSongId;
                SetListSong setListSongToDelete = db.SetListSongs.Find(setListSongId);
                db.SetListSongs.Remove(setListSongToDelete);
            }
            db.SaveChanges();
            //Remove old set list songs from ICollection
            setList.SetListSongs.Clear();
            //Add new set list songs
            foreach (SetListSong newSetListSong in setListSongs)
            {
                setList.SetListSongs.Add(newSetListSong);
            }
            db.SaveChanges();
            string songsHtml = "";
            foreach ( SetListSong setListSong in setList.SetListSongs )
            {
                songsHtml += GetSongHtml(setListSong);
            }
            return Json(songsHtml, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveSong(int setListId, int setListSongId)
        {
            SetList setList = db.SetLists
                .Include(s => s.SetListSongs)
                .Where(s => s.SetListId == setListId)
                .FirstOrDefault();
            var setListSong = db.SetListSongs.Find(setListSongId);
            SetListSong setListSongInternal = setList.SetListSongs.Where(s => s.SetListSongId == setListSongId).FirstOrDefault();
            setList.SetListSongs.Remove(setListSongInternal);
            db.SetListSongs.Remove(setListSong);
            db.SaveChanges();
            return Json(setList.Name, JsonRequestBehavior.AllowGet);
        }

        private string GetSongHtml(SetListSong setListSong)
        {
            StringBuilder songHtml = new StringBuilder();
            songHtml.Append("<li class=\"list-group-item\" data-songId=\"" + setListSong.Song.SongId + "\">");
            songHtml.Append("<div class=\"row\">");
            songHtml.Append("<div class=\"col-md-1\">");
            songHtml.Append("<a class=\"btn btn-link btn-s\"><span class=\"glyphicon glyphicon-move\"></span></a>");
            songHtml.Append("</div>");
            songHtml.Append("<div class=\"col-md-10\">");
            songHtml.Append("<h4>" + setListSong.Song.Name + "</h4>");
            songHtml.Append("</div>");
            songHtml.Append("<div class=\"col-md-1\">");
            songHtml.Append("<a class=\"btn btn-danger btn-s removeSong\" data-setListSongId=\"" + setListSong.SetListSongId + "\"><span class=\"glyphicon glyphicon-trash\"></span></a>");
            songHtml.Append("</div>");
            songHtml.Append("</div>");
            songHtml.Append("</li>");

            return songHtml.ToString();
        }

        public ActionResult Details(int setListId)
        {
            SetList setList = db.SetLists
                .Include(s => s.SetListSongs)
                .Include("SetListSongs.Song")
                .Where(s =>s.SetListId == setListId)
                .FirstOrDefault();
            db.SaveChanges();
            return View(setList);
        }


    }
}