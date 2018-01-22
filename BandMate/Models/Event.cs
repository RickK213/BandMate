using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
    }
}