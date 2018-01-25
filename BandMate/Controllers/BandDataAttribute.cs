using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace BandMate.Controllers
{
	public class BandDataAttribute : ActionFilterAttribute
	{
        //member variables
        private ApplicationDbContext db = new ApplicationDbContext();
        int? bandId;

        //properties
        public Band CurrentBand { get; set; }
        public List<Band> OtherBands { get; set; }
        public string BandId { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionParameters.ContainsKey(BandId))
            {
                bandId = filterContext.ActionParameters[BandId] as Int32?;
            }
            var bands = GetUserBands();
            if (bands.Count <= 0)
            {
                var controller = (BandController)filterContext.Controller;
                filterContext.Result = controller.RedirectToAction("Create", "Band", null);
            }
            Band currentBand = bands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                var controller = (BandController)filterContext.Controller;
                filterContext.Result = controller.RedirectToAction("Index", "Band", currentBand.BandId);
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            filterContext.RouteData.Values.Add("currentBand", currentBand);
            filterContext.RouteData.Values.Add("otherBands", otherBands);
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {


            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();


            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }

        private List<Band> GetUserBands()
        {
            string userId = HttpContext.Current.User.Identity.GetUserId();
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
                .Include("Bands.Store.Products")
                .Where(u => u.Id == userId)
                .FirstOrDefault();
            List<Band> bands = user.Bands.ToList();
            return bands;
        }

    }
}