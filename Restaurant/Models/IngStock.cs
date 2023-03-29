using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class IngStock
    {
        public string ingname { get; set; }
        public string totalstock { get; set; }
        public string usestock { get; set; }
        public string avastock { get; set; }
    }
}