using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant.Models
{
    public class Mlog
    {
        public int LogID { get; set; }
        public int OrderID { get; set; }
        public string LogStatus { get; set; }
        public string LogDate { get; set; }
    }
}