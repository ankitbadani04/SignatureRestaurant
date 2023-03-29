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
using static Restaurant.Models.Mitem;

namespace Restaurant.Controllers
{
    public class ItemController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Item
        public ActionResult Index()
        {
            DataTable ItemList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_item", con);
            adp.Fill(ItemList);
            return View(ItemList);
        }

        // GET: Item/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Item/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        // POST: Item/Create
        [HttpPost]
        public ActionResult Save(Mitem item)
        {
            try
            {

                string filename = "";
                // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                string FileExtension = Path.GetExtension(item.ItemImage.FileName);

                if (item.ItemImage.FileName != "" && item.ItemImage.FileName != null)
                {
                    filename = "ITEM " + DateTime.Now.ToString("yyyyMMdd") + "_" + FileExtension;
                }

                con.Open();
                string query = "insert into Tbl_item (ItemName,ItemDescription,ItemImage,ItemType,ItemSpicyLevel,ItemPrice)";
                query += " values('" + item.ItemName + "','" + item.ItemDescription + "','" + filename + "','" + item.ItemType + "','" + item.ItemSpicylevel + "','" + item.ItemPrice + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    if (item.ItemImage.FileName != "" && item.ItemImage.FileName != null)
                    {
                        item.ItemImage.SaveAs(Server.MapPath("~/Upload/Items/" + filename));
                    }
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
                return RedirectToAction("Create", "Item", new { @msg = msg });
            }
        }

        // GET: Item/Edit/5
        public ActionResult Edit(int id,Mitem item)
        {
            try
            {
                con.Open();
                string query = "select * from Tbl_item where ItemID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    item.ItemID = Convert.ToInt32(dr["ItemID"]);
                    //item.CatID = Convert.ToInt32(dr["CategoryID"]);
                    item.ItemName = dr["ItemName"].ToString();
                    item.ItemDescription = dr["ItemDescription"].ToString();
                    item.ItemType = dr["ItemType"].ToString();
                    item.ItemSpicylevel = dr["ItemSpicylevel"].ToString();
                    item.ItemPrice = Convert.ToInt32(dr["ItemPrice"]);

                    if (dr["ItemImage"] != null)
                    {
                        ViewBag.image = dr["ItemImage"];
                    }
                    
                }

                return View(item);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Item", new { id = id, @msg = msg });
            }
        }

        // POST: Item/Edit/5
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

        // GET: Item/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Item/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_item where ItemID=" + did;
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
