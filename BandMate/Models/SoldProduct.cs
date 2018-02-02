using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class SoldProduct
    {
        public int SoldProductId { get; set; }
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public int? SizeId { get; set; }
        public double Price { get; set; }
        public bool SoldAtTourDate { get; set; }
        public DateTime DateSold { get; set; }
    }
}