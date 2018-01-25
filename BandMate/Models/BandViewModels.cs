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

    public class BandMemberViewModel : BandViewModel
    {
        public List<ApplicationUser> CurrentBandMembers { get; set; }
    }

    public class BandTourViewModel : BandViewModel
    {
        public List<Tour> CurrentBandTours { get; set; }
    }

}