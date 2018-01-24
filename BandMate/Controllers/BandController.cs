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
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace BandMate.Controllers
{
    [Authorize]
    public class BandController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Band
        public ActionResult Index(int? bandId)
        {
            CheckSubscription();
            var bands = GetUserBands();
            if (bands.Count <= 0)
            {
                return RedirectToAction("Create");
            }
            Band currentBand = bands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            var viewModel = new BandViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
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
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Band band = db.Bands.Find(id);
        //    if (band == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.StoreId = new SelectList(db.Stores, "StoreId", "PlaylistId", band.StoreId);
        //    return View(band);
        //}

        // POST: Band/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string bandName, int bandId)
        {
            if (bandName.Length > 0)
            {
                Band band = db.Bands.Find(bandId);
                band.Name = bandName;
                db.SaveChanges();
                TempData["infoMessage"] = "Changes saved!";
            }
            return RedirectToAction("Index", new { bandId = bandId });
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

        public ActionResult Members(int? bandId)
        {
            CheckSubscription();
            var bands = GetUserBands();
            if (bands.Count <= 0)
            {
                return RedirectToAction("Create");
            }
            Band currentBand = bands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            var viewModel = new BandViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
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

        private List<Band> GetUserBands()
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Subscription)
                .Include(u => u.Subscription.SubscriptionType)
                .Include(u => u.Bands)
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            List<Band> bands = user.Bands.ToList();
            return bands;
        }

        private void CheckSubscription()
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
                Response.Redirect("/Subscription/Create");
            }
        }

        public ActionResult InviteMember(int? bandId, string email, string title, string bandName)
        {
            string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            string plainTextContent = User.Identity.GetUserName() + " has invited you to join the band '" + bandName + "' on BandMate!\r\n\r\nTo join, sign-up and log-in to your account using this email address!\r\n\r\n" + baseUrl;
            string htmlTextContent = "<p>" + User.Identity.GetUserName() + " has invited you to join the band '" + bandName + "' on BandMate!</p><p>To join, sign-up and log-in to your account using the email address: " + email + " </p><p>" + baseUrl + "</p>";
            SendEmail(email, "You've been invited to join BandMate!", plainTextContent, htmlTextContent);
            TempData["infoMessage"] = "Invitation sent to " + email;
            return RedirectToAction("Members", "Band", new { bandId = bandId });
        }

        private void SendEmail(string toEmail, string subject, string plainTextContent, string htmlTextContent)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("info@bandmate.com", "BandMate");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlTextContent);
            var response = client.SendEmailAsync(msg);
        }


    }
}
