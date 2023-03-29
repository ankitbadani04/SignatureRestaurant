using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Miteming
    {
        public int ItemIngID { get; set; }
        public int ItemID { get; set; }
        public int IngID { get; set; }
        public int ItemIngQty { get; set; }        
    }
}