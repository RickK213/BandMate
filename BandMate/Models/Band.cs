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
        //[StringLength(17, ErrorMessage = "Name cannot be longer than 17 characters.")]
        public string Name { get; set; }

        public ICollection<BandMember> BandMembers { get; set; }
        public ICollection<Invitation> Invitations { get; set; }
        public ICollection<Tour> Tours { get; set; }
        public ICollection<Venue> Venues { get; set; }
        public ICollection<SetList> SetLists { get; set; }
        public ICollection<Song> Songs { get; set; }
        public ICollection<Event> Events { get; set; }

        public Store Store { get; set; }
        public int StoreId { get; set; }
    }
}