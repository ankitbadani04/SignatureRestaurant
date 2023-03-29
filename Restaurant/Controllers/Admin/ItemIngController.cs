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
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_iteming", con);
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
            return View();
        }

        // POST: ItemIng/Create
        [HttpPost]
        public ActionResult Save(Miteming iteming)
        {
            try
            {
                con.Open();
                string query = "insert into Tbl_iteming (ItemIngQty)";
                query += " values('" + iteming.ItemIngQty + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    return RedirectToAction("Create");
                }
                else
                {
                    return RedirectToAction("Create");
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "ItemIng", new { @msg = msg });
            }
        }

        // GET: ItemIng/Edit/5
        public ActionResult Edit(int id,Miteming iteming)
        {
            try
            {
                con.Open();
                string query = "select * from Tbl_iteming where ItemIngID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    iteming.ItemIngID = Convert.ToInt32(dr["ItemIngID"]);
                    iteming.ItemIngQty = Convert.ToInt32(dr["ItemIngQty"]);

                }

                return View(iteming);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "ItemIng", new { id = id, @msg = msg });
            }
        }

        // POST: ItemIng/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
