using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class InvitationIndexViewModel
    {
        public ICollection<Invitation> Invitations { get; set; }
        public ICollection<Band> InviteBands { get; set; }
    }
}