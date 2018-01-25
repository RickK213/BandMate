using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;

namespace BandMate.Controllers
{
    public class TourController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Create(int bandId, string tourName)
        {
            Tour tour = new Tour();
            tour.Name = tourName;
            db.Tours.Add(tour);
            db.SaveChanges();

            var band = db.Bands
                .Include(b => b.Tours)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            band.Tours.Add(tour);
            db.SaveChanges();

            return RedirectToAction("Tours", "Band");
        }
    }
}