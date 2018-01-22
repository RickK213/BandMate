using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}