//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using BandMate.Models;
//using Microsoft.AspNet.Identity;
//using SendGrid;
//using SendGrid.Helpers.Mail;
//using System.Threading.Tasks;
//using System.Web.UI;

//namespace BandMate.Controllers
//{
//    [Authorize]
//    //[BandData(BandId = "bandId")]
//    public class BandController : Controller
//    {

//        private ApplicationDbContext db = new ApplicationDbContext();

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: HOME
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Index(int? bandId)
//        {
//            CheckSubscription();
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            var viewModel = new BandViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: BAND MEMBERS
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Members(int? bandId)
//        {
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            List<ApplicationUser> currentBandMembers = GetBandMembers(currentBand);
//            var viewModel = new BandMemberViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            viewModel.CurrentBandMembers = currentBandMembers;
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: MY TOURS
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Tours(int? bandId)
//        {
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            var viewModel = new BandTourViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            viewModel.CurrentBandTours = currentBand.Tours.ToList();
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: MY VENUES
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Venues(int? bandId)
//        {
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            var viewModel = new BandVenueViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            viewModel.CurrentBandVenues = currentBand.Venues.ToList();
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: MY SET LISTS
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult SetLists(int? bandId)
//        {
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            var viewModel = new BandSetListViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            viewModel.CurrentBandSetLists = currentBand.SetLists.ToList();
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: MY EVENTS
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Events(int? bandId)
//        {
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            var viewModel = new BandEventViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            viewModel.CurrentBandEvents = currentBand.Events.ToList();
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        ////////////////////////////////////////////////////////////////////////////////////////
//        //DASHBOARD: MY STORE
//        ////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Store(int? bandId)
//        {
//            Band currentBand = (Band)RouteData.Values["currentBand"];
//            List<Band> otherBands = (List<Band>)RouteData.Values["otherBands"];
//            var viewModel = new BandStoreViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            viewModel.CurrentBandProducts = currentBand.Store.Products.ToList();
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        // POST: Band/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(string Name)
//        {
//            Store store = new Store();
//            db.Stores.Add(store);
//            db.SaveChanges();

//            Band band = new Band();
//            band.Name = Name;
//            band.Store = store;
//            band.BandMembers = new List<BandMember>();
//            db.Bands.Add(band);
//            db.SaveChanges();

//            string userId = User.Identity.GetUserId();
//            var user = db.Users
//                .Include(u => u.Bands)
//                .Where(u => u.Id == userId)
//                .FirstOrDefault();

//            user.Bands.Add(band);
//            db.SaveChanges();

//            TempData["infoMessage"] = "Success! " + band.Name + " created.";

//            return RedirectToAction("Index", new { bandId = band.BandId });
//        }

//        // POST: Band/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(string bandName, int bandId)
//        {
//            if (bandName.Length > 0)
//            {
//                Band band = db.Bands.Find(bandId);
//                band.Name = bandName;
//                db.SaveChanges();
//                TempData["infoMessage"] = "Changes saved!";
//            }
//            return RedirectToAction("Index", new { bandId = bandId });
//        }

//        // GET: Band/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Band band = db.Bands.Find(id);
//            if (band == null)
//            {
//                return HttpNotFound();
//            }
//            return View(band);
//        }

//        // POST: Band/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Band band = db.Bands.Find(id);
//            db.Bands.Remove(band);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        private List<ApplicationUser> GetBandMembers(Band band)
//        {
//            List<ApplicationUser> bandMembers = new List<ApplicationUser>();
//            foreach (BandMember bandMember in band.BandMembers)
//            {
//                ApplicationUser user = db.Users.Where(u => u.Id == bandMember.UserId).FirstOrDefault();
//                user.Title = bandMember.Title;
//                user.BandMemberId = bandMember.BandMemberId;
//                bandMembers.Add(user);
//            }
//            return bandMembers;
//        }

//        private void CheckSubscription()
//        {
//            string userId = User.Identity.GetUserId();
//            var user = db.Users
//                .Include(u => u.Subscription)
//                .Include(u => u.Subscription.SubscriptionType)
//                .Include(u => u.Bands)
//                .Where(u => u.Id == userId)
//                .FirstOrDefault();
//            if (user.Subscription == null)
//            {
//                Response.Redirect("/Subscription/Create");
//            }
//        }

//        public ActionResult InviteMember(int? bandId, string email, string title)
//        {
//            Band band = db.Bands
//                .Include(b => b.Invitations)
//                .Where(b => b.BandId == bandId)
//                .FirstOrDefault();
//            if (band == null)
//            {
//                return HttpNotFound();
//            }
//            string bandName = band.Name;
//            string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
//            string plainTextContent = User.Identity.GetUserName() + " has invited you to join the band '" + bandName + "' on BandMate!\r\n\r\nTo join, sign-up and log-in to your account using this email address!\r\n\r\n" + baseUrl;
//            string htmlTextContent = "<p>" + User.Identity.GetUserName() + " has invited you to join the band '" + bandName + "' on BandMate!</p><p>To join, sign-up and log-in to your account using the email address: " + email + " </p><p>" + baseUrl + "</p>";
//            SendEmail(email, "You've been invited to join BandMate!", plainTextContent, htmlTextContent);

//            Invitation invitation = new Invitation();
//            invitation.Email = email;
//            invitation.Title = title;
//            invitation.InvitedBy = User.Identity.GetUserName();
//            invitation.BandName = band.Name;
//            invitation.CreatedOn = DateTime.Now;
//            invitation.IsAccepted = false;
//            invitation.BandId = band.BandId;
//            db.Invitations.Add(invitation);
//            db.SaveChanges();

//            band.Invitations.Add(invitation);
//            db.SaveChanges();

//            TempData["infoMessage"] = "Invitation sent to " + email;
//            return RedirectToAction("Members", "Band", new { bandId = bandId });
//        }

//        private void SendEmail(string toEmail, string subject, string plainTextContent, string htmlTextContent)
//        {
//            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
//            KeyManager keyManager = new KeyManager();
//            var apiKey = keyManager.SendGridKey;
//            var client = new SendGridClient(apiKey);
//            var from = new EmailAddress("info@bandmate.com", "BandMate");
//            var to = new EmailAddress(toEmail);
//            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlTextContent);
//            var response = client.SendEmailAsync(msg);
//        }

//        public ActionResult MemberBands(int? bandId)
//        {
//            var bands = db.Bands.Include(b =>b.BandMembers).ToList();
//            List<Band> myBands = new List<Band>();
//            foreach(Band band in bands)
//            {
//                foreach(BandMember bandMember in band.BandMembers)
//                {
//                    if ( bandMember.UserId == User.Identity.GetUserId() )
//                    {
//                        myBands.Add(band);
//                    }
//                }
//            }
//            //Get the Band Member's Invitations
//            string userId = User.Identity.GetUserId();
//            var user = db.Users
//                .Include(u => u.Bands)
//                .Where(u => u.Id == userId)
//                .FirstOrDefault();
//            var invitations = db.Invitations
//                .Where(i => i.IsAccepted == false)
//                .Where(i => i.Email == user.Email)
//                .ToList();

//            if (myBands.Count <= 0)
//            {
//                //User is not a member of any bands. Do they have any invitations?
//                if ( invitations.Count > 0 )
//                {
//                    return RedirectToAction("Index", "Invitation");
//                }
//                //User is not a member of any bands and they have no pending invitations. They cannot use the site.
//                return RedirectToAction("NoMemberData", "Band");
//            }
//            Band currentBand = myBands[0];
//            if (bandId != null)
//            {
//                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
//            }
//            else
//            {
//                return RedirectToAction("MemberBands", "Band", new { bandId = currentBand.BandId });
//            }
//            List<Band> otherBands;
//            otherBands = myBands.Where(b => b.BandId != bandId).ToList();
//            var viewModel = new BandViewModel();
//            viewModel.OtherBands = otherBands;
//            viewModel.CurrentBand = currentBand;
//            if (TempData["infoMessage"] != null)
//            {
//                ViewBag.infoMessage = TempData["infoMessage"].ToString();
//            }
//            if (TempData["dangerMessage"] != null)
//            {
//                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
//            }
//            return View(viewModel);
//        }

//        public ActionResult RemoveMember(int? bandId, int bandMemberId)
//        {
//            var band = db.Bands
//                .Include(b => b.BandMembers)
//                .Where(b => b.BandId == bandId)
//                .FirstOrDefault();

//            BandMember bandMember = band.BandMembers.Where(m => m.BandMemberId == bandMemberId).FirstOrDefault();
//            band.BandMembers.Remove(bandMember);
//            db.SaveChanges();
//            TempData["infoMessage"] = bandMember.Title + " removed from your band.";
//            return RedirectToAction("Members", "Band", new { bandId = bandId });
//        }

//        public ActionResult NoMemberData()
//        {
//            return View();
//        }

//        //For redirect from action filter:
//        public RedirectToRouteResult RedirectToAction(string action, string controller, int? bandId)
//        {
//            if (bandId == null )
//            {
//                return base.RedirectToAction(action, controller);
//            }
//            return base.RedirectToAction(action, controller, new { bandId = bandId });
//        }
//    }
//}
