using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mingredients
    {
        public int IngID { get; set; }
        public string IngName { get; set; }
        public HttpPostedFileBase IngImage { get; set; }
        public string oldimage { get; set; }
    }
}