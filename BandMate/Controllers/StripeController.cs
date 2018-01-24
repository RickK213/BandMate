using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BandMate.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace BandMate.Controllers
{
    public class StripeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult CreateSubscription(string stripeEmail, string stripeToken, string subscriptionType)
        {

            KeyManager keyManager = new KeyManager();
            string stripeKey = keyManager.StripeSecretKey;
            StripeConfiguration.SetApiKey(stripeKey);

            //Create Customer
            var customerOptions = new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
            };
            var customerService = new StripeCustomerService();
            StripeCustomer customer = customerService.Create(customerOptions);

            //Add Customer to Monthly Subscription
            List<StripeSubscriptionItemOption> items;
            if (subscriptionType == "monthly")
            {
                items = new List<StripeSubscriptionItemOption> {
                    new StripeSubscriptionItemOption {PlanId = "bm-monthly"}
                };
            }
            else
            {
                items = new List<StripeSubscriptionItemOption> {
                    new StripeSubscriptionItemOption {PlanId = "bm-annual"}
                };
            }

            var options = new StripeSubscriptionCreateOptions
            {
                Items = items,
            };
            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription stripeSubscription = subscriptionService.Create(customer.Id, options);

            //Save subscription to db
            var subscription = new Subscription();
            subscription.AutoRenewal = true;
            subscription.IsActive = true;
            subscription.StartDate = DateTime.Now;

            if (subscriptionType == "monthly")
            {
                subscription.EndDate = subscription.StartDate.AddMonths(1);
                subscription.SubscriptionTypeId = 1; //Monthly Subscription
            }
            else
            {
                subscription.EndDate = subscription.StartDate.AddYears(1);
                subscription.SubscriptionTypeId = 2; //Annual Subscription
            }

            db.Subscriptions.Add(subscription);
            db.SaveChanges();

            //Set User's subscription Id
            string userId = User.Identity.GetUserId();
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            user.Subscription = subscription;
            db.SaveChanges();

            return RedirectToAction("Index", "Subscription");

        }

    }
}