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
                .Include("Store.Products.ProductType")
                .Include("Store.Products.Sizes")
                .Where(b => b.Name == bandName)
                .FirstOrDefault();
            if (band.Store.PlaylistId != null)
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

        [HttpPost]
        public ActionResult CheckoutAjax(int bandId, int storeId, string cartProducts)
        {
            return Json(Url.Action("Checkout", "Store", new { bandId = bandId, storeId = storeId, cartProducts = cartProducts }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Checkout(int bandId, int storeId, string cartProducts)
        {
            StoreCheckoutViewModel viewModel = new StoreCheckoutViewModel();
            var band = db.Bands.Find(bandId);
            viewModel.Band = band;
            viewModel.StoreId = storeId;
            viewModel.CartProducts = cartProducts;
            ViewBag.StateId = new SelectList(db.States, "StateId", "Abbreviation");
            return View(viewModel);
        }

        public ActionResult ThankYou()
        {
            if (TempData["storeId"] != null)
            {
                ViewBag.storeId = TempData["storeId"].ToString();
            }
            if (TempData["bandName"] != null)
            {
                ViewBag.bandName = TempData["bandName"].ToString();
            }
            if (TempData["orderConfirmation"] != null)
            {
                ViewBag.orderConfirmation = TempData["orderConfirmation"].ToString();
            }
            return View();
        }

    }
}