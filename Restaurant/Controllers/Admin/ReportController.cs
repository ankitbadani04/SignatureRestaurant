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
    public class ReportController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Report
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