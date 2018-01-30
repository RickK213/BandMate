﻿using System;
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

        public double AppearanceFee { get; set; }
        public ICollection<Product> ProductsSold { get; set; }
        public DateTime? FeeCollectedOn { get; set; }

        public int BandId { get; set; }
    }
}