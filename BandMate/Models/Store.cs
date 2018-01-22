using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Store
    {
        public int StoreId { get; set; }
        public ICollection<Product> Products { get; set; }
        public string PlaylistId { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}