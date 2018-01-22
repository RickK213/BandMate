using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Double TotalPrice { get; set; }
        public ICollection<Product> ProductsOrdered { get; set; }
    }
}