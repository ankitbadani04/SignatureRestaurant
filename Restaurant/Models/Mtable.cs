using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mtable
    {
        public int TableID { get; set; }
        public int TableNumber { get; set; }
        public int TableCapacity { get; set; }
        public string IsActive { get; set; }
    }
}