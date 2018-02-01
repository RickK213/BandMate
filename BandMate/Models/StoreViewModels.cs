using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class StoreCheckoutViewModel
    {
        public Band Band { get; set; }
        public int StoreId { get; set; }
        public string CartProducts { get; set; }
    }
}