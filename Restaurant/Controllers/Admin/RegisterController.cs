using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mregister;

namespace Restaurant.Controllers
{
    public class RegisterController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Save(Mregister reg)
        {
            try
            {
                con.Open();
                string query = "insert into Tbl_register(Name,Contact,Email,UserName,Password,IsBlock,RegDate,UserType)";
                query += " values('" + reg.Name + "','" + reg.Contact + "','" + reg.Email + "','" + reg.UserName + "','" + reg.Password + "','" + reg.IsBlock + "','" + reg.RegDate + "','" + reg.UserType + "')";

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
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Register", new { @msg = msg });
            }
        }

    }
}