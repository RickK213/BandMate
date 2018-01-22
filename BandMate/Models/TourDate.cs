using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class TourDate : Event
    {
        public SetList SetList { get; set; }
        public Venue Venue { get; set; }
        public double AppearanceFee { get; set; }
        public ICollection<Product> ProductsSold { get; set; }
        public DateTime? FeeCollectedOn { get; set; }
    }
}