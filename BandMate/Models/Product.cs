using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public int QuantityAvailable { get; set; }
        public ICollection<Size> Sizes { get; set; }
        public ProductType ProductType { get; set; }
    }
}