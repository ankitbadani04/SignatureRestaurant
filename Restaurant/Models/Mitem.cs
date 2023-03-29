using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mitem
    {
        public int ItemID { get; set; }
        public int CatID { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public HttpPostedFileBase ItemImage { get; set; }
        public string oldimage { get; set; }
        public string ItemType { get; set; }
        public string ItemSpicylevel { get; set; }
        public decimal ItemPrice { get; set; }
    }
}