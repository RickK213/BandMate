﻿using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Newtonsoft.Json;

namespace BandMate.Controllers
{
    [Authorize]
    public class TourDateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Create(int tourId, int bandId)
        {
            var tour = db.Tours.Find(tourId);
            var band = db.Bands
                .Include(b => b.Venues)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            var setLists = db.SetLists
                .Where(s => s.BandId == tour.BandId)
                .ToList();
            List<Venue> venues = band.Venues.ToList();
            ViewBag.VenueId = new SelectList(venues, "VenueId", "Name");
            ViewBag.SetListId = new SelectList(setLists, "SetListId", "Name");
            return View(tour);
        }

        [HttpPost]
        public ActionResult Create(int tourId, int bandId, DateTime eventDate, double appearanceFee, int SetListId, int VenueId)
        {
            TourDate tourDate = new TourDate();
            tourDate.AppearanceFee = Convert.ToDouble(appearanceFee);
            tourDate.BandId = bandId;
            tourDate.ParentId = tourId;
            tourDate.EventDate = eventDate;
            tourDate.SetListId = SetListId;
            tourDate.VenueId = VenueId;

            Tour tour = db.Tours
                .Include(t => t.TourDates)
                .Where(t => t.TourId == tourId)
                .FirstOrDefault();
            tour.TourDates.Add(tourDate);

            db.TourDates.Add(tourDate);
            db.SaveChanges();
            TempData["infoMessage"] = "Tour date created!";
            return RedirectToAction("Tours", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult Edit(int tourDateId, int bandId)
        {
            var tourDate = db.TourDates.Find(tourDateId);
            var band = db.Bands
                .Include(b => b.Venues)
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();
            var setLists = db.SetLists
                .Where(s => s.BandId == bandId)
                .ToList();
            List<Venue> venues = band.Venues.ToList();
            ViewBag.VenueId = new SelectList(venues, "VenueId", "Name");
            ViewBag.SetListId = new SelectList(setLists, "SetListId", "Name");
            return View(tourDate);
        }

        [HttpPost]
        public ActionResult Edit(int tourDateId, DateTime eventDate, double appearanceFee, int SetListId, int VenueId)
        {
            TourDate tourDate = db.TourDates.Find(tourDateId);

            tourDate.AppearanceFee = Convert.ToDouble(appearanceFee);
            tourDate.EventDate = eventDate;
            tourDate.SetListId = SetListId;
            tourDate.VenueId = VenueId;

            db.SaveChanges();
            TempData["infoMessage"] = "Tour date modified!";
            return RedirectToAction("Details", "Tour", new { tourId = tourDate.ParentId });
        }

        [HttpGet]
        public ActionResult Delete(int tourDateId)
        {
            TourDate tourDate = db.TourDates.Find(tourDateId);
            db.TourDates.Remove(tourDate);
            db.SaveChanges();
            TempData["infoMessage"] = "Tour date deleted!";
            return RedirectToAction("Details", "Tour", new { tourId = tourDate.ParentId });
        }

        [HttpGet]
        public ActionResult MarkAsPaid(int tourDateId)
        {
            TourDate tourDate = db.TourDates
                .Include(t => t.Venue)
                .Where(t => t.TourDateId == tourDateId)
                .FirstOrDefault();
            return View(tourDate);
        }

        [HttpPost]
        public ActionResult MarkAsPaid(int tourDateId, DateTime datePaid)
        {
            TourDate tourDate = db.TourDates.Find(tourDateId);
            tourDate.FeeCollectedOn = datePaid;
            db.SaveChanges();
            TempData["infoMessage"] = String.Format("Tour date on {0:MM/dd/yy} has been marked as paid", tourDate.EventDate);
            return RedirectToAction("Details", "Tour", new { tourId = tourDate.ParentId });
        }

        [HttpGet]
        public ActionResult RemovePayment(int tourDateId)
        {
            TourDate tourDate = db.TourDates.Find(tourDateId);
            tourDate.FeeCollectedOn = null;
            db.SaveChanges();
            TempData["infoMessage"] = String.Format("Payment removed from tour date on {0:MM/dd/yy}.", tourDate.EventDate);
            return RedirectToAction("Details", "Tour", new { tourId = tourDate.ParentId });
        }

        [HttpGet]
        public ActionResult TrackMerchandise(int tourDateId, int bandId)
        {
            var band = db.Bands
                .Include(b => b.Store)
                .Include("Store.Products")
                .Include("Store.Products.ProductType")
                .Include("Store.Products.Sizes")
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            var tourDate = db.TourDates
                .Include(t => t.Venue)
                .Include(t => t.SoldProducts)
                .Where(t => t.TourDateId == tourDateId)
                .FirstOrDefault();

            TourDateTrackMerchandiseViewModel viewModel = new TourDateTrackMerchandiseViewModel();

            viewModel.Band = band;
            viewModel.TourDate = tourDate;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult TrackMerchandise(int tourDateId, int bandId, string productsSold)
        {
            List<SoldProduct> soldProducts = JsonConvert.DeserializeObject<List<SoldProduct>>(productsSold);

            var tourDate = db.TourDates
                .Include(t => t.SoldProducts)
                .Where(t => t.TourDateId == tourDateId)
                .FirstOrDefault();

            var band = db.Bands
                .Include(b => b.Store)
                .Include("Store.Products")
                .Include("Store.Products.ProductType")
                .Include("Store.Products.Sizes")
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            //If we have already tracked the inventory for this tour date, put the inventory back, clear the list of products sold and re-add them
            if (tourDate.SoldProducts.Count > 0)
            {
                //put the inventory back
                foreach (SoldProduct soldProduct in tourDate.SoldProducts)
                {
                    //increment the inventory
                    Product product = db.Products
                        .Include(p => p.ProductType)
                        .Include(p => p.Sizes)
                        .Where(p => p.ProductId == soldProduct.ProductId)
                        .FirstOrDefault();
                    if (soldProduct.ProductTypeId == 2)//Garment
                    {
                        foreach (Size size in product.Sizes)
                        {
                            if (size.SizeId == soldProduct.SizeId)
                            {
                                size.QuantityAvailable++;
                                product.QuantityAvailable++;
                                tourDate.MerchSoldValue -= soldProduct.Price;
                            }
                        }
                    }
                    else//standard product
                    {
                        product.QuantityAvailable++;
                        tourDate.MerchSoldValue -= soldProduct.Price;
                    }
                }
            }

            //products have been returned to inventory, so...
            tourDate.SoldProducts.Clear();
            foreach (SoldProduct soldProduct in soldProducts)
            {
                soldProduct.SoldAtTourDate = true;
                soldProduct.BandId = bandId;
                soldProduct.DateSold = tourDate.EventDate;
                tourDate.SoldProducts.Add(soldProduct);

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
                            tourDate.MerchSoldValue += size.Price;
                        }
                    }
                }
                else
                {
                    product.QuantityAvailable--;
                    tourDate.MerchSoldValue += product.Price;
                }
            }
            db.SaveChanges();
            TempData["infoMessage"] = "Merchandise has been successfully tracked";
            return Json("success", JsonRequestBehavior.AllowGet);
        }

    }
}