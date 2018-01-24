using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BandMate.Models;
using Microsoft.AspNet.Identity;

namespace BandMate.Controllers
{
    [Authorize]
    public class BandController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Band
        public ActionResult Index(int? bandId)
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Subscription)
                .Include(u => u.Subscription.SubscriptionType)
                .Include(u => u.Bands)
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            if (user.Subscription == null)
            {
                return RedirectToAction("Create", "Subscription");
            }

            if( !(user.Bands.Count > 0) )
            {
                return RedirectToAction("Create");
            }
            var bands = user.Bands.ToList();
            Band currentBand = bands[0];
            if (bandId == null)
            {
                return RedirectToAction("Index", "Band", new { bandId = currentBand.BandId});
            }
            else
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            if (currentBand == null)
            {
                return HttpNotFound();
            }
            var viewModel = new BandIndexViewModel();

            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;

            if (bands.Count > 0) //This works because a default band is set up after registration
            {
                if (TempData["infoMessage"] != null)
                {
                    ViewBag.infoMessage = TempData["infoMessage"].ToString();
                }

                if (TempData["dangerMessage"] != null)
                {
                    ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
                }

                return View(viewModel);
            }

            //we only reach this if there was an error creating the band on registration
            return RedirectToAction("Create", "Band");
        }

        // GET: Band/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Band band = db.Bands.Find(id);
            if (band == null)
            {
                return HttpNotFound();
            }
            return View(band);
        }

        // GET: Band/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Band/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Name)
        {
            Store store = new Store();
            db.Stores.Add(store);
            db.SaveChanges();

            Band band = new Band();
            band.Name = Name;
            band.Store = store;
            db.Bands.Add(band);
            db.SaveChanges();

            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Bands)
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            user.Bands.Add(band);
            db.SaveChanges();

            TempData["infoMessage"] = "Success! " + band.Name + " created.";

            return RedirectToAction("Index", new { bandId = band.BandId });
        }

        // GET: Band/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Band band = db.Bands.Find(id);
            if (band == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreId = new SelectList(db.Stores, "StoreId", "PlaylistId", band.StoreId);
            return View(band);
        }

        // POST: Band/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BandId,Name,StoreId")] Band band)
        {
            if (ModelState.IsValid)
            {
                db.Entry(band).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StoreId = new SelectList(db.Stores, "StoreId", "PlaylistId", band.StoreId);
            return View(band);
        }

        // GET: Band/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Band band = db.Bands.Find(id);
            if (band == null)
            {
                return HttpNotFound();
            }
            return View(band);
        }

        // POST: Band/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Band band = db.Bands.Find(id);
            db.Bands.Remove(band);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST: Band/Rename/5
        [HttpPost]
        public ActionResult Rename(int bandId, string bandName)
        {
            if ( bandName.Length > 0)
            {
                Band band = db.Bands.Find(bandId);
                band.Name = bandName;
                db.SaveChanges();
                TempData["infoMessage"] = "Success! Band name changed to " + band.Name + ".";
            }
            return RedirectToAction("Index", new { bandId = bandId });
        }


    }
}
