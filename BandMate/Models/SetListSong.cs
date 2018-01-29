using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class SetListSong
    {
        public int SetListSongId { get; set; }
        public int SetListOrder { get; set; }

        public Song Song { get; set; }
        public int SongId { get; set; }
    }
}