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

    public class BandVenueViewModel : BandViewModel
    {
        public List<Venue> CurrentBandVenues { get; set; }
    }

    public class BandSongViewModel : BandViewModel
    {
        public List<Song> CurrentBandSongs { get; set; }
    }

    public class BandSetListViewModel : BandViewModel
    {
        public List<SetList> CurrentBandSetLists { get; set; }
    }

    public class BandEventViewModel : BandViewModel
    {
        public List<Event> CurrentBandEvents { get; set; }
    }

    public class BandStoreViewModel : BandViewModel
    {
        public List<Product> CurrentBandProducts { get; set; }
    }

}