using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mlogin;

namespace Restaurant.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login        
        public ActionResult Index()
        {
            return View();
        }
    }
}