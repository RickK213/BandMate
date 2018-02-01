using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class TourDate
    {
        public int TourDateId { get; set; }
        public DateTime EventDate { get; set; }

        public SetList SetList { get; set; }
        public int SetListId { get; set; }

        public Venue Venue { get; set; }
        public int VenueId { get; set; }

        public int ParentId { get; set; }
        public double AppearanceFee { get; set; }
        public double MerchSoldValue { get; set; }
        public ICollection<SoldProduct> SoldProducts { get; set; }
        public DateTime? FeeCollectedOn { get; set; }

        public int BandId { get; set; }
    }
}