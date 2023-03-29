using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mfeedback
    {
        public int FeedbackID { get; set; }
        public int CustomerID { get; set; }        
        public string Feedback { get; set; }
    }
}