using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mreview
    {
        public int ReviewID { get; set; }
        public int OrderID { get; set; }
        public string Name { get; set; }

        public string Contact { get; set; }

        public int Rating { get; set; }
        public string Review { get; set; }

    }
}