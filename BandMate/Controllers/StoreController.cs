using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;


namespace BandMate.Controllers
{
    public class StoreController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string bandName)
        {
            var band = db.Bands
                .Include(b => b.Store)
                .Include("Store.Products")
                .Where(b => b.Name == bandName)
                .FirstOrDefault();
            if (band.Store.PlaylistId.Length > 0)
            {
                string spotifyEmbed = @"https://open.spotify.com/embed?uri=";
                spotifyEmbed += band.Store.PlaylistId;
                ViewBag.SpotifyUri = spotifyEmbed;
            }
            return View(band);
        }

        [Authorize]
        public ActionResult SavePlaylistId(int storeId, int bandId, string playListId)
        {
            var store = db.Stores.Find(storeId);
            store.PlaylistId = playListId;
            db.SaveChanges();
            TempData["infoMessage"] = "Spotify URI has been saved!";
            return RedirectToAction("Store", "Band", new { bandId = bandId });
        }

    }
}