using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Morder;

namespace Restaurant.Controllers.kitchen
{
    public class KitchenDashboardController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: KitchenDashboard
        public ActionResult Index()
        {
            Morder mord = new Morder();

            DataTable OrderList1 = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ord.*,tbl.TableNumber from Tbl_order as ord LEFT JOIN Tbl_table as tbl ON tbl.TableID=ord.TableID where ord.OrderStatus='New' or ord.OrderStatus='Preparing' or ord.OrderStatus='Delivered'", con);
            adp.Fill(OrderList1);
            mord.Orderlist = OrderList1;
            
            DataTable OrderList2 = new DataTable();
            SqlDataAdapter adp2 = new SqlDataAdapter("select ordi.*,ord.OrderID,ord.OrderStatus,ord.TableID,item.ItemName from Tbl_orderitem as ordi LEFT JOIN Tbl_order as ord ON ord.OrderID=ordi.OrderID LEFT JOIN Tbl_item as item ON item.ItemID=ordi.ItemID where ord.OrderStatus='New' or ord.OrderStatus='Preparing' or ord.OrderStatus='Delivered' order by OrderItemID desc", con);
            adp2.Fill(OrderList2);
            mord.Orderitemlist = OrderList2;

            return View(mord);
        }
        
        public ActionResult OStatusPre(int id)
        {
            con.Open();
            SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_order where OrderID='" + id + "' and OrderStatus='New' or OrderStatus='Preparing' or OrderStatus='Delivered'", con);
            DataTable dts = new DataTable();
            adps.Fill(dts);
            if (dts.Rows.Count > 0)
            {
                string query1 = "update Tbl_order set OrderStatus = 'Preparing' where OrderID = '" + id + "'";
                SqlCommand cmds = new SqlCommand(query1, con);
                cmds.ExecuteNonQuery();

                SqlDataAdapter adpi = new SqlDataAdapter("select * from Tbl_orderitem where OrderID='" + id + "' and OItemStatus='New'", con);
                DataTable dtsi = new DataTable();
                adpi.Fill(dtsi);
                if (dtsi.Rows.Count > 0)
                {
                    string query2 = "update Tbl_orderitem set OitemStatus='Preparing' where OrderID = '" + id + "' and OItemStatus='New'";
                    SqlCommand cmd2 = new SqlCommand(query2, con);
                    cmd2.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "kitchenDashboard");
        }

        public ActionResult OStatusdeli(int id)
        {
            con.Open();
           
            SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_order where OrderID='" + id + "' and OrderStatus='Preparing'", con);
            DataTable dts = new DataTable();
            adps.Fill(dts);
            if (dts.Rows.Count > 0)
            {
                string query = "update Tbl_order set OrderStatus='Delivered' where OrderID = '" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            
            return RedirectToAction("Index", "kitchenDashboard");
        }

        public ActionResult changestatus(int id)
        {

            con.Open();
            SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_orderitem where OrderItemID = '" + id + "' and OitemStatus='Preparing'", con);
            DataTable dts = new DataTable();
            adps.Fill(dts);
            if (dts.Rows.Count > 0)
            {
                string query = "update Tbl_orderitem set OItemStatus='Complete' where OrderItemID = '" + id + "' and OitemStatus='Preparing'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "kitchenDashboard");
        }

    }
}