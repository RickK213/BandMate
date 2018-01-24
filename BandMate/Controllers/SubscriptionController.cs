using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;

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
    }
}