using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandMate.Controllers
{
    [Authorize]
    public class TourDateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Create(int tourId)
        {
            var tour = db.Tours.Find(tourId);
            var venues = db.Venues
                .Where(v => v.BandId == tour.BandId)
                .ToList();
            var setLists = db.SetLists
                .Where(s => s.BandId == tour.BandId)
                .ToList();
            ViewBag.VenueId = new SelectList(venues, "VenueId", "Name");
            ViewBag.SetListId = new SelectList(setLists, "SetListId", "Name");
            return View();
        }
    }
}