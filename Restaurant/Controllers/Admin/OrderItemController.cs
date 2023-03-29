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
using static Restaurant.Models.Morderitem;

namespace Restaurant.Controllers.Client
{
    public class OrderItemController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: OrderItem
        public ActionResult Index(int id)
        {
            DataTable OrderList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ordi.*,ord.OrderID,item.ItemName from Tbl_orderitem as ordi LEFT JOIN Tbl_order as ord ON ord.OrderID=ordi.OrderID LEFT JOIN Tbl_item as item ON item.ItemID=ordi.ItemID where ord.OrderID =" +id , con);
            adp.Fill(OrderList);
            return View(OrderList);
        }


        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;

            string Text = "";
            string Value = "";
            con.Open();

            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_order", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem
                {

                    Text = dr["TableID"].ToString(),
                    Value = dr["OrderID"].ToString()
                });

            }
            ViewBag.OrderList = new SelectList(list, "Value", "Text");

            SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_item", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            List<SelectListItem> list1 = new List<SelectListItem>();
            foreach (DataRow dr1 in dt1.Rows)
            {
                list1.Add(new SelectListItem
                {

                    Text = dr1["ItemName"].ToString(),
                    Value = dr1["ItemID"].ToString()
                });

            }
            ViewBag.ItemList = new SelectList(list1, "Value", "Text");

            return View();
        }

        public ActionResult Save(Morderitem orderi)
        {
            try
            {
                con.Open();
                string msg = "";
                string query = "insert into Tbl_orderitem (OrderID,ItemID,OItemQty,OItemPrice,OItemStatus)";
                query += " values('" + orderi.OrderID + "','" + orderi.ItemID + "','" + orderi.Qty + "','" + orderi.Price + "','" + orderi.Status + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                }
                return RedirectToAction("Create", "OrderItem", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "OrderItem", new { @msg = msg });
            }
        }

        public ActionResult Edit(int id, string msg, Morderitem orderi)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_orderitem where OrderItemID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    orderi.OrderItemID = Convert.ToInt32(dr["OrderItemID"]);
                    orderi.OrderID = Convert.ToInt32(dr["OrderID"]);
                    orderi.ItemID = Convert.ToInt32(dr["ItemID"]);
                    orderi.Qty = Convert.ToInt32(dr["OItemQty"]);
                    orderi.Price = Convert.ToInt32(dr["OItemPrice"]);
                    orderi.Status = dr["OItemStatus"].ToString();
                }
                dr.Close();

                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_order", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                List<SelectListItem> list = new List<SelectListItem>();
                foreach (DataRow drs in dt.Rows)
                {
                    list.Add(new SelectListItem
                    {

                        Text = drs["TableID"].ToString(),
                        Value = drs["OrderID"].ToString()
                    });

                }
                ViewBag.OrderList = new SelectList(list, "Value", "Text");

                SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_item", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);

                List<SelectListItem> list1 = new List<SelectListItem>();
                foreach (DataRow dr1 in dt1.Rows)
                {
                    list1.Add(new SelectListItem
                    {

                        Text = dr1["ItemName"].ToString(),
                        Value = dr1["ItemID"].ToString()
                    });

                }
                ViewBag.ItemList = new SelectList(list1, "Value", "Text");

                return View(orderi);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "OrderItem", new { id = id, @msg = msg });
            }

        }


        public ActionResult Update(Morderitem orderi)
        {
            try
            {
                string query = "update Tbl_orderitem set OrderID = '" + orderi.OrderID + "',ItemID='" + orderi.ItemID + "',OItemQty='" + orderi.Qty + "',OItemPrice='" + orderi.Price + "',OItemStatus='" + orderi.Status + "' where OrderItemID = '" + orderi.OrderItemID + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index", "OrderItem", new { id = orderi.OrderID});
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "OrderItem", new { id = orderi.OrderItemID, @msg = msg });
            }


        }


        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string dids = collection["txthfdoid"];
                string query = "delete from Tbl_orderitem where OrderItemID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                return RedirectToAction("Index", new { id = dids });
            }
            catch (Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }
    }
}