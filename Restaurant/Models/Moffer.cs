using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Moffer
    {
        public int OfferID { get; set; }
        public string Name { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string oldimage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalAmt { get; set; }
        public int Discount { get; set; }
        public string IsActive { get; set; }
    }
}