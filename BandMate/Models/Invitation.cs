using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Invitation
    {
        public int InvitationId { get; set; }

        [Required]
        public string Email { get; set; }

        public string InvitedBy { get; set; }

        public string BandName { get; set; }

        public string Title { get; set; }

        public int BandId { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool IsAccepted { get; set; }
    }
}