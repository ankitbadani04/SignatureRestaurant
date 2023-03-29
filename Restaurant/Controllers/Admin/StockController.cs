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
using static Restaurant.Models.Mstock;


namespace Restaurant.Controllers
{
    public class StockController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: Stock
        public ActionResult Index()
        {
            DataTable StockList = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select sto.*,par.PartyName,ing.IngName from Tbl_stock as sto LEFT JOIN  Tbl_party as par ON par.PartyID=sto.PartyID  LEFT JOIN  Tbl_ingredients as ing ON ing.IngID=sto.IngID ", con);
            adp.Fill(StockList);
            return View(StockList);
        }
        
        // GET: Stock/Details/5
        public ActionResult Stockreport()
        {
            con.Open();
            List<IngStock> ingstocklist = new List<IngStock>();
            string totalstock = "0";
            string ingr = "-";
            string totalitemsale = "0";
            IngStock obj = null;
            string totalinguse = "0";
            double usestock = 0;
            double avastock = 0;
            SqlCommand cmd = new SqlCommand("select *,(select sum(StockQty) from Tbl_stock where IngID=Tbl_ingredients.IngID) as totalstock from Tbl_ingredients", con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    usestock = 0;
                    avastock = 0;
                    if (reader["totalstock"].ToString() == "" || reader["totalstock"].ToString() == null)
                    {
                        totalstock = "0";
                    }
                    else
                    {
                        totalstock = reader["totalstock"].ToString();
                    }

                    ingr = reader["IngName"].ToString();

                    obj = new IngStock();
                    obj.ingname = ingr;
                    obj.totalstock = totalstock;

                    ViewBag.stock = totalstock;


                    SqlCommand items = new SqlCommand("select ItemID,sum(OItemQty) as totalsale from Tbl_orderitem group by ItemID", con);
                    using (SqlDataReader itemreader = items.ExecuteReader())
                    {
                        while (itemreader.Read())
                        {
                            if (itemreader["ItemID"].ToString() == "" || itemreader["ItemID"].ToString() == null)
                            {

                                totalitemsale = "0";
                            }
                            else
                            { 
                                totalitemsale = itemreader["totalsale"].ToString();
                            }

                            SqlCommand iteming = new SqlCommand("select sum(ItemIngQty) as totaling from Tbl_iteming where ItemID='" + itemreader["ItemID"] + "' and IngID='" + reader["IngID"] + "'", con);
                            using (SqlDataReader ingreader = iteming.ExecuteReader())
                            {
                                while (ingreader.Read())
                                {
                                   
                                    if (ingreader["totaling"].ToString()=="" || ingreader["totaling"].ToString()==null)
                                    {

                                         totalinguse = "0";
                                    }
                                    else
                                    {

                                         totalinguse = ingreader["totaling"].ToString();
                                    }
                                    usestock = usestock+ Convert.ToDouble((Convert.ToInt32(totalinguse) * Convert.ToInt32(totalitemsale)).ToString());
                                    
                                }
                            }
                        }
                    }
                    avastock = Convert.ToDouble((Convert.ToInt32(totalstock) - usestock));
                    obj.usestock = usestock.ToString();
                    obj.avastock = avastock.ToString();
                    ingstocklist.Add(obj);
                }
            }
            con.Close();
            return View(ingstocklist);
        }

        // GET: Stock/Create
        public ActionResult Create(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            string Text = "";
            string Value = "";
            con.Open();

            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_party", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem
                {

                    Text = dr["PartyName"].ToString(),
                    Value = dr["PartyID"].ToString()
                });

            }
            ViewBag.PartyList = new SelectList(list, "Value", "Text");

            SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_ingredients", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            List<SelectListItem> list1 = new List<SelectListItem>();
            foreach (DataRow dr1 in dt1.Rows)
            {
                list1.Add(new SelectListItem
                {

                    Text = dr1["IngName"].ToString(),
                    Value = dr1["IngID"].ToString()
                });

            }
            ViewBag.IngList = new SelectList(list1, "Value", "Text");

            return View();
        }

        // POST: Stock/Create
        [HttpPost]
        public ActionResult Save(Mstock stock)
        {
            try
            {
                con.Open();
                string msg = "";
                string query = "insert into Tbl_stock (PartyID,ingID,StockQty)";
                query += " values('" + stock.PartyID + "','" + stock.IngID + "','" + stock.StockQty + "')";

                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                if (res != "-1")
                {
                    msg = "Success";
                }
                return RedirectToAction("Create", "Stock", new { @msg = msg });
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Create", "Stock", new { @msg = msg });
            }
        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int id, string msg, Mstock stock)
        {
            ViewBag.errormsg = msg;
            try
            {
                con.Open();
                string query = "select * from Tbl_stock where StockID=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    stock.StockID = Convert.ToInt32(dr["StockID"]);
                    stock.PartyID = Convert.ToInt32(dr["PartyID"]);
                    stock.IngID = Convert.ToInt32(dr["IngID"]);
                    stock.StockQty = Convert.ToInt32(dr["StockQty"]);
                }
                dr.Close();
                SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_party", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                List<SelectListItem> list = new List<SelectListItem>();
                foreach (DataRow drs in dt.Rows)
                {
                    list.Add(new SelectListItem
                    {

                        Text = drs["PartyName"].ToString(),
                        Value = drs["PartyID"].ToString()
                    });

                }
                ViewBag.PartyList = new SelectList(list, "Value", "Text");

                SqlDataAdapter adp1 = new SqlDataAdapter("select * from Tbl_ingredients", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);

                List<SelectListItem> list1 = new List<SelectListItem>();
                foreach (DataRow dr1 in dt1.Rows)
                {
                    list1.Add(new SelectListItem
                    {

                        Text = dr1["IngName"].ToString(),
                        Value = dr1["IngID"].ToString()
                    });

                }
                ViewBag.IngList = new SelectList(list1, "Value", "Text");

                return View(stock);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Stock", new { id = id, @msg = msg });
            }
           
        }

        // POST: Stock/Edit/5
        [HttpPost]
        public ActionResult Update(Mstock stock)
        {
            try
            {
                string query = "update Tbl_stock set PartyID = '" + stock.PartyID + "',IngID='" + stock.IngID + "',StockQty='" + stock.StockQty + "' where StockID = '" + stock.StockID + "'";
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                con.Close();
                return RedirectToAction("Edit", "Stock", new { id = stock.StockID, @msg = msg });
            }


        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Stock/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                con.Open();
                string did = collection["txthfdid"];
                string query = "delete from Tbl_stock where StockID=" + did;
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
