using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mlogin;

namespace Restaurant.Controllers.kitchen
{
    public class KitchenLoginController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: KitchenLogin
        public ActionResult Index(string msg = "")
        {
            ViewBag.errormsg = msg;
            return View();
        }
        [HttpPost]
        public ActionResult Save(Mlogin login)
        {
            string username = "";
            string password = "";
            string msg = "";
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_user where username = '" + login.Username + "' and password = '" + login.Password + "' and UserType='Chef'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if(dt.Rows[0]["IsBlock"].ToString() == "No")
                { 
                    if (dt.Rows[0]["username"].ToString() == login.Username && dt.Rows[0]["password"].ToString() == login.Password)
                    {
                    
                        Session["KitchenId"] = dt.Rows[0]["UserID"].ToString();
                        Session["KitchenName"] = dt.Rows[0]["Name"].ToString();
                        return RedirectToAction("Index", "KitchenDashboard");
                    }
                    else
                    {
                        msg = "NotFound";                 
                    }
                }
                else
                {
                    msg = "UserBlock";
                }

            }
            else
            {
                msg = "Invalid";                
            }
            return RedirectToAction("Index", new { msg = msg });
        }


        public ActionResult ForgotPassword(string msg)
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult SendOTP(string Contact)
        {

            string msg = "";
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_user where Contact = '" + Contact + "'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int _min = 1000;
                int _max = 9999;
                Random _rdm = new Random();
                int OTP = _rdm.Next(_min, _max);
                Session["OTP"] = OTP;
                Session["Con"] = Contact;

                String message = HttpUtility.UrlEncode("This is a verification otp code for forget password. Enter the following Forget Password OTP " + OTP);
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



                return RedirectToAction("VerifyOTP", "KitchenLogin");
                //, new { otp = OTP }

            }
            else
            {
                msg = "Invalid";
                return RedirectToAction("ForgotPassword", "KitchenLogin", new { msg = msg });

            }

        }


        public ActionResult VerifyOTP(string msg)
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Verify(string VerifyOTP)
        {

            string msg = "";
            if (Session["OTP"].ToString() == VerifyOTP.ToString())
            {
                return RedirectToAction("NewPassword", "KitchenLogin");

            }
            else
            {
                msg = "Invalid";
                return RedirectToAction("VerifyOTP", "KitchenLogin", new { msg = msg });

            }
        }



        public ActionResult NewPassword(string msg)
        {
            ViewBag.errormsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult PassUpdate(string ConfPass)
        {
            try
            {

                string query = "update Tbl_user set  Password= '" + ConfPass + "' where Contact = '" + Session["Con"] + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index", "KitchenLogin");

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("NewPassword", "KitchenLogin", new { @msg = msg });
            }
        }

        public ActionResult Logout()
        {
            if (Session["KitchenName"] != null)
            {
                Session.Remove("KitchenName");
            }
            return RedirectToAction("Index", "KitchenLogin");
        }
    }
}