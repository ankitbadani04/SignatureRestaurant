using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mcustregister;

using System.Net;
using System.Net.Mail;
using System.Text;

namespace Restaurant.Controllers.Customer
{
    public class SignatureRestaurantController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: SignatureRestaurant
        public ActionResult Home()
        {
            DataTable MenuList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select TOP 10 * from Tbl_item", con);
            adp.Fill(MenuList);

            DataTable OfferList = new DataTable();
            SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_offer", con);
            adps.Fill(OfferList);
            ViewBag.Collection = OfferList;

            DataTable review = new DataTable();
            SqlDataAdapter adp2 = new SqlDataAdapter("select TOP 9 rev.*,ord.OrderID from Tbl_review as rev LEFT JOIN Tbl_order as ord ON ord.OrderID=rev.OrderID order by rev.ReviewID DESC ", con);
            adp2.Fill(review);
            ViewBag.Collectionrev = review;

            return View(MenuList);
        }

        //public ActionResult Menu()
        //{
        //    DataTable categorylist = new DataTable();
        //    SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_category", con);
        //    adps.Fill(categorylist);
        //    ViewBag.Collectioncat = categorylist;

        //    DataTable MenuList = new DataTable();
        //    SqlDataAdapter adp = new SqlDataAdapter("select cat.CategoryName,itm.* from Tbl_item as itm LEFT JOIN Tbl_category as cat ON cat.CategoryID=itm.CategoryID", con);
        //    adp.Fill(MenuList);
        //    return View(MenuList);
        //}

        public ActionResult Menu(int? id)
        {
            DataTable categorylist = new DataTable();
            SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_category", con);
            adps.Fill(categorylist);
            ViewBag.Collectioncat = categorylist;

            if (!id.HasValue && categorylist.Rows.Count > 0)
            {
                id = Convert.ToInt32(categorylist.Rows[0]["CategoryID"]);
            }
            id = id ?? 0;
            DataTable MenuList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select cat.CategoryName,itm.* from Tbl_item as itm LEFT JOIN Tbl_category as cat ON cat.CategoryID=itm.CategoryID where itm.CategoryID ='" + id + "'", con);
            adp.Fill(MenuList);
            return View(MenuList);
        }

        public ActionResult MoreInfo(int id)
        {
            DataTable MenuList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_item where ItemID ='" + id + "' ", con);
            adp.Fill(MenuList);

            DataTable Iteamist = new DataTable();
            SqlDataAdapter adp1 = new SqlDataAdapter("select ing.*,ingr.IngName,itm.ItemName from Tbl_iteming as ing LEFT JOIN  Tbl_item as itm ON itm.ItemID=ing.ItemID LEFT JOIN Tbl_ingredients as ingr ON ingr.IngID=ing.IngID where itm.ItemID='" + id + "'", con);
            adp1.Fill(Iteamist);
            ViewBag.Ing = Iteamist;

            return View(MenuList);
        }

        public ActionResult BookATable(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        public ActionResult SaveTable(Mbooktable reg)
        {
            try
            {
                con.Open();
                string msg = "";
                DateTime time = Convert.ToDateTime(reg.Time);
                reg.Time = time.ToString("hh:mm tt");


                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_booktable where Contact = '" + reg.Contact + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                }
                else
                {
                    string query = "insert into Tbl_booktable(Name,Guest,Date,Time,Contact,Status)";
                    query += " values('" + reg.Name + "','" + reg.Guests + "','" + reg.Date + "','" + reg.Time + "','" + reg.Contact + "','Pending')";

                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                    }
                }
                return RedirectToAction("BookATable", "SignatureRestaurant", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("BookATable", "SignatureRestaurant", new { @msg = msg });
            }

        }

        public ActionResult Review(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Savereview(Mreview rev)
        {
            try
            {
                con.Open();
                string msg = "";

                string query = "insert into Tbl_review(Rating,Review,CustomerID)";
                query += " values('" + rev.Rating + "','" + rev.Review + "','" + Session["CustomerID"] + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                }

                return RedirectToAction("Review", "SignatureRestaurant", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Review", "SignatureRestaurant", new { @msg = msg });
            }
        }

        public ActionResult Feedback(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Savefeedback(Mfeedback reg)
        {
            try
            {
                con.Open();
                string msg = "";

                string query = "insert into Tbl_feedback(Feedback,CustomerID)";
                query += " values('" + reg.Feedback + "','" + Session["CustomerID"] + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                }

                return RedirectToAction("Feedback", "SignatureRestaurant", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Feedback", "SignatureRestaurant", new { @msg = msg });
            }

        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult MyProfile(int id, string msg,string msgpass, Mcustregister custd)
        {
            ViewBag.errormsg = msg;
            ViewBag.errormsgpass = msgpass;
            try
            {
                con.Open();
                string query = "select * from Tbl_customer where CustomerID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    custd.CustomerID = Convert.ToInt32(dr["CustomerID"]);
                    custd.Name = dr["Name"].ToString();                    
                    custd.Contact = dr["Contact"].ToString();
                    custd.Email = dr["Email"].ToString();                   

                }
                return View(custd);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("MyProfile", "SignatureRestaurant", new { id = id, @msg = msg });
            }
        }

        [HttpPost]
        public ActionResult UpdateProfile(Mcustregister custd)
        {
            try
            {
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_customer where Contact = '" + custd.Contact + "' and Email = '"+ custd.Email +"' and CustomerID !='" + custd.CustomerID + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                    return RedirectToAction("MyProfile", "SignatureRestaurant", new { id = custd.CustomerID, @msg = msg });
                }
                else
                {
                    string query = "update Tbl_customer set Name = '" + custd.Name + "',Contact = '" + custd.Contact + "',Email = '" + custd.Email + "' where CustomerID='" + custd.CustomerID + "'";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.ExecuteNonQuery();
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                    }
                    return RedirectToAction("MyProfile", "SignatureRestaurant", new { id = custd.CustomerID, @msg });
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("MyProfile", "SignatureRestaurant", new { id = custd.CustomerID, @msg = msg });
            }
        }

       
        public ActionResult ChangePassword(string oldpassword,string comfpassword)
        {
            
            con.Open();
            string msgpass = "";
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_customer where password = '" + oldpassword + "' ", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if(dt.Rows[0]["password"].ToString() == oldpassword)
                {
                    string query = "update Tbl_customer set  password= '" + comfpassword + "' where  password= '" + oldpassword + "'";
                    
                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();
                    
                    if (res != "-1")
                    {
                        msgpass = "PassSuccess";
                    }                    
                }                                 
            }
            else
            {
                msgpass = "InvalidOldPass";

            }

            return RedirectToAction("MyProfile", "SignatureRestaurant", new { id = Session["CustomerID"], @msgpass = msgpass });
        }

        public ActionResult Register(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Save(Mcustregister reg)
        {
            try
            {
                con.Open();
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_customer where Email = '" + reg.Email + "' and Contact = '" + reg.Contact + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                }
                else
                {
                    string query = "insert into Tbl_customer(Name,Password,Contact,Email,RegDate)";
                    query += " values('" + reg.Name + "','" + reg.Password + "','" + reg.Contact + "','" + reg.Email + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "')";

                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                    }
                }
                return RedirectToAction("Register", "SignatureRestaurant", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Register", "SignatureRestaurant", new { @msg = msg });
            }
        }

        public ActionResult Login(string msg)
        {
            ViewBag.errormsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Saves(Mcustlogin login)
        {
            string Email = "";
            string password = "";
            string name = "";
            string msg = "";
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_customer where Email = '" + login.Email + "' and password = '" + login.Password + "' ", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["Email"].ToString() == login.Email && dt.Rows[0]["password"].ToString() == login.Password)
                {
                    Session["CustomerID"] = dt.Rows[0]["CustomerID"].ToString();
                    Session["Email"] = dt.Rows[0]["Email"].ToString();
                    Session["Name"]= dt.Rows[0]["Name"].ToString();
                    return RedirectToAction("Home", "SignatureRestaurant");
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
            return RedirectToAction("Login", new { msg = msg });

        }

        public ActionResult Verifycontact(string msg)
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult SendOTP(string Emailvery)
        {
            
            string msg = "";           
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_customer where Email = '" + Emailvery + "'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int _min = 1000;
                int _max = 9999;
                
                Random _rdm = new Random();
                int OTP = _rdm.Next(_min, _max);
                Session["OTP"] = OTP;
                Session["emails"] = Emailvery;
                Session["Name"] = dt.Rows[0]["Name"].ToString();

                //===== send otp=====                
                
                var fromAddress = new MailAddress("signaturerestaurant007@gmail.com", "Signature Restaurant");
                var toAddress = new MailAddress(Emailvery, Session["Name"].ToString());
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


                return RedirectToAction("VerifyOTP", "SignatureRestaurant");
                //, new { otp = OTP }

            }
            else
            {
                msg = "Invalid";
                return RedirectToAction("Verifycontact", "SignatureRestaurant", new { msg = msg });

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
                return RedirectToAction("NewPassword","SignatureRestaurant");

            }
            else
            {
                msg = "Invalid";
                return RedirectToAction("VerifyOTP","SignatureRestaurant", new { msg = msg });

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

                string query = "update Tbl_customer set  password= '" + ConfPass + "' where Email = '" + Session["emails"] + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Login", "SignatureRestaurant");

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("NewPassword", "SignatureRestaurant", new { @msg = msg });
            }
        }

        public ActionResult Logout()
        {
            if (Session["Email"] != null)
            {
                Session.Remove("Email");
            }
            return RedirectToAction("Home", "SignatureRestaurant");
        }        
    }
}