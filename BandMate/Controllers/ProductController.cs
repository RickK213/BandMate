using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BandMate.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

namespace BandMate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create(int bandId)
        {
            ViewBag.BandId = bandId;
            ViewBag.ProductTypeId = new SelectList(db.ProductTypes, "ProductTypeId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(int bandId, string productName, HttpPostedFileBase productImage, string description, double price, int ProductTypeId)
        {
            //Upload Image
            string imageUrl = "";
            if (productImage != null && productImage.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/ProductImages"),
                                               Path.GetFileName(productImage.FileName));
                    imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    imageUrl += "ProductImages/" + productImage.FileName;
                    productImage.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            ProductType productType = db.ProductTypes.Find(ProductTypeId);

            Product product = new Product();
            product.Name = productName;
            product.Description = description;
            product.ImageUrl = imageUrl;
            product.Price = price;
            product.QuantityAvailable = 0;
            product.ProductType = productType;
            db.Products.Add(product);

            if ( productType.ProductTypeId == 2 )//Garment
            {
                //Seed the sizes
                product.Sizes = new List<Size>();

                Size small = new Size();
                small.Name = "Small";
                small.Abbreviation = "S";
                small.QuantityAvailable = 0;
                small.UpCharge = 0;

                Size medium = new Size();
                medium.Name = "Medium";
                medium.Abbreviation = "M";
                medium.QuantityAvailable = 0;
                medium.UpCharge = 0;

                Size large = new Size();
                large.Name = "Large";
                large.Abbreviation = "L";
                large.QuantityAvailable = 0;
                large.UpCharge = 0;

                Size xLarge = new Size();
                xLarge.Name = "X-Large";
                xLarge.Abbreviation = "XL";
                xLarge.QuantityAvailable = 0;
                xLarge.UpCharge = 0;

                Size xxLarge = new Size();
                xxLarge.Name = "XX-Large";
                xxLarge.Abbreviation = "XXL";
                xxLarge.QuantityAvailable = 0;
                xxLarge.UpCharge = 0;

                product.Sizes.Add(small);
                product.Sizes.Add(medium);
                product.Sizes.Add(large);
                product.Sizes.Add(xLarge);
                product.Sizes.Add(xxLarge);

            }

            var band = db.Bands
                .Include(b => b.Store)
                .Include("Store.Products")
                .Where(b => b.BandId == bandId)
                .FirstOrDefault();

            band.Store.Products.Add(product);

            db.SaveChanges();
            TempData["infoMessage"] = "You have added the product: " + product.Name;
            return RedirectToAction("Store", "Band", new { bandId = bandId });
        }

        public ActionResult Delete(int productId, int bandId)
        {
            var product = db.Products
                .Include(p => p.Sizes)
                .Where(p => p.ProductId == productId)
                .FirstOrDefault();
            List<Size> sizesToDelete = product.Sizes.ToList();
            for (int i = sizesToDelete.Count - 1; i >= 0; i--)
            {
                db.Sizes.Remove(sizesToDelete[i]);
            }
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["infoMessage"] = "You have removed the product: " + product.Name;
            return RedirectToAction("Store", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult ManageInventory(int productId, int bandId)
        {
            var product = db.Products
                .Include(p => p.Sizes)
                .Include(p => p.ProductType)
                .Where(p => p.ProductId == productId)
                .FirstOrDefault();
            ViewBag.BandId = bandId;
            return View(product);
        }

        [HttpPost]
        public ActionResult ManageInventory(int productId, int bandId, int? quantityAvailable, int? quantityAvailableS, int? quantityAvailableM, int? quantityAvailableL, int? quantityAvailableXL, int? quantityAvailableXXL)
        {
            var product = db.Products
                .Include(p => p.Sizes)
                .Include(p => p.ProductType)
                .Where(p => p.ProductId == productId)
                .FirstOrDefault();

            if ( product.ProductType.ProductTypeId == 2 )//Garment
            {
                int qtyS = Convert.ToInt32(quantityAvailableS);
                int qtyM = Convert.ToInt32(quantityAvailableM);
                int qtyL = Convert.ToInt32(quantityAvailableL);
                int qtyXL = Convert.ToInt32(quantityAvailableXL);
                int qtyXXL = Convert.ToInt32(quantityAvailableXXL);

                foreach ( Size size in product.Sizes )
                {
                    switch(size.Abbreviation)
                    {
                        case "S":
                            size.QuantityAvailable = qtyS;
                            break;
                        case "M":
                            size.QuantityAvailable = qtyM;
                            break;
                        case "L":
                            size.QuantityAvailable = qtyL;
                            break;
                        case "XL":
                            size.QuantityAvailable = qtyXL;
                            break;
                        case "XXL":
                            size.QuantityAvailable = qtyXXL;
                            break;
                    }
                }
                product.QuantityAvailable = qtyS + qtyM + qtyL + qtyXL + qtyXXL;
            }
            else
            {
                product.QuantityAvailable = Convert.ToInt32(quantityAvailable);
            }

            db.SaveChanges();

            TempData["infoMessage"] = "Inventory updated for the product: " + product.Name;
            return RedirectToAction("Store", "Band", new { bandId = bandId });
        }

        [HttpGet]
        public ActionResult Edit(int productId, int bandId)
        {
            ViewBag.BandId = bandId;
            var product = db.Products
                .Include(p => p.ProductType)
                .Where(p => p.ProductId == productId)
                .FirstOrDefault();
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(int productId, int bandId, string productName, HttpPostedFileBase productImage, string description, double price)
        {
            Product product = db.Products.Find(productId);

            //Upload Image
            if (productImage != null && productImage.ContentLength > 0)
                try
                {
                    string imageUrl = "";
                    string path = Path.Combine(Server.MapPath("~/ProductImages"),
                                               Path.GetFileName(productImage.FileName));
                    imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    imageUrl += "ProductImages/" + productImage.FileName;
                    productImage.SaveAs(path);
                    product.ImageUrl = imageUrl;
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            product.Name = productName;
            product.Description = description;
            product.Price = price;

            db.SaveChanges();
            TempData["infoMessage"] = "You have modified the product: " + product.Name;
            return RedirectToAction("Store", "Band", new { bandId = bandId });
        }

    }
}