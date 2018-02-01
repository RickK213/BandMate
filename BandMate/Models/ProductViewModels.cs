using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandMate.Models
{
    public class ProductManageInventoryViewModel
    {
        public Product Product { get; set; }
        public List<Size> Sizes { get; set; }
    }
}