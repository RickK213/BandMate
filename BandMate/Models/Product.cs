using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageFileName { get; set; }
        public double Price { get; set; }
        public int QuantityAvailable { get; set; }
        public bool IsSoldOut { get; set; }
        public ICollection<Size> Sizes { get; set; }
        public ProductType ProductType { get; set; }
    }
}