using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BandMate.Models;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
using System.Text;

namespace BandMate.Controllers
{
    [Authorize]
    public class BandController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: HOME
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Index(int? bandId)
        {
            //Common code for all actions in BandController
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
            //End of common code

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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: BAND MEMBERS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Members(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("Members", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code

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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY TOURS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Tours(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("Tours", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code

            //Chart Data!!!//////////////////////////////////////////////////////////////
            //var data = {
            // A labels array that can contain any sort of values
            //labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
            // Our series array that contains series objects or in this case series data arrays
            //series: [
            //      [5, 2, 4, 2, 0]
            //    ]
            //};
            DateTime date = DateTime.Now.AddDays(-4);
            List<String> labels = new List<String>();
            List<Double> series = new List<Double>();
            for (int i=0; i<5; i++)
            {
                Double dailyAppearanceFeeCollected = 0d;
                foreach (Tour tour in currentBand.Tours)
                {
                    foreach (TourDate tourDate in tour.TourDates)
                    {
                        DateTime feeCollectedDate = tourDate.FeeCollectedOn ?? DateTime.Now.AddDays(365);
                        if (feeCollectedDate.Date == date.Date )
                        {
                            dailyAppearanceFeeCollected += tourDate.AppearanceFee;
                        }   
                    }
                }
                series.Add(dailyAppearanceFeeCollected);
                labels.Add(String.Format("{0:MM/dd/yy}", date));
                date = date.AddDays(1);
            }

            StringBuilder chartData = new StringBuilder();
            chartData.Append("{");
            chartData.Append("labels: ['"+ labels[0]+"', '" + labels[1] + "', '" + labels[2] + "', '" + labels[3] + "', '" + labels[4] + "'],");
            chartData.Append("series: [");
            chartData.Append("[" + series[0] + ", " + series[1] + "," + series[2] + "," + series[3] + ", " + series[4] + "]");
            chartData.Append("]");
            chartData.Append("};");

            ///////////////////////

            var viewModel = new BandTourViewModel();
            viewModel.ChartData = chartData.ToString();
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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY VENUES
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Venues(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("Venues", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code

            var viewModel = new BandVenueViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
            viewModel.CurrentBandVenues = currentBand.Venues.ToList();
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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY SONGS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Songs(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("Venues", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code

            var viewModel = new BandSongViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
            viewModel.CurrentBandSongs = currentBand.Songs.ToList();
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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY SET LISTS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult SetLists(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("SetLists", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code

            var viewModel = new BandSetListViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
            viewModel.CurrentBandSetLists = currentBand.SetLists.ToList();
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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY EVENTS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Events(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("Events", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code

            var viewModel = new BandEventViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;

            //get all the events
            viewModel.CurrentBandEvents = currentBand.Events.ToList();
            string eventsJson = "[";
            foreach (Event bandEvent in currentBand.Events)
            {
                eventsJson += "{";
                eventsJson += "\"id\": " + bandEvent.EventId + ",";
                eventsJson += "\"title\": \"" + bandEvent.Name + "\",";
                eventsJson += "\"start\": \"" + bandEvent.EventDate.ToString("r") + "\",";
                eventsJson += "\"description\": \"" + bandEvent.Description + "\"";
                eventsJson += "},";
            }

            //add all of the tour dates to the list of events too
            var tours = currentBand.Tours;
            List<TourDate> tourDates = new List<TourDate>();
            foreach (Tour tour in tours)
            {
                foreach(TourDate tourDate in tour.TourDates)
                {
                    tourDates.Add(tourDate);
                }
            }
            foreach ( TourDate tourDate in tourDates )
            {
                eventsJson += "{";
                eventsJson += "\"id\": \"0\",";
                eventsJson += "\"title\": \"Tour Date: " + tourDate.Venue.Name + "\",";
                eventsJson += "\"color\": \"#f89406\",";
                eventsJson += "\"start\": \"" + tourDate.EventDate.ToString("r") + "\",";
                eventsJson += "\"description\": \"Tour Date at " + tourDate.Venue.Name + "\"";
                eventsJson += "},";
            }
            eventsJson += "]";
            viewModel.EventsJson = eventsJson;
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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY STORE
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Store(int? bandId)
        {
            //Common code for all actions in BandController
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
                return RedirectToAction("Store", "Band", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code


            //Chart Data!!!//////////////////////////////////////////////////////////////
            //var data = {
            // A labels array that can contain any sort of values
            //labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
            // Our series array that contains series objects or in this case series data arrays
            //series: [
            //      [5, 2, 4, 2, 0]
            //    ]
            //};
            DateTime date = DateTime.Now.AddDays(-4);
            List<String> labels = new List<String>();
            List<Double> storeSeries = new List<Double>();
            List<Double> tourDateSeries = new List<Double>();
            var soldProducts = db.SoldProducts
                .Where(s => s.BandId == currentBand.BandId)
                .ToList();
            for (int i = 0; i < 5; i++)
            {
                Double dailyStoreSales = 0d;
                Double dailyTourDateSales = 0d;

                foreach (SoldProduct soldProduct in soldProducts)
                {
                    if ( soldProduct.DateSold.Date == date.Date )
                    {
                        if (soldProduct.SoldAtTourDate)
                        {
                            dailyTourDateSales += soldProduct.Price;
                        }
                        if (!soldProduct.SoldAtTourDate)
                        {
                            dailyStoreSales += soldProduct.Price;
                        }
                    }
                }
                storeSeries.Add(Math.Round(dailyStoreSales, 2));
                tourDateSeries.Add(Math.Round(dailyTourDateSales,2));
                labels.Add(String.Format("{0:MM/dd/yy}", date));
                date = date.AddDays(1);
            }

            StringBuilder chartData = new StringBuilder();
            chartData.Append("{");
            chartData.Append("labels: ['" + labels[0] + "', '" + labels[1] + "', '" + labels[2] + "', '" + labels[3] + "', '" + labels[4] + "'],");
            chartData.Append("series: [");
            chartData.Append("[" + storeSeries[0] + ", " + storeSeries[1] + "," + storeSeries[2] + "," + storeSeries[3] + ", " + storeSeries[4] + "],");
            chartData.Append("[" + tourDateSeries[0] + ", " + tourDateSeries[1] + "," + tourDateSeries[2] + "," + tourDateSeries[3] + ", " + tourDateSeries[4] + "]");
            chartData.Append("]");
            chartData.Append("};");

            ///////////////////////



            var viewModel = new BandStoreViewModel();
            viewModel.ChartData = chartData.ToString();
            viewModel.OtherBands = otherBands;
            viewModel.CurrentBand = currentBand;
            viewModel.CurrentBandProducts = currentBand.Store.Products.ToList();
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

        // GET: Band/Create
        public ActionResult Create()
        {
            return View();
        }
        
        // POST: Band/Create
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

        // POST: Band/Create
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

        //MOVE THIS TO BAND MEMBER CONTROLLER!!!!!!!!!!!!!!!!!!!!!!!!
        //public ActionResult MemberBands(int? bandId)
        //{
        //    var bands = db.Bands.Include(b =>b.BandMembers).ToList();
        //    List<Band> myBands = new List<Band>();
        //    foreach(Band band in bands)
        //    {
        //        foreach(BandMember bandMember in band.BandMembers)
        //        {
        //            if ( bandMember.UserId == User.Identity.GetUserId() )
        //            {
        //                myBands.Add(band);
        //            }
        //        }
        //    }
        //    //Get the Band Member's Invitations
        //    string userId = User.Identity.GetUserId();
        //    var user = db.Users
        //        .Include(u => u.Bands)
        //        .Where(u => u.Id == userId)
        //        .FirstOrDefault();
        //    var invitations = db.Invitations
        //        .Where(i => i.IsAccepted == false)
        //        .Where(i => i.Email == user.Email)
        //        .ToList();

        //    if (myBands.Count <= 0)
        //    {
        //        //User is not a member of any bands. Do they have any invitations?
        //        if ( invitations.Count > 0 )
        //        {
        //            return RedirectToAction("Index", "Invitation");
        //        }
        //        //User is not a member of any bands and they have no pending invitations. They cannot use the site.
        //        return RedirectToAction("NoMemberData", "Band");
        //    }
        //    Band currentBand = myBands[0];
        //    if (bandId != null)
        //    {
        //        currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
        //    }
        //    else
        //    {
        //        return RedirectToAction("MemberBands", "Band", new { bandId = currentBand.BandId });
        //    }
        //    List<Band> otherBands;
        //    otherBands = myBands.Where(b => b.BandId != bandId).ToList();
        //    var viewModel = new BandViewModel();
        //    viewModel.OtherBands = otherBands;
        //    viewModel.CurrentBand = currentBand;
        //    if (TempData["infoMessage"] != null)
        //    {
        //        ViewBag.infoMessage = TempData["infoMessage"].ToString();
        //    }
        //    if (TempData["dangerMessage"] != null)
        //    {
        //        ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
        //    }
        //    return View(viewModel);
        //}

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

        //For redirect from action filter:
        public RedirectToRouteResult RedirectToAction(string action, string controller, int? bandId)
        {
            if (bandId == null )
            {
                return base.RedirectToAction(action, controller);
            }
            return base.RedirectToAction(action, controller, new { bandId = bandId });
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
                .Include("Bands.Tours.TourDates.SoldProducts")
                .Include("Bands.Venues")
                .Include("Bands.Venues.Address")
                .Include("Bands.Venues.Address.City")
                .Include("Bands.Venues.Address.State")
                .Include("Bands.Venues.Address.ZipCode")
                .Include("Bands.Songs")
                .Include("Bands.SetLists")
                .Include("Bands.SetLists.SetListSongs")
                .Include("Bands.Events")
                .Include("Bands.Store")
                .Include("Bands.Store.Products")
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            List<Band> bands = user.Bands.ToList();
            foreach (Band band in bands)
            {
                //band.SetLists = band.SetLists.OrderBy(s => s.Name);
                foreach (var setList in band.SetLists)
                {
                    setList.SetListSongs = setList.SetListSongs.OrderBy(s => s.SetListOrder).ToList();
                }
            }
            return bands;
        }

    }
}
