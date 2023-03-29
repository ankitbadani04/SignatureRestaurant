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
            SqlDataAdapter adp = new SqlDataAdapter("select itm.*,cat.CategoryName from Tbl_item as itm LEFT JOIN  Tbl_category as cat ON cat.CategoryID=itm.CategoryID", con);
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
            string Text = "";
            string Value = "";
            con.Open();

            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_category", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem {

                    Text = dr["CategoryName"].ToString(),
                    Value = dr["CategoryID"].ToString()
                });

            }
            ViewBag.CategoryList = new SelectList(list,"Value", "Text");

            return View();
        }

        // POST: Item/Create
        [HttpPost]
        public ActionResult Save(Mitem item)
        {
            try
            {
                con.Open();
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_item where ItemName = '" + item.ItemName + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                }
                else
                {
                    string filename = "";
                    // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                    string FileExtension = Path.GetExtension(item.ItemImage.FileName);
                    if (item.ItemImage.FileName != "" && item.ItemImage.FileName != null)
                    {
                        filename = "ITEM_" + DateTime.Now.ToString("yyyyMMddss") + "_" + FileExtension;
                    }

                    
                    string query = "insert into Tbl_item (CategoryID,ItemName,ItemDescription,ItemImage,ItemType,ItemSpicyLevel,ItemPrice)";
                    query += " values('" + item.CatID + "','" + item.ItemName + "','" + item.ItemDescription + "','" + filename + "','" + item.ItemType + "','" + item.ItemSpicylevel + "','" + item.ItemPrice + "')";

                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                        if (item.ItemImage.FileName != "" && item.ItemImage.FileName != null)
                        {
                            item.ItemImage.SaveAs(Server.MapPath("~/Upload/Items/" + filename));
                        }                      
                    }                    
                }
                return RedirectToAction("Create", "Item", new { @msg = msg });
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Item", new { @msg = msg });
            }
        }

        // GET: Item/Edit/5
        public ActionResult Edit(int id,string msg,Mitem item)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_item where ItemID=" + id;
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    item.ItemID = Convert.ToInt32(dr["ItemID"]);
                    item.CatID = Convert.ToInt32(dr["CategoryID"]);
                    item.ItemName = dr["ItemName"].ToString();
                    item.ItemDescription = dr["ItemDescription"].ToString();
                    item.ItemType = dr["ItemType"].ToString();
                    item.ItemSpicylevel = dr["ItemSpicylevel"].ToString();
                    item.ItemPrice = Convert.ToInt32(dr["ItemPrice"]);
                    item.oldimage = dr["ItemImage"].ToString();
                    if (dr["ItemImage"] != null)
                    {
                        ViewBag.image = dr["ItemImage"];
                    }
                    
                }
                dr.Close();
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_category", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                List<SelectListItem> list = new List<SelectListItem>();
                foreach (DataRow drs in dt.Rows)
                {
                    list.Add(new SelectListItem
                    {

                        Text = drs["CategoryName"].ToString(),
                        Value = drs["CategoryID"].ToString()
                    });

                }
                ViewBag.CategoryList = new SelectList(list, "Value", "Text");


                return View(item);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Item", new { id = id, @msg = msg });
            }
        }

        // POST: Item/Edit/5
        [HttpPost]
        public ActionResult Update(Mitem item)
        {
            try
            {
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_item where ItemName = '" + item.ItemName + "' and ItemID !='" + item.ItemID + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                    return RedirectToAction("Edit", "Item", new { id = item.ItemID, @msg = msg });
                }
                else
                {
                    string filename = "";
                    // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                    if (item.ItemImage != null)
                    {
                        string FileExtension = Path.GetExtension(item.ItemImage.FileName);
                        filename = "ITEM_" + DateTime.Now.ToString("yyyyMMddss") + "_" + FileExtension;
                    }
                    else
                    {
                        filename = item.oldimage;
                    }
                    string query = "update Tbl_item set CategoryID = '" + item.CatID + "',ItemName = '" + item.ItemName + "',ItemDescription = '" + item.ItemDescription + "',ItemType = '" + item.ItemType + "',ItemSpicylevel = '" + item.ItemSpicylevel + "',ItemImage = '" + filename + "',ItemPrice = '" + item.ItemPrice + "' where ItemID='" + item.ItemID + "'";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    if (item.ItemImage != null)
                    {
                        item.ItemImage.SaveAs(Server.MapPath("~/Upload/Items/" + filename));
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Item", new { id = item.ItemID, @msg = msg });
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

        public ActionResult ItemReport()
        {
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
            return View();
        }
        public class CategoryReport
        {
            public string catname { get; set; }
            public string totalitems { get; set; }
        }
    }
}
