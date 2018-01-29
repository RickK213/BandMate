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
        public int BandId { get; set; }
        public ICollection<SetListSong> SetListSongs { get; set; }
    }
}