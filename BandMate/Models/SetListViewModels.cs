using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class SetListEditViewModel
    {
        public ICollection<Song> Songs { get; set; }
        public SetList SetList { get; set; }
    }
}