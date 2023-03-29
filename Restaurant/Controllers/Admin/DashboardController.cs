using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;

namespace Restaurant.Controllers.Admin
{
    public class DashboardController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Dashboard
        public ActionResult Index()
        {
            DataTable StatusListn = new DataTable();
            SqlDataAdapter adppn = new SqlDataAdapter("select OrderStatus from Tbl_order where OrderStatus='New'", con);
            adppn.Fill(StatusListn);
            ViewBag.statuslistn = StatusListn.Rows.Count;

            DataTable StatusListp = new DataTable();
            SqlDataAdapter adppe = new SqlDataAdapter("select OrderStatus from Tbl_order where OrderStatus='Preparing'", con);
            adppe.Fill(StatusListp);
            ViewBag.statuslistp = StatusListp.Rows.Count;

            DataTable StatusList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select OrderStatus from Tbl_order where OrderStatus='Complete'", con);
            adp.Fill(StatusList);
            ViewBag.statuslist = StatusList.Rows.Count;

            DataTable OrderList = new DataTable();
            SqlDataAdapter adpc = new SqlDataAdapter("select * from Tbl_order", con);
            adpc.Fill(OrderList);
            ViewBag.totalorders = OrderList.Rows.Count;

            DataTable Itemlist = new DataTable();
            SqlDataAdapter adp2 = new SqlDataAdapter("select * from Tbl_item", con);
            adp2.Fill(Itemlist);
            ViewBag.totalitem = Itemlist.Rows.Count;


            DataTable Inglist = new DataTable();
            SqlDataAdapter adp3 = new SqlDataAdapter("select * from Tbl_ingredients", con);
            adp3.Fill(Inglist);
            ViewBag.totalIng = Inglist.Rows.Count;

            string totalamount = "";
            SqlDataAdapter adpq = new SqlDataAdapter("select SUM(OrderTotalBill) as totalamount from Tbl_order", con);
            DataTable total = new DataTable();
            adpq.Fill(total);
            if (total.Rows.Count > 0)
            {
                foreach (DataRow dr in total.Rows)
                {
                    totalamount = dr["totalamount"].ToString();
                }
            }
            ViewBag.total = totalamount;

            string amounts = "";
            SqlDataAdapter adpa = new SqlDataAdapter("select CAST(OrderDate AS DATE),SUM(OrderTotalBill) as todayamt from Tbl_order group by CAST(OrderDate AS DATE)", con);
            DataTable totals = new DataTable();
            adpa.Fill(totals);
            if (totals.Rows.Count > 0)
            {
                foreach (DataRow dr in totals.Rows)
                {
                    amounts = dr["todayamt"].ToString();
                }
            }
            ViewBag.totaldate = amounts;
           
            con.Open();
            List<CategoryReport> items = new List<CategoryReport>();
            string query = "SELECT *,(select count(*) from Tbl_item where CategoryID=Tbl_category.CategoryID) as totalitems FROM Tbl_category";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                CategoryReport obj = new CategoryReport();
                obj.catname = rdr["CategoryName"].ToString();
                obj.totalitems = rdr["totalitems"].ToString();

                items.Add(obj);
            }
            con.Close();
            ViewBag.items = items;


            con.Open();
            List<IngredientsReport> ingitems = new List<IngredientsReport>();
            string querys = "SELECT *,(select ISNULL(sum(StockQty),0) from Tbl_stock where IngID=Tbl_Ingredients.IngID) as totalstock FROM Tbl_Ingredients";
            SqlCommand cmds = new SqlCommand(querys, con);
            cmds.CommandType = CommandType.Text;

            SqlDataReader rdrs = cmds.ExecuteReader();
            while (rdrs.Read())
            {
                IngredientsReport objs = new IngredientsReport();
                objs.ingname = rdrs["IngName"].ToString();
                objs.totalstock = rdrs["totalstock"].ToString();

                ingitems.Add(objs);
            }

            con.Close();
            ViewBag.ingitems = ingitems;

            DataTable OrderList1 = new DataTable();
            SqlDataAdapter adp1 = new SqlDataAdapter("select TOP 6 ord.*,tbl.TableNumber,reg.Name from Tbl_order as ord LEFT JOIN Tbl_table as tbl ON tbl.TableID=ord.TableID LEFT JOIN Tbl_user as reg ON reg.UserID=ord.UserID ORDER BY ord.OrderID DESC", con);
            adp1.Fill(OrderList1);

            return View(OrderList1);
        }

        public class IngredientsReport
        {
            public string ingname { get; set; }
            public string totalstock { get; set; }
        }

        public class CategoryReport
        {
            public string catname { get; set; }
            public string totalitems { get; set; }
        }


    }
}