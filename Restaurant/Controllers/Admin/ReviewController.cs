using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mreview;
using Microsoft.Ajax.Utilities;

namespace Restaurant.Controllers.Client
{
    public class ReviewController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Review
        public ActionResult Index()
        {
            DataTable ReviewList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select rev.*,ord.OrderID from Tbl_review as rev LEFT JOIN Tbl_order as ord ON ord.OrderID=rev.OrderID", con);
            adp.Fill(ReviewList);
            return View(ReviewList);
        }

        public ActionResult Create(string id)
        {
         
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult Save(Mreview rev,int txthoid)
        { 
            try
            {
                con.Open();
                string msgs = "";
                string oid = Request.Form["txthoid"] ;
                string query = "insert into Tbl_review(OrderID,Name,Contact,Rating,Review)";
                query += " values('" + oid + "','" + rev.Name + "','" + rev.Contact + "','" + rev.Rating + "','" + rev.Review + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msgs = "Success";
                }
                return RedirectToAction("Index", "EmployeeDashboard", new { @msg = msgs });
            }
            catch (Exception ex)
            {
                string msgs = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Review", new { @msg = msgs });
            }
        }

        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_review where ReviewID=" + did;
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