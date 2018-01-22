using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class ZipCode
    {
        public int ZipCodeId { get; set; }

        [Required]
        public string Number { get; set; }
    }
}