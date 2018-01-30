using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;


namespace BandMate.Controllers
{
    [Authorize]
    public class TourDateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Create(int tourId, int bandId)
        {
            var tour = db.Tours.Find(tourId);
            var band = db.Bands
                .Include(b => b.Venues)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            var setLists = db.SetLists
                .Where(s => s.BandId == tour.BandId)
                .ToList();
            List<Venue> venues = band.Venues.ToList();
            ViewBag.VenueId = new SelectList(venues, "VenueId", "Name");
            ViewBag.SetListId = new SelectList(setLists, "SetListId", "Name");
            return View(tour);
        }

        [HttpPost]
        public ActionResult Create(int tourId, int bandId, DateTime eventDate, double appearanceFee, int SetListId, int VenueId)
        {
            TourDate tourDate = new TourDate();
            tourDate.AppearanceFee = Convert.ToDouble(appearanceFee);
            tourDate.BandId = bandId;
            tourDate.EventDate = eventDate;
            tourDate.SetListId = SetListId;
            tourDate.VenueId = VenueId;

            Tour tour = db.Tours
                .Include(t => t.TourDates)
                .Where(t =>t.TourId == tourId)
                .FirstOrDefault();
            tour.TourDates.Add(tourDate);

            db.TourDates.Add(tourDate);
            db.SaveChanges();
            TempData["infoMessage"] = "Tour date created!";
            return RedirectToAction("Tours", "Band", new { bandId = bandId });
        }

    }
}