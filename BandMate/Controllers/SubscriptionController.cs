using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using Stripe;

namespace BandMate.Controllers
{
    public class SubscriptionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscription
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Subscription)
                .Include(u => u.Subscription.SubscriptionType)
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            if (user.Subscription == null )
            {
                return RedirectToAction("Create", "Subscription");
            }
            if (TempData["infoMessage"] != null)
            {
                ViewBag.infoMessage = TempData["infoMessage"].ToString();
            }
            if (TempData["dangerMessage"] != null)
            {
                ViewBag.dangerMessage = TempData["dangerMessage"].ToString();
            }
            return View(user.Subscription);
        }

        //
        // GET: /Subscription/Create
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if ( !User.IsInRole("Band Manager") )
            {
                return HttpNotFound();
            }

            //Check if user already has a subscription, redirect if already created
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.Subscription)
                .Include(u => u.Subscription.SubscriptionType)
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            var subscription = user.Subscription;

            if(subscription != null)
            {
                return RedirectToAction("Index", "Subscription");
            }
            var subscriptionTypes = db.SubscriptionTypes.ToList();

            return View(subscriptionTypes);
        }

        public ActionResult Cancel(int subscriptionId)
        {
            var subscription = db.Subscriptions.Find(subscriptionId);
            subscription.AutoRenewal = false;
            db.SaveChanges();
            //do Stripe cancellation
            KeyManager keyManager = new KeyManager();
            string StripeSecretKey = keyManager.StripeSecretKey;
            StripeConfiguration.SetApiKey(StripeSecretKey);

            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription stripeSubscription = subscriptionService.Cancel(subscription.StripeSubscriptionId);

            TempData["infoMessage"] = "Your subscription has been cancelled. You will no longer be able to use BandMate after your subscription end date.";
            return RedirectToAction("Index");
        }

    }
}