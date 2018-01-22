using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class NotificationPreference
    {
        public int NotificationPreferenceId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}