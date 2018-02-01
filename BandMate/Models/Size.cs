using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Size
    {
        public int SizeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Abbreviation { get; set; }

        public double? UpCharge { get; set; }
        public double Price { get; set; }
        public int QuantityAvailable { get; set; }

    }
}