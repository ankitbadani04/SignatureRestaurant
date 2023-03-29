using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mcategory;

namespace Restaurant.Controllers
{
    public class CategoryController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Category
        public ActionResult Index()
        {
            DataTable CategoryList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_category", con);
            adp.Fill(CategoryList);
            return View(CategoryList);
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Category/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Save(Mcategory cate)
        {
            try
            {
                con.Open();
                string query = "insert into Tbl_category (CategoryName)";
                query += " values('" + cate.CategoryName + "')";

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
                return RedirectToAction("Create", "Category", new { @msg = msg });
            }
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int id,Mcategory Mcat)
        {
            try
            {
                con.Open();
                string query = "select * from Tbl_category where CategoryID="+id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Mcat.CategoryID = Convert.ToInt32(dr["CategoryID"]);
                    Mcat.CategoryName = dr["CategoryName"].ToString();
                    
                }
                
                return View(Mcat);
              
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Category", new { id=id, @msg = msg });
            }
           
        }

        // POST: Category/Edit/5
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

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Category/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_category where CategoryID="+ did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();
                
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }
    }
}
