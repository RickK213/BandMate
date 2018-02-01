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
using Newtonsoft.Json;

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

        public ActionResult StoreCheckout(string stripeEmail, string stripeToken, int bandId, string productsSold, string firstName, string lastName, string streetOne, string city, string StateId, string zipCode, double totalPrice)
        {

            List<SoldProduct> soldProducts = JsonConvert.DeserializeObject<List<SoldProduct>>(productsSold);

            var band = db.Bands
                .Include(b => b.Store)
                .Include("Store.Products")
                .Include("Store.Products.ProductType")
                .Include("Store.Products.Sizes")
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            //STRIPE STUFF///////////////////////////////////////////////////////
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

            // Charge the user's card:

            int amountInCents = Convert.ToInt32(totalPrice * 100);
            var charges = new StripeChargeService();
            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Description = band.Name+" Band Merch Order",
                SourceTokenOrExistingSourceId = stripeToken
            });
            /////////////////////////////////////////////////////////////////////////

            Transaction transaction = new Transaction();
            transaction.CreatedOn = DateTime.Now;
            Address address = GetAddress(streetOne, city, StateId, zipCode);
            transaction.CustomerAddress = address;
            transaction.CustomerFirstName = firstName;
            transaction.CustomerLastName = lastName;
            transaction.TotalPrice = totalPrice;
            transaction.ConfirmationNumber = stripeToken;
            transaction.CustomerEmail = stripeEmail;
            transaction.SoldProducts = new List<SoldProduct>();

            //decrease inventory and create a transaction
            foreach (SoldProduct soldProduct in soldProducts)
            {
                soldProduct.SoldAtTourDate = false;
                transaction.SoldProducts.Add(soldProduct);

                //decrement the inventory
                Product product = db.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Sizes)
                    .Where(p => p.ProductId == soldProduct.ProductId)
                    .FirstOrDefault();
                if (product.ProductType.ProductTypeId == 2)//Garment
                {
                    foreach (Size size in product.Sizes)
                    {
                        if (size.SizeId == soldProduct.SizeId)
                        {
                            size.QuantityAvailable--;
                            product.QuantityAvailable--;
                        }
                    }
                }
                else
                {
                    product.QuantityAvailable--;
                }
            }
            db.SaveChanges();

            TempData["storeId"] = band.Store.StoreId;
            TempData["bandName"] = band.Name;
            TempData["orderConfirmation"] = transaction.ConfirmationNumber;
            return RedirectToAction("ThankYou", "Store");
        }

        //TO DO: move these duplicate helper methods (they are also in the VenueController) to a static class
        private Address GetAddress(string StreetOne, string City_Name, string StateId, string ZipCode_Number)
        {
            int stateIdNumber = Convert.ToInt32(StateId);
            if (db.Addresses.Any(a => a.StreetOne == StreetOne && a.City.Name == City_Name && a.State.StateId == stateIdNumber && a.ZipCode.Number == ZipCode_Number))
            {
                var addressFound = db.Addresses.First(a => a.StreetOne == StreetOne && a.City.Name == City_Name && a.State.StateId == stateIdNumber && a.ZipCode.Number == ZipCode_Number);
                return addressFound;
            }
            Address address = new Address();
            address.StreetOne = StreetOne;
            address.CityId = GetCityID(City_Name);
            address.StateId = GetStateID(StateId);
            address.ZipCodeId = GetZipCodeID(ZipCode_Number);
            //address.Lat = Convert.ToSingle(lat);
            //address.Lng = Convert.ToSingle(lng);
            db.Addresses.Add(address);
            db.SaveChanges();
            return address;
        }

        private int GetStateID(string StateId)
        {
            int stateIdNumber = Convert.ToInt32(StateId);
            var stateFound = db.States.First(s => s.StateId == stateIdNumber);
            return stateFound.StateId;
        }

        private int GetZipCodeID(string ZipCode_Number)
        {
            if (db.ZipCodes.Any(z => z.Number == ZipCode_Number))
            {
                var zipCodeFound = db.ZipCodes.First(z => z.Number == ZipCode_Number);
                return zipCodeFound.ZipCodeId;
            }
            ZipCode zipCode = new ZipCode();
            zipCode.Number = ZipCode_Number;
            db.ZipCodes.Add(zipCode);
            db.SaveChanges();
            return zipCode.ZipCodeId;
        }

        private int GetCityID(string City_Name)
        {
            if (db.Cities.Any(c => c.Name.ToLower() == City_Name.ToLower()))
            {
                var cityFound = db.Cities.First(c => c.Name == City_Name);
                return cityFound.CityId;
            }
            City city = new City();
            string formattedName = char.ToUpper(City_Name[0]) + City_Name.Substring(1).ToLower();
            city.Name = formattedName;
            db.Cities.Add(city);
            db.SaveChanges();
            return city.CityId;
        }


    }
}