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
                    if(songFound == existingSetListSong.Song)
                    {
                        duplicateSong = true;
                    }
                }
                if(!duplicateSong)
                {
                    setListSong.Song = songFound;
                    setList.SetListSongs.Add(setListSong);
                    db.SaveChanges();
                    string existingSongHtml = WebUtility.HtmlEncode(GetSongHtml(setListSong));
                    json = "{\"append\": true, \"newSong\": false, \"html\": \""+existingSongHtml+"\"}";
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