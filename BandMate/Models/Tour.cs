using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Tour
    {
        public int TourId { get; set; }
        public string Name { get; set; }
        public ICollection<TourDate> TourDates { get; set; }
    }
}