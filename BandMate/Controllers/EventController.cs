using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandMate.Controllers
{
    public class EventController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Event
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int bandId, DateTime eventDate, string name, string description)
        {
            Event newEvent = new Event();
            newEvent.BandId = bandId;
            newEvent.EventDate = eventDate;
            newEvent.Name = name;
            newEvent.Description = description;
            db.Events.Add(newEvent);
            db.SaveChanges();
            TempData["infoMessage"] = name + " created!";
            return RedirectToAction("Events", "Band", new { bandId = bandId });            
        }
    }
}