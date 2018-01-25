using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;

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
            var tour = db.Tours.Find(tourId);
            var band = db.Bands
                .Include(b => b.Tours)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            db.Tours.Remove(tour);
            band.Tours.Remove(tour);
            db.SaveChanges();

            return RedirectToAction("Tours", "Band", new { bandId = tour.BandId });
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
                .Include("TourDates.ProductsSold")
                .Where(t => t.TourId == tourId)
                .FirstOrDefault();
        return View(tour);
        }

    }
}