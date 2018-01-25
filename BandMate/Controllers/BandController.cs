﻿using System;
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
            List<ApplicationUser> currentBandMembers = GetBandMembers(currentBand);
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
            band.BandMembers = new List<BandMember>();
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
            List<ApplicationUser> currentBandMembers = GetBandMembers(currentBand);
            var viewModel = new BandMemberViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
            viewModel.CurrentBandMembers = currentBandMembers;
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

        private List<ApplicationUser> GetBandMembers(Band band)
        {
            List<ApplicationUser> bandMembers = new List<ApplicationUser>();
            foreach (BandMember bandMember in band.BandMembers)
            {
                ApplicationUser user = db.Users.Where(u => u.Id == bandMember.UserId).FirstOrDefault();
                user.Title = bandMember.Title;
                user.BandMemberId = bandMember.BandMemberId;
                bandMembers.Add(user);
            }
            return bandMembers;
        }

        private List<Band> GetUserBands()
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Subscription)
                .Include(u => u.Subscription.SubscriptionType)
                .Include(u => u.Bands)
                .Include("Bands.BandMembers")
                .Include("Bands.Invitations")
                .Include("Bands.Tours")
                .Include("Bands.Tours.TourDates")
                .Include("Bands.Tours.TourDates.ProductsSold")
                .Include("Bands.Venues")
                .Include("Bands.SetLists")
                .Include("Bands.SetLists.Songs")
                .Include("Bands.Events")
                .Include("Bands.Store")
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

        public ActionResult InviteMember(int? bandId, string email, string title)
        {
            Band band = db.Bands
                .Include(b => b.Invitations)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            if (band == null)
            {
                return HttpNotFound();
            }
            string bandName = band.Name;
            string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            string plainTextContent = User.Identity.GetUserName() + " has invited you to join the band '" + bandName + "' on BandMate!\r\n\r\nTo join, sign-up and log-in to your account using this email address!\r\n\r\n" + baseUrl;
            string htmlTextContent = "<p>" + User.Identity.GetUserName() + " has invited you to join the band '" + bandName + "' on BandMate!</p><p>To join, sign-up and log-in to your account using the email address: " + email + " </p><p>" + baseUrl + "</p>";
            SendEmail(email, "You've been invited to join BandMate!", plainTextContent, htmlTextContent);

            Invitation invitation = new Invitation();
            invitation.Email = email;
            invitation.Title = title;
            invitation.InvitedBy = User.Identity.GetUserName();
            invitation.BandName = band.Name;
            invitation.CreatedOn = DateTime.Now;
            invitation.IsAccepted = false;
            invitation.BandId = band.BandId;
            db.Invitations.Add(invitation);
            db.SaveChanges();

            band.Invitations.Add(invitation);
            db.SaveChanges();

            TempData["infoMessage"] = "Invitation sent to " + email;
            return RedirectToAction("Members", "Band", new { bandId = bandId });
        }

        private void SendEmail(string toEmail, string subject, string plainTextContent, string htmlTextContent)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            KeyManager keyManager = new KeyManager();
            var apiKey = keyManager.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("info@bandmate.com", "BandMate");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlTextContent);
            var response = client.SendEmailAsync(msg);
        }

        public ActionResult MemberBands(int? bandId)
        {
            var bands = db.Bands.Include(b =>b.BandMembers).ToList();
            List<Band> myBands = new List<Band>();
            foreach(Band band in bands)
            {
                foreach(BandMember bandMember in band.BandMembers)
                {
                    if ( bandMember.UserId == User.Identity.GetUserId() )
                    {
                        myBands.Add(band);
                    }
                }
            }

            //Get the Band Member's Invitations
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Bands)
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            var invitations = db.Invitations
                .Where(i => i.IsAccepted == false)
                .Where(i => i.Email == user.Email)
                .ToList();

            if (myBands.Count <= 0)
            {
                //User is not a member of any bands. Do they have any invitations?
                if ( invitations.Count > 0 )
                {
                    return RedirectToAction("Index", "Invitation");
                }
                //User is not a member of any bands and they have no pending invitations. They cannot use the site.
                return RedirectToAction("NoMemberData", "Band");
            }
            Band currentBand = myBands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("MemberBands", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = myBands.Where(b => b.BandId != bandId).ToList();
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

        public ActionResult RemoveMember(int? bandId, int bandMemberId)
        {
            var band = db.Bands
                .Include(b => b.BandMembers)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            BandMember bandMember = band.BandMembers.Where(m => m.BandMemberId == bandMemberId).FirstOrDefault();
            band.BandMembers.Remove(bandMember);
            db.SaveChanges();
            TempData["infoMessage"] = bandMember.Title + " removed from your band.";
            return RedirectToAction("Members", "Band", new { bandId = bandId });
        }

        public ActionResult NoMemberData()
        {
            return View();
        }

        public ActionResult Tours(int? bandId)
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

            //Beginning of action-specific code
            var viewModel = new BandTourViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
            viewModel.CurrentBandTours = currentBand.Tours.ToList();

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


    }
}
