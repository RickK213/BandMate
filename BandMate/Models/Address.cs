﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        //[Display(Name = "streetOne")]
        [Required]
        public string StreetOne { get; set; }

        //[Display(Name = "city")]
        public City City { get; set; }
        public int CityId { get; set; }

        public State State { get; set; }
        public int StateId { get; set; }

        public ZipCode ZipCode { get; set; }
        public int ZipCodeId { get; set; }

        public float? Lat { get; set; }
        public float? Lng { get; set; }
    }
}