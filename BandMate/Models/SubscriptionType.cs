﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class SubscriptionType
    {
        public int SubscriptionTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public double Price { get; set; }
    }
}