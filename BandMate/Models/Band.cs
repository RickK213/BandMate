﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Band
    {
        public int BandId { get; set; }
        public string Name { get; set; }

        public Subscription Subscription { get; set; }
        public int SubscriptionId { get; set; }

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