using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Restaurant.Models;
using static Restaurant.Models.Mingredients;


namespace Restaurant.Controllers
{
    public class IngredientsController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);

        public object Ingredients { get; private set; }

        // GET: Ingredients
        public ActionResult Index()
        {
            DataTable IngredientsList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_ingredients", con);
            adp.Fill(IngredientsList);
            return View(IngredientsList);            
        }

        // GET: Ingredients/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Ingredients/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            return View();
        }

        // POST: Ingredients/Create
        [HttpPost]
        public ActionResult Save(Mingredients ingr)
        {
            try
            {

                string filename = "";
                // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                string FileExtension = Path.GetExtension(ingr.IngImage.FileName);

                if (ingr.IngImage.FileName != "" && ingr.IngImage.FileName != null)
                {
                    filename = "ITEM_" + DateTime.Now.ToString("yyyyMMdd") + "_" + FileExtension;
                }


                con.Open();
                string query = "insert into Tbl_ingredients (IngName,IngImage)";
                query += " values('" + ingr.IngName + "','" + filename + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    if (ingr.IngImage.FileName != "" && ingr.IngImage.FileName != null)
                    {
                        ingr.IngImage.SaveAs(Server.MapPath("~/Upload/Ingredients/" + filename));
                    }
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
                return RedirectToAction("Create", "Ingredients", new { @msg = msg });
            }
        }

        // GET: Ingredients/Edit/5
        public ActionResult Edit(int id,Mingredients ing)
        {
            try
            {
                con.Open();
                string query = "select * from Tbl_ingredients where IngID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    ing.IngID = Convert.ToInt32(dr["IngID"]);
                    ing.IngName = dr["IngName"].ToString();

                }

                return View(ing);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Ingredients", new { id = id, @msg = msg });
            }
        }

        // POST: Ingredients/Edit/5
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

        // GET: Ingredients/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ingredients/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_ingredients where IngID=" + did;
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
