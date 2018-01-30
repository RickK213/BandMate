using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;
using System.Text;
using System.Net;

namespace BandMate.Controllers
{
    public class SongController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Create(int bandId, int setListId, string songName)
        {
            string json;
            SetList setList = db.SetLists
                .Include(s => s.SetListSongs)
                .Where(s => s.SetListId == setListId)
                .FirstOrDefault();
            SetListSong setListSong = new SetListSong();
            setListSong.SetListOrder = setList.SetListSongs.Count;
            if (db.Songs.Any(s => s.BandId == bandId && s.Name == songName))
            {
                var songFound = db.Songs.First(s => s.BandId == bandId && s.Name == songName);
                bool duplicateSong = false;
                foreach (SetListSong existingSetListSong in setList.SetListSongs)
                {
                    if (songFound == existingSetListSong.Song)
                    {
                        duplicateSong = true;
                    }
                }
                if (!duplicateSong)
                {
                    setListSong.Song = songFound;
                    setList.SetListSongs.Add(setListSong);
                    db.SaveChanges();
                    string existingSongHtml = WebUtility.HtmlEncode(GetSongHtml(setListSong));
                    json = "{\"append\": true, \"newSong\": false, \"html\": \"" + existingSongHtml + "\"}";
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                json = "{\"append\": false, \"newSong\": false, \"html\": \"\"}";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            Song song = new Song();
            song.Name = songName;
            song.BandId = bandId;
            db.Songs.Add(song);
            db.SaveChanges();

            setListSong.Song = song;
            setList.SetListSongs.Add(setListSong);
            db.SaveChanges();
            string songHtml = WebUtility.HtmlEncode(GetSongHtml(setListSong));
            json = "{\"append\": true, \"newSong\": true, \"html\": \"" + songHtml + "\"}";
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateForBand(int bandId, string songName)
        {
            if (db.Songs.Any(s => s.BandId == bandId && s.Name == songName))
            {
                TempData["dangerMessage"] = songName + " already exists!";
                return RedirectToAction("Songs", "Band", new { bandId = bandId });
            }
            Song song = new Song();
            song.Name = songName;
            song.BandId = bandId;
            db.Songs.Add(song);
            db.SaveChanges();

            TempData["infoMessage"] = songName + " created!";
            return RedirectToAction("Songs", "Band", new { bandId = bandId });
        }

        public ActionResult Delete(int bandId, int songId)
        {
            var song = db.Songs.Find(songId);
            db.Songs.Remove(song);
            db.SaveChanges();
            TempData["infoMessage"] = song.Name + " deleted!";
            return RedirectToAction("Songs", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult Edit(int songId)
        {
            var song = db.Songs.Find(songId);
            return View(song);
        }

        [HttpPost]
        public ActionResult Edit(int songId, string songName)
        {
            var song = db.Songs.Find(songId);
            song.Name = songName;
            db.SaveChanges();
            TempData["infoMessage"] = "Song successfully modified!";
            return RedirectToAction("Songs", "Band", new { bandId = song.BandId });
        }

        private string GetSongHtml(SetListSong setListSong)
        {
            //< li class="list-group-item" data-songId="@setListSong.Song.SongId">
            //    <div class="row">
            //        <div class="col-md-1">
            //            <a class="btn btn-link btn-s"><span class="glyphicon glyphicon-move"></span></a>
            //        </div>
            //        <div class="col-md-10">
            //            <h4>@setListSong.Song.Name</h4>
            //        </div>
            //        <div class="col-md-1">
            //            <a class="btn btn-danger btn-s removeSong" data-setListSongId="@setListSong.SetListSongId"><span class="glyphicon glyphicon-trash"></span></a>
            //        </div>
            //    </div>
            //</li>

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

    }
}