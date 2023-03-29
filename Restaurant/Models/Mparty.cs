using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace Restaurant.Models
{
    public class Mparty
    {
        public int PartyID { get; set; }
        public string PartyName { get; set; }
        public string PartyContact { get; set; }
        public string PartyEmail { get; set; }
        public string PartyAddress { get; set; }
        public string PartyLandmark { get; set; }
        public int PartyPincode { get; set; }
    }
}