using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using System.Net.Mail;
using System.Text;

using System.Web.Security;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mlogin;

namespace Restaurant.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Login        
        public ActionResult Index(string msg)
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
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_user where username = '" + login.Username + "' and password = '" + login.Password + "' and UserType='admin'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["username"].ToString() == login.Username && dt.Rows[0]["password"].ToString() == login.Password)
                {
                   
                    Session["AdminId"] = dt.Rows[0]["UserID"].ToString();
                    Session["AdminName"] = dt.Rows[0]["Name"].ToString();       
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    msg = "NotFound";                    
                }                
            }
            else
            {
                msg = "Invalid";
               
            }
            return RedirectToAction("Index", new { msg = msg });

        }

        public ActionResult Logout()
        {
            if(Session["AdminName"]!=null)
            {
                Session.Remove("AdminName");
            }
            return RedirectToAction("Index","Login");
        }

        public ActionResult ForgotPassword(string msg)
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult SendOTP(string Emails)
        {

            string msg = "";
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_user where Email = '" + Emails + "'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int _min = 1000;
                int _max = 9999;

                Random _rdm = new Random();
                int OTP = _rdm.Next(_min, _max);
                Session["OTP"] = OTP;
                Session["emails"] = Emails;
                Session["Name"] = dt.Rows[0]["Name"].ToString();

                //===== send otp=====                

                var fromAddress = new MailAddress("signaturerestaurant007@gmail.com", "Signature Restaurant");
                var toAddress = new MailAddress(Emails, Session["Name"].ToString());
                string fromPassword = "Signature007@";
                string subject = "Password Reset OTP";
                string body = "Dear " + Session["Name"] + " , " + OTP + " is the One Time Password to reset password in to signature restaurant.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                //===== send otp=====


                return RedirectToAction("VerifyOTP", "Login");
                //, new { otp = OTP }

            }
            else
            {
                msg = "Invalid";
                return RedirectToAction("ForgotPassword", "Login", new { msg = msg });

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
                return RedirectToAction("NewPassword", "Login");

            }
            else
            {
                msg = "Invalid";
                return RedirectToAction("VerifyOTP", "Login", new { msg = msg });

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

                string query = "update Tbl_user set  Password= '" + ConfPass + "' where Contact = '" + Session["emails"] + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index", "Login");

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("NewPassword", "Login", new { @msg = msg });
            }
        }
    }
}