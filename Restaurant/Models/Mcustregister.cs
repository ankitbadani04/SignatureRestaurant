using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mcustregister
    {
        public int CustomerID { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Contact { get; set; }

        public string Email { get; set; }

        public string RegDate { get; set; }
    }
}