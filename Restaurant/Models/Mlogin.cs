using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mlogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}