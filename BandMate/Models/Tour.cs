using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Tour
    {
        public int TourId { get; set; }
        public int BandId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<TourDate> TourDates { get; set; }
    }
}