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

            return View(band);
        }
    }
}