using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Mparty;


namespace Restaurant.Controllers
{
    public class PartyController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Party
        public ActionResult Index()
        {
            DataTable PartyList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_party", con);
            adp.Fill(PartyList);
            return View(PartyList);
        }

        // GET: Party/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Party/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        // POST: Party/Create
        [HttpPost]
        public ActionResult Save(Mparty party)
        {
            try
            {
                con.Open();
                string query = "insert into Tbl_party (PartyName,PartyContact,PartyEmail,PartyAddress,PartyLandmark,PartyPincode)";
                query += " values('" + party.PartyName + "','" + party.PartyContact + "','" + party.PartyEmail + "','" + party.PartyAddress + "','" + party.PartyLandmark + "','" + party.PartyPincode + "')";

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
            catch(Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Party", new { @msg = msg });
            }
        }

        // GET: Party/Edit/5
        public ActionResult Edit(int id,Mparty party)
        {
            try
            {
                con.Open();
                string query = "select *  from Tbl_party where PartyID =" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    party.PartyID = Convert.ToInt32(dr["PartyID"]);
                    party.PartyName = dr["PartyName"].ToString();
                    party.PartyContact = Convert.ToInt32(dr["PartyContact"]);
                    party.PartyEmail = dr["PartyEmail"].ToString();
                    party.PartyAddress = dr["PartyAddress"].ToString();
                    party.PartyLandmark = dr["PartyLandmark"].ToString();
                    party.PartyPincode = Convert.ToInt32(dr["PartyPincode"]);

                }


                return View(party);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Party", new { id = id, @msg = msg });
            }
        }

        // POST: Party/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Party/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Party/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_party where PartyID=" + did;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                con.Close();
                string msg = ex.Message;
                return View();
            }
        }
    }
}
