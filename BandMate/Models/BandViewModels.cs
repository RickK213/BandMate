using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class BandViewModel
    {
        public Band CurrentBand { get; set; }
        public List<Band> OtherBands { get; set; }
    }

    public class BandIndexViewModel : BandViewModel
    {
    }
}