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


namespace Restaurant.Controllers.Admin
{
    public class FeedbackController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Feedback
        public ActionResult Index()
        {
            DataTable FeedbackList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select fed.*,cust.* from Tbl_feedback as fed LEFT JOIN Tbl_customer as cust ON cust.CustomerID=fed.CustomerID", con);
            adp.Fill(FeedbackList);
            return View(FeedbackList);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_feedback where FeedbackID=" + did;
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