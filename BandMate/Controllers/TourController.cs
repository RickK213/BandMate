using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;
using System.Collections.Generic;


namespace BandMate.Controllers
{
    [Authorize]
    public class TourController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Create(int bandId, string tourName)
        {
            Tour tour = new Tour();
            tour.Name = tourName;
            tour.BandId = bandId;
            db.Tours.Add(tour);
            db.SaveChanges();

            var band = db.Bands
                .Include(b => b.Tours)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            band.Tours.Add(tour);
            db.SaveChanges();

            return RedirectToAction("Tours", "Band", new { bandId = bandId });
        }

        public ActionResult Delete(int tourId, int bandId)
        {
            var tour = db.Tours
                .Include(t => t.TourDates)
                .Where(t => t.TourId == tourId)
                .FirstOrDefault();
            List<TourDate> tourDatesToDelete = tour.TourDates.ToList();
            for (int i= tourDatesToDelete.Count-1; i>=0; i--)
            {
                db.TourDates.Remove(tourDatesToDelete[i]);
            }
            db.Tours.Remove(tour);
            db.SaveChanges();
            TempData["infoMessage"] = "You have removed the tour: " + tour.Name;
            return RedirectToAction("Tours", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult Edit(int tourId)
        {
            var tour = db.Tours.Find(tourId);
            return View(tour);
        }

        [HttpPost]
        public ActionResult Edit(int tourId, string tourName)
        {
            var tour = db.Tours.Find(tourId);
            tour.Name = tourName;
            db.SaveChanges();
            return RedirectToAction("Tours", "Band", new { bandId = tour.BandId });
        }

        public ActionResult Details(int tourId)
        {
        var tour = db.Tours
                .Include(t => t.TourDates)
                .Include("TourDates.SoldProducts")
                .Include("TourDates.SetList")
                .Include("TourDates.Venue")
                .Include("TourDates.Venue.Address")
                .Include("TourDates.Venue.Address.City")
                .Include("TourDates.Venue.Address.State")
                .Include("TourDates.Venue.Address.ZipCode")
                .Where(t => t.TourId == tourId)
                .FirstOrDefault();
            tour.TourDates = (ICollection<TourDate>)tour.TourDates.OrderBy(t => t.EventDate).ToList();
            if (TempData["infoMessage"] != null)
            {
                ViewBag.infoMessage = TempData["infoMessage"].ToString();
            }
            if (TempData["dangerMessage"] != null)
            {
                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
            }
            return View(tour);
        }

    }
}