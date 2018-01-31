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
            tourDate.ParentId = tourId;
            tourDate.EventDate = eventDate;
            tourDate.SetListId = SetListId;
            tourDate.VenueId = VenueId;

            Tour tour = db.Tours
                .Include(t => t.TourDates)
                .Where(t => t.TourId == tourId)
                .FirstOrDefault();
            tour.TourDates.Add(tourDate);

            db.TourDates.Add(tourDate);
            db.SaveChanges();
            TempData["infoMessage"] = "Tour date created!";
            return RedirectToAction("Tours", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult Edit(int tourDateId, int bandId)
        {
            var tourDate = db.TourDates.Find(tourDateId);
            var band = db.Bands
                .Include(b => b.Venues)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            var setLists = db.SetLists
                .Where(s => s.BandId == bandId)
                .ToList();
            List<Venue> venues = band.Venues.ToList();
            ViewBag.VenueId = new SelectList(venues, "VenueId", "Name");
            ViewBag.SetListId = new SelectList(setLists, "SetListId", "Name");
            return View(tourDate);
        }

        [HttpPost]
        public ActionResult Edit(int tourDateId, DateTime eventDate, double appearanceFee, int SetListId, int VenueId)
        {
            TourDate tourDate = db.TourDates.Find(tourDateId);

            tourDate.AppearanceFee = Convert.ToDouble(appearanceFee);
            tourDate.EventDate = eventDate;
            tourDate.SetListId = SetListId;
            tourDate.VenueId = VenueId;

            db.SaveChanges();
            TempData["infoMessage"] = "Tour date modified!";
            return RedirectToAction("Details", "Tour", new { tourId = tourDate.ParentId });
        }

        [HttpGet]
        public ActionResult Delete(int tourDateId)
        {
            TourDate tourDate = db.TourDates.Find(tourDateId);
            db.TourDates.Remove(tourDate);
            db.SaveChanges();
            TempData["infoMessage"] = "Tour date deleted!";
            return RedirectToAction("Details", "Tour", new { tourId = tourDate.ParentId });
        }

    }
}