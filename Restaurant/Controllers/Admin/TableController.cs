using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mtable;
using System.Net;
using System.Collections.Specialized;

namespace Restaurant.Controllers
{
    public class TableController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Table
        public ActionResult Index()
        {
            DataTable TableList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table", con);
            adp.Fill(TableList);
            return View(TableList);
        }

        // GET: Table/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Table/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        // POST: Table/Create
        [HttpPost]
        public ActionResult Save(Mtable table)
        {
            try
            {
                con.Open();
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table where TableNumber = '" + table.TableNumber + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                }
                else
                {
                    string query = "insert into Tbl_table (TableNumber,TableCapacity,IsActive)";
                    query += " values('" + table.TableNumber + "','" + table.TableCapacity + "','" + table.IsActive + "')";

                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                    }                   
                }
                return RedirectToAction("Create", "Table", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Table", new { @msg = msg });
            }
        }

        // GET: Table/Edit/5
        public ActionResult Edit(int id, string msg,Mtable table)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_table where tableID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    table.TableID = Convert.ToInt32(dr["TableID"]);
                    table.TableNumber = Convert.ToInt32(dr["TableNumber"]);
                    table.TableCapacity = Convert.ToInt32(dr["TableCapacity"]);
                    table.IsActive = dr["IsActive"].ToString();

                }

                return View(table);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Table", new { id = id, @msg = msg });
            }
        }

        // POST: Table/Edit/5
        [HttpPost]
        public ActionResult Update(Mtable table)
        {
            try
            {
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table where TableNumber = '" + table.TableNumber + "' and TableID !='" + table.TableID + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                    return RedirectToAction("Edit", "Table", new { id = table.TableID, @msg = msg });
                }
                else
                {
                    string query = "update Tbl_table set TableNumber = '" + table.TableNumber + "',TableCapacity = '" + table.TableCapacity + "',IsActive = '" + table.IsActive + "' where TableID='" + table.TableID + "'";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Category", new { id = table.TableID, @msg = msg });
            }
        }

        public ActionResult tablestatus(int id,string status)
        {
            con.Open();
            string query = "update Tbl_table set IsActive='"+ status +"' where TableID='"+id+"'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index","Table");
        }

        // GET: Table/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Table/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_table where TableID=" + did;
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

        public ActionResult TableReport()
        {
            con.Open();
            List<TableOrdReport> tables = new List<TableOrdReport>();

            string query = "SELECT *,(select count(*) from Tbl_order where TableID=Tbl_table.TableID) as totalitems FROM Tbl_table";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                TableOrdReport obj = new TableOrdReport();
                obj.tblno = rdr["TableNumber"].ToString();
                obj.totalorder = rdr["totalitems"].ToString();

                tables.Add(obj);
            }

            con.Close();
            ViewBag.tables = tables;
            return View();
        }
        public class TableOrdReport
        {
            public string tblno { get; set; }
            public string totalorder { get; set; }
        }

        public ActionResult BookTable()
        {
            DataTable Booking = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_booktable", con);
            adp.Fill(Booking);
            return View(Booking);
        }


        public ActionResult confirmtbl(int id)
        {
            con.Open();
            string query = "update Tbl_booktable set Status='Confirm' where BookTableID='" + id + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();

            string Contact = "";
            string Date = "";
            string Guest = "";
            string Time = "";


            string query1 = "select * from Tbl_booktable where BookTableID  = '" + id + "'";
            SqlCommand cmd1 = new SqlCommand(query1, con);
            cmd1.CommandType = CommandType.Text;

            SqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {

                Contact = rdr["Contact"].ToString();
                Date = rdr["Date"].ToString();
                Time = rdr["Time"].ToString();
                Guest = rdr["Guest"].ToString();

                String message = HttpUtility.UrlEncode("Your reservation at Signature Restaurant on the " + Convert.ToDateTime(Date).ToString("dd/MM/yyyy") + " at " + Time + " for " + Guest + " people is now CONFIRMED, HAVE A GREAT TIME!");
                using (var wb = new WebClient())
                {
                    byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                {
                {"apikey" , "WSlDc2oonEM-8ZrHQVU8R0Bwl9cnaCWl48uiD9Wud4"},
                {"numbers" , Contact},
                {"message" , message},
                {"sender" , "TXTLCL"}
                });
                    string result = System.Text.Encoding.UTF8.GetString(response);

                }

            }
            return RedirectToAction("BookTable", "Table");
        }

        [HttpPost]
        public ActionResult BookTableDelete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_booktable where BookTableID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                return RedirectToAction("BookTable");
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
