using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mstock
    {
        public int StockID { get; set; }
        public int PartyID { get; set; }
        public int IngID { get; set; }
        public int StockQty { get; set; }
    }
}