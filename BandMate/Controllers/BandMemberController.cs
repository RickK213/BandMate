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

namespace BandMate.Controllers
{
    [Authorize]
    public class BandMemberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: HOME
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Index(int? bandId)
        {
            var bands = db.Bands
                .Include(b => b.BandMembers)
                .Include("BandMembers")
                .Include("Invitations")
                .Include("Tours")
                .Include("Tours.TourDates")
                .Include("Tours.TourDates.SoldProducts")
                .Include("Venues")
                .Include("Venues.Address")
                .Include("Venues.Address.City")
                .Include("Venues.Address.State")
                .Include("Venues.Address.ZipCode")
                .Include("Songs")
                .Include("SetLists")
                .Include("SetLists.SetListSongs")
                .Include("Events")
                .Include("Store")
                .Include("Store.Products")
                .ToList();
            List<Band> myBands = new List<Band>();
            foreach (Band band in bands)
            {
                foreach (BandMember bandMember in band.BandMembers)
                {
                    if (bandMember.UserId == User.Identity.GetUserId())
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
                if (invitations.Count > 0)
                {
                    return RedirectToAction("Index", "Invitation");
                }
                //User is not a member of any bands and they have no pending invitations. They cannot use the site.
                return RedirectToAction("NoMemberData", "BandMember");
            }
            Band currentBand = myBands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "BandMember", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = myBands.Where(b => b.BandId != bandId).ToList();
            var viewModel = new BandViewModel();
            viewModel.OtherBands = otherBands;
            viewModel.Invitations = invitations;
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
        //DASHBOARD: MY SET LISTS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult SetLists(int? bandId)
        {
            //Common code for all actions in BandMemberController
            var bands = db.Bands
                .Include(b => b.BandMembers)
                .Include("BandMembers")
                .Include("Invitations")
                .Include("Tours")
                .Include("Tours.TourDates")
                .Include("Tours.TourDates.SoldProducts")
                .Include("Venues")
                .Include("Venues.Address")
                .Include("Venues.Address.City")
                .Include("Venues.Address.State")
                .Include("Venues.Address.ZipCode")
                .Include("Songs")
                .Include("SetLists")
                .Include("SetLists.SetListSongs")
                .Include("SetLists.SetListSongs.Song")
                .Include("Events")
                .Include("Store")
                .Include("Store.Products")
                .ToList();
            List<Band> myBands = new List<Band>();
            foreach (Band band in bands)
            {
                foreach (BandMember bandMember in band.BandMembers)
                {
                    if (bandMember.UserId == User.Identity.GetUserId())
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
                if (invitations.Count > 0)
                {
                    return RedirectToAction("Index", "Invitation");
                }
                //User is not a member of any bands and they have no pending invitations. They cannot use the site.
                return RedirectToAction("NoMemberData", "BandMember");
            }
            Band currentBand = myBands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "BandMember", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code/////////////////////////////////////////////////////////////////

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
        //DASHBOARD: MY TOURS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Tours(int? bandId)
        {
            //Common code for all actions in BandMemberController
            var bands = db.Bands
                .Include(b => b.BandMembers)
                .Include("BandMembers")
                .Include("Invitations")
                .Include("Tours")
                .Include("Tours.TourDates")
                .Include("Tours.TourDates.SoldProducts")
                .Include("Venues")
                .Include("Venues.Address")
                .Include("Venues.Address.City")
                .Include("Venues.Address.State")
                .Include("Venues.Address.ZipCode")
                .Include("Songs")
                .Include("SetLists")
                .Include("SetLists.SetListSongs")
                .Include("SetLists.SetListSongs.Song")
                .Include("Events")
                .Include("Store")
                .Include("Store.Products")
                .ToList();
            List<Band> myBands = new List<Band>();
            foreach (Band band in bands)
            {
                foreach (BandMember bandMember in band.BandMembers)
                {
                    if (bandMember.UserId == User.Identity.GetUserId())
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
                if (invitations.Count > 0)
                {
                    return RedirectToAction("Index", "Invitation");
                }
                //User is not a member of any bands and they have no pending invitations. They cannot use the site.
                return RedirectToAction("NoMemberData", "BandMember");
            }
            Band currentBand = myBands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "BandMember", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code/////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////
        //DASHBOARD: MY EVENTS
        ////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Events(int? bandId)
        {
            //Common code for all actions in BandMemberController
            var bands = db.Bands
                .Include(b => b.BandMembers)
                .Include("BandMembers")
                .Include("Invitations")
                .Include("Tours")
                .Include("Tours.TourDates")
                .Include("Tours.TourDates.SoldProducts")
                .Include("Venues")
                .Include("Venues.Address")
                .Include("Venues.Address.City")
                .Include("Venues.Address.State")
                .Include("Venues.Address.ZipCode")
                .Include("Songs")
                .Include("SetLists")
                .Include("SetLists.SetListSongs")
                .Include("SetLists.SetListSongs.Song")
                .Include("Events")
                .Include("Store")
                .Include("Store.Products")
                .ToList();
            List<Band> myBands = new List<Band>();
            foreach (Band band in bands)
            {
                foreach (BandMember bandMember in band.BandMembers)
                {
                    if (bandMember.UserId == User.Identity.GetUserId())
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
                if (invitations.Count > 0)
                {
                    return RedirectToAction("Index", "Invitation");
                }
                //User is not a member of any bands and they have no pending invitations. They cannot use the site.
                return RedirectToAction("NoMemberData", "BandMember");
            }
            Band currentBand = myBands[0];
            if (bandId != null)
            {
                currentBand = bands.Where(b => b.BandId == bandId).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "BandMember", new { bandId = currentBand.BandId });
            }
            List<Band> otherBands;
            otherBands = bands.Where(b => b.BandId != bandId).ToList();
            //End of common code/////////////////////////////////////////////////////////////////

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
                foreach (TourDate tourDate in tour.TourDates)
                {
                    tourDates.Add(tourDate);
                }
            }
            foreach (TourDate tourDate in tourDates)
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


        public ActionResult NoMemberData()
        {
            return View();
        }

        public ActionResult LeaveBand(int id)
        {
            string userId = User.Identity.GetUserId();
            Band band = db.Bands.Find(id);

            //YOU ARE HERE...GET THE BAND MEMBER BY USER ID AND BAND ID WITHIN THE FOREACH AND DELETE IT FROM THE DB THERE.

            var bandMembers = db.BandMembers.ToList();
            foreach ( BandMember bandMember in bandMembers )
            {
                if (bandMember.UserId == userId && bandMember.BandId == id )
                {
                    db.BandMembers.Remove(bandMember);
                    db.SaveChanges();
                }
            }

            TempData["infoMessage"] = "You have left the band '"+ band.Name +"'";
            return RedirectToAction("Index", "BandMember");
        }

    }
}