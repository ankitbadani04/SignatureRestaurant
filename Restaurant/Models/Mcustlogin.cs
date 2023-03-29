using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mcustlogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}