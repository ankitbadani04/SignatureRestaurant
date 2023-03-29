using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Muser;

namespace Restaurant.Controllers.Admin
{
    public class UserController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Admin()
        {
            DataTable UserList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_User where UserType='Admin' ", con);
            adp.Fill(UserList);
            return View(UserList);            
        }
        public ActionResult Waiter()
        {
            DataTable UserList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_User where UserType='Staff' ", con);
            adp.Fill(UserList);
            return View(UserList);
        }
        public ActionResult Kitchen()
        {
            DataTable UserList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_User where UserType='Chef' ", con);
            adp.Fill(UserList);
            return View(UserList);
        }

        public ActionResult Customer()
        {
            DataTable UserList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_customer ", con);
            adp.Fill(UserList);
            return View(UserList);
        }

        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;            
            return View();
        }

        [HttpPost]
        public ActionResult Save(Muser reg)
        {
            try
            {                
                con.Open();
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_user where UserName = '" + reg.UserName + "' and Contact = '" + reg.Contact + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                }
                else
                {
                    string query = "insert into Tbl_user(Name,Contact,Email,UserName,Password,IsBlock,RegDate,UserType)";
                    query += " values('" + reg.Name + "','" + reg.Contact + "','" + reg.Email + "','" + reg.UserName + "','" + reg.Password + "','" + reg.IsBlock + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "','" + reg.UserType + "')";

                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                    }                    
                }
                return RedirectToAction("Create", "User", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "User", new { @msg = msg });
            }
        }
        public ActionResult Edit(int id, string msg, Muser reg)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_User where UserID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    reg.UserID = Convert.ToInt32(dr["UserID"]);
                    reg.Name = dr["Name"].ToString();
                    reg.Contact = dr["Contact"].ToString();
                    reg.Email = dr["Email"].ToString();
                    reg.UserName = dr["UserName"].ToString();
                    reg.Password = dr["Password"].ToString();
                    reg.IsBlock = dr["IsBlock"].ToString();
                    reg.RegDate = dr["RegDate"].ToString();
                    reg.UserType = dr["UserType"].ToString();
                }                
                return View(reg);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "User", new { id = id, @msg = msg });
            }

        }


        public ActionResult Update(Muser reg)
        {
            try
            {
                string msg = "";

                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_user where UserName = '" + reg.UserName + "' and Contact = '" + reg.Contact + "' and UserID !='" + reg.UserID + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                    return RedirectToAction("Edit", "User", new { id = reg.UserID, @msg = msg });                    
                }
                else
                {
                    string query = "update Tbl_user set Name = '" + reg.Name + "', Contact = '" + reg.Contact + "',Email= '" + reg.Email + "',UserName= '" + reg.UserName + "',Password= '" + reg.Password + "',IsBlock= '" + reg.IsBlock + "',UserType= '" + reg.UserType + "' where UserID='" + reg.UserID + "'";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    

                    if(reg.UserType == "Admin")
                    {
                        return RedirectToAction("Admin", "User");
                    }
                    else if(reg.UserType == "Staff")
                    {
                        return RedirectToAction("Waiter", "User");
                    }
                    else
                    {
                        return RedirectToAction("Kitchen", "User");
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "User", new { id = reg.UserID, @msg = msg });
            }
        }





        public ActionResult Deleteadmin(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_User where UserID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();
                return RedirectToAction("admin");
            }
            catch (Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }
        public ActionResult Deletewaiter(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_User where UserID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();
                return RedirectToAction("waiter");
            }
            catch (Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }
        public ActionResult Deletekitchen(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_User where UserID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();
                return RedirectToAction("kitchen");
            }
            catch (Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }

        public ActionResult UserBlock(int id, string IsBlock ,string type)
        {
            con.Open();
            string query = "update Tbl_user set IsBlock='" + IsBlock + "' where UserID='" + id + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();

            if(type == "Staff")
            {
                return RedirectToAction("Waiter", "User");
            }
            else
            {
                return RedirectToAction("Kitchen", "User");
            }
            
        }
    }
}