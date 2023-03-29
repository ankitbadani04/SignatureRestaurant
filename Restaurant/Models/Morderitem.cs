using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Morderitem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}