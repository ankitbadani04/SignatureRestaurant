using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Restaurant.Models
{
    public class Morder
    {
        public int OrderID { get; set; }
        public int TableID { get; set; }
        public int NoPerson { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public float OrderDiscount { get; set; }
        public float OrderTotalBill { get; set; }
        public string CustContact { get; set; }
        public int UserID { get; set; }
        public string OrderType { get; set; }

        public DataTable Orderlist { get; set; }
        public DataTable Orderitemlist { get; set; }
        public DataTable orditem { get; set; }


    }
}