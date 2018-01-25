using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;
using System;

namespace BandMate.Controllers
{
    [Authorize]
    public class VenueController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Create(int bandId)
        {
            ViewBag.BandId = bandId;
            ViewBag.StateId = new SelectList(db.States, "StateId", "Abbreviation");
            return View();
        }

        [HttpPost]
        public ActionResult Create(string venueName, int bandId, string contactFirstName, string contactLastName, string contactPhone, string contactEmail, string streetOne, string city, string StateId, string zipCode, string lat, string lng)
        {
            if (!User.IsInRole("Band Manager"))
            {
                return HttpNotFound();
            }

            //TO DO: CHECK FOR DUPLICATE VENUE, IF EXISTS, JUST ADD IT TO BAND
            Venue venue = new Venue();
            Address address = GetAddress(streetOne, city, StateId, zipCode, lat, lng);
            venue.Name = venueName;
            venue.ContactFirstName = contactFirstName;
            venue.ContactLastName = contactLastName;
            venue.ContactPhoneNumber = contactPhone;
            venue.ContactEmail = contactEmail;
            venue.BandId = bandId;
            venue.AddressId = address.AddressId;
            db.Venues.Add(venue);
            db.SaveChanges();
            return RedirectToAction("Venues", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult Edit(int venueId)
        {
            var venue = db.Venues
                .Include(v => v.Address)
                .Include("Address.City")
                .Include("Address.State")
                .Include("Address.ZipCode")
                .Where(v => v.VenueId == venueId)
                .FirstOrDefault();
            ViewBag.BandId = venue.BandId;
            ViewBag.StateId = new SelectList(db.States, "StateId", "Abbreviation");
            return View(venue);
        }

        [HttpPost]
        public ActionResult Edit(string venueName, int venueId, string contactFirstName, string contactLastName, string contactPhone, string contactEmail, string streetOne, string city, string StateId, string zipCode, string lat, string lng)
        {
            if (!User.IsInRole("Band Manager"))
            {
                return HttpNotFound();
            }

            var venue = db.Venues
                .Include(v => v.Address)
                .Include("Address.City")
                .Include("Address.State")
                .Include("Address.ZipCode")
                .Where(v => v.VenueId == venueId)
                .FirstOrDefault();

            Address address = GetAddress(streetOne, city, StateId, zipCode, lat, lng);
            venue.Name = venueName;
            venue.ContactFirstName = contactFirstName;
            venue.ContactLastName = contactLastName;
            venue.ContactPhoneNumber = contactPhone;
            venue.ContactEmail = contactEmail;
            venue.AddressId = address.AddressId;
            db.SaveChanges();
            return RedirectToAction("Venues", "Band", new { bandId = venue.BandId });
        }

        public ActionResult Delete(int venueId)
        {
            var venue = db.Venues.Find(venueId);
            var band = db.Bands
                .Include(b => b.Venues)
                .Where(b => b.BandId == venue.BandId)
                .FirstOrDefault();
            db.Venues.Remove(venue);
            band.Venues.Remove(venue);
            db.SaveChanges();
            TempData["infoMessage"] = venue.Name + " Deleted!";
            return RedirectToAction("Venues", "Band", new { bandId = band.BandId });
        }

        public ActionResult Details(int venueId)
        {
            var venue = db.Venues
                .Include(v => v.Address)
                .Include("Address.City")
                .Include("Address.State")
                .Include("Address.ZipCode")
                .Where(v => v.VenueId == venueId)
                .FirstOrDefault();
            return View(venue);
        }

        private Address GetAddress(string StreetOne, string City_Name, string StateId, string ZipCode_Number, string lat, string lng)
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
            address.Lat = Convert.ToSingle(lat);
            address.Lng = Convert.ToSingle(lng);
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