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
        public ActionResult Save(Mingredients ing)
        {
            try
            {
                con.Open();
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_ingredients where IngName = '" + ing.IngName + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                }
                else
                {
                    string filename = "";
                    // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                    string FileExtension = Path.GetExtension(ing.IngImage.FileName);
                    if (ing.IngImage.FileName != "" && ing.IngImage.FileName != null)
                    {
                        filename = "INGRE_" + DateTime.Now.ToString("yyyyMMddss") + "_" + FileExtension;
                    }


                
                    string query = "insert into Tbl_ingredients (IngName,IngImage)";
                    query += " values('" + ing.IngName + "','" + filename + "')";

                    SqlCommand cmd = new SqlCommand(query, con);
                    string res = cmd.ExecuteNonQuery().ToString();

                    if (res != "-1")
                    {
                        msg = "Success";
                        if (ing.IngImage.FileName != "" && ing.IngImage.FileName != null)
                        {
                            ing.IngImage.SaveAs(Server.MapPath("~/Upload/Ingredients/" + filename));
                        }                        
                    }                   
                }
                return RedirectToAction("Create", "Ingredients", new { @msg = msg });
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Ingredients", new { @msg = msg });
            }
        }

        // GET: Ingredients/Edit/5
        public ActionResult Edit(int id,string msg,Mingredients ing)
        {
            ViewBag.errormsg = msg;
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
                    ing.oldimage = dr["IngImage"].ToString();
                    if (dr["IngImage"] != null)
                    {
                        ViewBag.image = dr["IngImage"];
                    }

                }

                return View(ing);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Ingredients", new { id = id, @msg = msg });
            }
        }

        // POST: Ingredients/Edit/5
        [HttpPost]
        public ActionResult Update(Mingredients ing)
        {
            try
            {
                string msg = "";
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_ingredients where IngName = '" + ing.IngName + "' and IngID !='" + ing.IngID + "'", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    msg = "Exist";
                    return RedirectToAction("Edit", "Ingredients", new { id = ing.IngID, @msg = msg });
                }
                else
                {
                    string filename = "";
                    // string filename = Path.GetFileNameWithoutExtension(item.imageURL.FileName);
                    if (ing.IngImage != null)
                    {
                        string FileExtension = Path.GetExtension(ing.IngImage.FileName);
                        filename = "INGRE_" + DateTime.Now.ToString("yyyyMMddss") + "_" + FileExtension;
                    }
                    else
                    {
                        filename = ing.oldimage;
                    }

                    string query = "update Tbl_ingredients set IngName = '" + ing.IngName + "',IngImage = '" + filename + "'  where IngID='" + ing.IngID + "'";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    if (ing.IngImage != null)
                    {
                        ing.IngImage.SaveAs(Server.MapPath("~/Upload/Ingredients/" + filename));
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Ingredients", new { id = ing.IngID, @msg = msg });
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


        public ActionResult IngredientReport()
        {
            con.Open();
            List<IngredientsReport> items = new List<IngredientsReport>();

            string query = "SELECT *,(select ISNULL(sum(StockQty),0) from Tbl_stock where IngID=Tbl_Ingredients.IngID) as totalstock FROM Tbl_Ingredients";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                IngredientsReport obj = new IngredientsReport();
                obj.ingname = rdr["IngName"].ToString();
                obj.totalstock = rdr["totalstock"].ToString();

                items.Add(obj);
            }

            con.Close();
            ViewBag.items = items;
            return View();
        }

        public class IngredientsReport
        {
            public string ingname { get; set; }
            public string totalstock { get; set; }
        }
    }
}
