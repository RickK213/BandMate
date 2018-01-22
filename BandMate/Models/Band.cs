using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Band
    {
        public int BandId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<ApplicationUser> BandMembers { get; set; }

        public ICollection<Invitation> Invitations { get; set; }
        public ICollection<Tour> Tours { get; set; }
        public ICollection<Venue> Venues { get; set; }
        public ICollection<SetList> SetLists { get; set; }
        public ICollection<Event> Events { get; set; }

        public Store Store { get; set; }
        public int StoreId { get; set; }
    }
}