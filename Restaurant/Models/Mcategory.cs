using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mcategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}