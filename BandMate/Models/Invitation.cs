using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Invitation
    {
        public int InvitationId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsAccepted { get; set; }
    }
}