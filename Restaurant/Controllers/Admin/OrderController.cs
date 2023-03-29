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

namespace Restaurant.Controllers.Client
{
    public class OrderController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Order
        public ActionResult Index()
        {
            DataTable OrderList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ord.*,tbl.TableNumber,reg.Name from Tbl_order as ord LEFT JOIN Tbl_table as tbl ON tbl.TableID=ord.TableID LEFT JOIN Tbl_user as reg ON reg.UserID=ord.UserID", con);
            adp.Fill(OrderList);
           
            return View(OrderList);
        }

        public ActionResult Preparingorder()
        {
            DataTable OrderList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ord.*,tbl.TableNumber from Tbl_order as ord LEFT JOIN Tbl_table as tbl ON tbl.TableID=ord.TableID where OrderStatus='Preparing' ", con);
            adp.Fill(OrderList);
            return View(OrderList);
        }

        public ActionResult Completeorder()
        {
            DataTable OrderList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ord.*,tbl.TableNumber from Tbl_order as ord LEFT JOIN Tbl_table as tbl ON tbl.TableID=ord.TableID where OrderStatus='Complete' ", con);
            adp.Fill(OrderList);
            return View(OrderList);
        }

        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;

            string Text = "";
            string Value = "";
            con.Open();

            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem
                {

                    Text = dr["TableNumber"].ToString(),
                    Value = dr["TableID"].ToString()
                });

            }
            ViewBag.TableList = new SelectList(list, "Value", "Text");

            SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_user", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            List<SelectListItem> list1 = new List<SelectListItem>();
            foreach (DataRow dr1 in dt1.Rows)
            {
                list1.Add(new SelectListItem
                {

                    Text = dr1["Name"].ToString(),
                    Value = dr1["UserID"].ToString()
                });

            }
            ViewBag.RegList = new SelectList(list1, "Value", "Text");

            return View();
        }

        public ActionResult Save(Morder order)
        {
            try
            {
                con.Open();
                string msg = "";
                string query = "insert into Tbl_order (TableID,NoPerson,OrderDate,OrderStatus,OrderDiscount,OrderTotalBill,CustContact,UserID,OrderType)";
                query += " values('" + order.TableID + "','" + order.NoPerson + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "','" + order.OrderStatus + "','" + order.OrderDiscount + "','" + order.OrderTotalBill + "','" + order.CustContact + "','" + order.UserID + "','" + order.OrderType + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                }
                return RedirectToAction("Create", "Order", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Order", new { @msg = msg });
            }
        }

        public ActionResult Edit(int id, string msg, Morder order)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_order where OrderID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    order.OrderID = Convert.ToInt32(dr["OrderID"]);
                    order.TableID = Convert.ToInt32(dr["TableID"]);
                    order.NoPerson = Convert.ToInt32(dr["NoPerson"]);
                    order.OrderDate = dr["OrderDate"].ToString();
                    order.OrderStatus = dr["OrderStatus"].ToString();
                    order.OrderDiscount = Convert.ToInt32(dr["OrderDiscount"]);
                    order.OrderTotalBill = Convert.ToInt32(dr["OrderTotalBill"]);
                    order.CustContact = dr["CustContact"].ToString();
                    order.UserID = Convert.ToInt32(dr["UserID"]);
                    order.OrderType = dr["OrderType"].ToString();
                }
                dr.Close();

                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                List<SelectListItem> list = new List<SelectListItem>();
                foreach (DataRow drs in dt.Rows)
                {
                    list.Add(new SelectListItem
                    {

                        Text = drs["TableNumber"].ToString(),
                        Value = drs["TableID"].ToString()
                    });

                }
                ViewBag.TableList = new SelectList(list, "Value", "Text");

                SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_user", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);

                List<SelectListItem> list1 = new List<SelectListItem>();
                foreach (DataRow dr1 in dt1.Rows)
                {
                    list1.Add(new SelectListItem
                    {

                        Text = dr1["Name"].ToString(),
                        Value = dr1["UserID"].ToString()
                    });

                }
                ViewBag.RegList = new SelectList(list1, "Value", "Text");

                return View(order);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Order", new { id = id, @msg = msg });
            }

        }

        public ActionResult Update(Morder order)
        {
            try
            {
                string query = "update Tbl_order set TableID = '" + order.TableID + "',NoPerson='" + order.NoPerson + "',OrderDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "',OrderStatus='" + order.OrderStatus + "',OrderDiscount='" + order.OrderDiscount + "',OrderTotalBill='" + order.OrderTotalBill + "',CustContact='" + order.CustContact + "',UserID='" + order.UserID + "',OrderType='" + order.OrderType + "' where OrderID = '" + order.OrderID + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Order", new { id = order.OrderID, @msg = msg });
            }


        }

        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_order where OrderID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }

        public ActionResult Status(Morder order, int id)
        {
            try
            {
                con.Open();
                string query = "select * from Tbl_order where OrderID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    order.OrderID = Convert.ToInt32(dr["OrderID"]);

                    order.OrderStatus = dr["OrderStatus"].ToString();

                }
                return View(order);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Status", "Order", new { id = id, @msg = msg });
            }
        }


        public ActionResult updatestatus(Morder order)
        {
            try
            {
                string query = "update Tbl_order set OrderStatus='" + order.OrderStatus + "' where OrderID = '" + order.OrderID + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Index", "Order", new { id = order.OrderID, @msg = msg });
            }

        }
        
        public ActionResult Iteminfo(int id)
        {


            DataTable ItemList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ordi.*,itm.ItemName from Tbl_orderitem as ordi LEFT JOIN Tbl_item as itm ON itm.ItemID=ordi.ItemID where OrderID=" + id, con);
            adp.Fill(ItemList);
            return View(ItemList);


        }


        public ActionResult Invoice(int id, int disc)
        {
            string subtotal = "0";
            decimal dicamt, grandtotal;
            SqlDataAdapter adpq = new SqlDataAdapter("select SUM(OItemQty * OItemPrice) as Subtotal from Tbl_orderitem where OrderID ='" + id + "' GROUP BY OrderID", con);
            DataTable total = new DataTable();
            adpq.Fill(total);
            if (total.Rows.Count > 0)
            {
                foreach (DataRow dr in total.Rows)
                {
                    subtotal = dr["Subtotal"].ToString();

                }
            }


            ViewBag.subtotal = subtotal;

            ViewBag.discount = disc;

            dicamt = Convert.ToDecimal(subtotal) * Convert.ToDecimal(disc) / 100;
            ViewBag.dictotal = dicamt;

            grandtotal = Convert.ToDecimal(subtotal) - Convert.ToDecimal(dicamt);
            ViewBag.grandtotal = grandtotal;

            DataTable OrderList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ordi.*,itm.ItemName,(ordi.OItemQty * ordi.OItemPrice) as total from Tbl_orderitem as ordi LEFT JOIN Tbl_item as itm ON itm.ItemID=ordi.ItemID where OrderID ='" + id + "' and OitemStatus='Complete'  ", con);
            adp.Fill(OrderList);
            return View(OrderList);
        }
    }
}