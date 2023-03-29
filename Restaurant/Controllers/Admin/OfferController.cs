using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Restaurant.Models;
using static Restaurant.Models.Moffer;



namespace Restaurant.Controllers.Admin
{
    public class OfferController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Offer
        public ActionResult Index()
        {
            DataTable OfferList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_offer", con);
            adp.Fill(OfferList);
            return View(OfferList);
        }
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Save(Moffer ofr)
        {
            try
            {
                con.Open();
                string msg = "";
                string filename = "";
                // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                string FileExtension = Path.GetExtension(ofr.Image.FileName);
                if (ofr.Image.FileName != "" && ofr.Image.FileName != null)
                {
                    filename = "Offer_" + DateTime.Now.ToString("yyyyMMddss") + "_" + FileExtension;
                }


                string query = "insert into Tbl_offer (Name,Image,StartDate,EndDate,TotalAmt,Discount,IsActive)";
                query += " values('" + ofr.Name + "','" + filename + "','" + Convert.ToDateTime(ofr.StartDate).ToString("yyyy/MM/dd") + "','" + Convert.ToDateTime(ofr.EndDate).ToString("yyyy/MM/dd") + "','" + ofr.TotalAmt + "','" + ofr.Discount + "','" + ofr.IsActive + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                    if (ofr.Image.FileName != "" && ofr.Image.FileName != null)
                    {
                        ofr.Image.SaveAs(Server.MapPath("~/Upload/Offer/" + filename));
                    }
                }
                return RedirectToAction("Create", "Offer", new { @msg = msg });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Offer", new { @msg = msg });
            }
        }

        public ActionResult Edit(int id, string msg, Moffer ofr)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_offer where OfferID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    ofr.OfferID = Convert.ToInt32(dr["OfferID"]);
                    ofr.Name = dr["Name"].ToString();                   
                    ofr.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    ofr.EndDate = Convert.ToDateTime(dr["EndDate"]);               
                    ofr.TotalAmt = Convert.ToInt32(dr["TotalAmt"]);
                    ofr.Discount = Convert.ToInt32(dr["Discount"]);
                    ofr.IsActive = dr["IsActive"].ToString();
                    ofr.oldimage = dr["Image"].ToString();
                    if (dr["Image"] != null)
                    {
                        ViewBag.image = dr["Image"];
                    }

                }

                return View(ofr);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Offer", new { id = id, @msg = msg });
            }

        }

        [HttpPost]
        public ActionResult Update(Moffer ofr)
        {
            try
            {
                string filename = "";
                // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                if (ofr.Image != null)
                {
                    string FileExtension = Path.GetExtension(ofr.Image.FileName);
                    filename = "Offer_" + DateTime.Now.ToString("yyyyMMddss") + "_" + FileExtension;
                }
                else
                {
                    filename = ofr.oldimage;
                }
                string query = "update Tbl_offer set Name = '" + ofr.Name + "',Image = '" + filename + "',StartDate = '" + Convert.ToDateTime(ofr.StartDate).ToString("yyyy/MM/dd") + "',EndDate = '" + Convert.ToDateTime(ofr.EndDate).ToString("yyyy/MM/dd") + "',TotalAmt = '" + ofr.TotalAmt + "',Discount = '" + ofr.Discount + "',IsActive = '" + ofr.IsActive + "' where OfferID='" + ofr.OfferID + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                if (ofr.Image != null)
                {
                    ofr.Image.SaveAs(Server.MapPath("~/Upload/Offer/" + filename));
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Offer", new { id = ofr.OfferID, @msg = msg });
            }


        }

        public ActionResult offerstatus(int id, string status)
        {
            con.Open();          
            string query = "update Tbl_offer set IsActive='" + status + "' where OfferID='" + id + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index", "Offer");
        }

        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_offer where OfferID=" + did;
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