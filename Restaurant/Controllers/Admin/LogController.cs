using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mlog;


namespace Restaurant.Controllers.Admin
{
   
    public class LogController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Log
        public ActionResult Index()
        {
            DataTable LogList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select log.*,ord.OrderID from Tbl_log as log LEFT JOIN Tbl_order as ord ON ord.OrderID=log.OrderID", con);
            adp.Fill(LogList);
            return View(LogList);
        }

        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_log where LogID=" + did;
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