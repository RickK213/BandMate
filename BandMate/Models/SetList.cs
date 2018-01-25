using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class SetList
    {
        public int SetListId { get; set; }
        public string Name { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}