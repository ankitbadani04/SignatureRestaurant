using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Restaurant.Models
{
    public class Viewmodel
    {
        public Morder orders { get; set; }
        public Morderitem orderitems { get; set; }
       
        public DataTable Orderlist { get; set; }
        public DataTable Orderitemlist { get; set; }
        public DataTable orditem { get; set; }
    }
}