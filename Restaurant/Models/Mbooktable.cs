using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant.Models
{
    public class Mbooktable
    {
        public int BookTableId { get; set; }

        public string Name { get; set; }

        public int Guests { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string Contact { get; set; }

        public string Status { get; set; }
       
    }
}