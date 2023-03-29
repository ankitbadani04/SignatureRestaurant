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
using static Restaurant.Models.Miteming;

namespace Restaurant.Controllers
{
    public class ItemIngController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: ItemIng
        public ActionResult Index()
        {
            DataTable ItemIngList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select ing.*,itm.ItemName,itmi.IngName from Tbl_iteming as ing LEFT JOIN  Tbl_item as itm ON itm.ItemID=ing.ItemID LEFT JOIN  Tbl_ingredients as itmi ON itmi.IngID=ing.IngID", con);
            adp.Fill(ItemIngList);
            return View(ItemIngList);
        }

        // GET: ItemIng/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ItemIng/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;

            string Text = "";
            string Value = "";
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_item", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem
                {

                    Text = dr["ItemName"].ToString(),
                    Value = dr["ItemID"].ToString()
                });

            }
            ViewBag.CategoryList = new SelectList(list, "Value", "Text");

            SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_ingredients", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            List<SelectListItem> list1 = new List<SelectListItem>();
            foreach (DataRow dr1 in dt1.Rows)
            {
                list1.Add(new SelectListItem
                {

                    Text = dr1["IngName"].ToString(),
                    Value = dr1["IngID"].ToString()
                });

            }
            ViewBag.IngList = new SelectList(list1, "Value", "Text");

            return View();
        }

        // POST: ItemIng/Create
        [HttpPost]
        public ActionResult Save(Miteming iteming)
        {
            try
            {
                con.Open();
                string msg = "";
                string query = "insert into Tbl_iteming (ItemID,IngID,ItemIngQty)";
                query += " values('" + iteming.ItemID + "','" + iteming.IngID + "','" + iteming.ItemIngQty + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                }
                return RedirectToAction("Create", "ItemIng", new { @msg = msg });
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "ItemIng", new { @msg = msg });
            }
        }

        // GET: ItemIng/Edit/5
        public ActionResult Edit(int id, string msg, Miteming iteming)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_iteming where ItemIngID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    iteming.ItemIngID = Convert.ToInt32(dr["ItemIngID"]);
                    iteming.ItemID = Convert.ToInt32(dr["ItemID"]);
                    iteming.IngID = Convert.ToInt32(dr["IngID"]);
                    iteming.ItemIngQty = Convert.ToInt32(dr["ItemIngQty"]);

                }
                dr.Close();

                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_item", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                List<SelectListItem> list = new List<SelectListItem>();
                foreach (DataRow drs in dt.Rows)
                {
                    list.Add(new SelectListItem
                    {

                        Text = drs["ItemName"].ToString(),
                        Value = drs["ItemID"].ToString()
                    });

                }
                ViewBag.CategoryList = new SelectList(list, "Value", "Text");

                SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_ingredients", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);

                List<SelectListItem> list1 = new List<SelectListItem>();
                foreach (DataRow dr1 in dt1.Rows)
                {
                    list1.Add(new SelectListItem
                    {

                        Text = dr1["IngName"].ToString(),
                        Value = dr1["IngID"].ToString()
                    });

                }
                ViewBag.IngList = new SelectList(list1, "Value", "Text");

                return View(iteming);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "ItemIng", new { id = id, @msg = msg });
            }
        }

        // POST: ItemIng/Edit/5
        [HttpPost]
        public ActionResult Update(Miteming iteming)
        {
            try
            {
                string query = "update Tbl_iteming set ItemID = '" + iteming.ItemID + "',IngID = '" + iteming.IngID + "',ItemIngQty = '" + iteming.ItemIngQty + "' where ItemIngID='" + iteming.ItemIngID + "' ";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "ItemIng", new { id = iteming.ItemIngID, @msg = msg });
            }
        }

        // GET: ItemIng/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ItemIng/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_iteming where ItemIngID=" + did;
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
    }
}
