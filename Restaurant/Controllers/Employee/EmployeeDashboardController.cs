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
using static Restaurant.Models.Morder;

namespace Restaurant.Controllers.Employee
{
    public class EmployeeDashboardController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        // GET: EmployeeDashboard
        public ActionResult Index(string msg = "")
        {
            ViewBag.ErrorMsg = msg;
            Morder mord = new Morder();

            DataTable OrderList1 = new DataTable();            
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table", con);            
            adp.Fill(OrderList1);
            mord.Orderlist = OrderList1;

            DataTable OrderList3 = new DataTable();
            SqlDataAdapter adp3 = new SqlDataAdapter("select ordi.*,ord.OrderID,ord.TableID,item.ItemName from Tbl_orderitem as ordi LEFT JOIN Tbl_order as ord ON ord.OrderID=ordi.OrderID LEFT JOIN Tbl_item as item ON item.ItemID=ordi.ItemID where ord.OrderStatus='New' or ord.OrderStatus='Preparing' or ord.OrderStatus='Delivered' order by OrderItemID desc", con);
            adp3.Fill(OrderList3);
            mord.Orderitemlist = OrderList3;

            SqlDataAdapter adp2 = new SqlDataAdapter("select * from Tbl_item", con);
            DataTable dt = new DataTable();
            adp2.Fill(dt);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem
                {

                    Text = dr["ItemName"].ToString(),
                    Value = dr["ItemID"].ToString()
                });

            }
            ViewBag.CategoryList = new SelectList(list, "Value", "Text");

            
            return View(mord);
        }

        [HttpPost]
        public ActionResult additem(FormCollection oitem, string msg)
        {
            bool flag = false;
            string qty = oitem["Qty"].ToString();
            ViewBag.errormsg = msg;
            try
            {
                con.Open();

                //=======Stock repport======
                List<IngStock> ingstocklist = new List<IngStock>();
                string totalstock = "0";
                string ingr = "-";
                string totalitemsale = "0", totalitemsale1 = "0";
                IngStock obj = null;
                string totalinguse = "0", totalinguse1 = "0";
                double usestock = 0, usestock1 = 0;
                double avastock = 0;
                SqlCommand cmdstk = new SqlCommand("select *,(select sum(StockQty) from Tbl_stock where IngID=Tbl_ingredients.IngID) as totalstock from Tbl_ingredients where IngID in (select IngID from Tbl_iteming where ItemID='" + oitem["ItemID"] + "' )", con);
                using (SqlDataReader reader = cmdstk.ExecuteReader())
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


                        SqlCommand items = new SqlCommand("select ItemID,sum(OItemQty) as totalsale from Tbl_orderitem where ItemID='" + oitem["ItemID"] + "' group by ItemID", con);
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

                                        if (ingreader["totaling"].ToString() == "" || ingreader["totaling"].ToString() == null)
                                        {

                                            totalinguse = "0";
                                        }
                                        else
                                        {

                                            totalinguse = ingreader["totaling"].ToString();
                                        }
                                        usestock = usestock + Convert.ToDouble((Convert.ToInt32(totalinguse) * Convert.ToInt32(totalitemsale)).ToString());

                                    }
                                }
                            }

                            string id = oitem["ItemID"].ToString();
                            string iid1 = reader["IngID"].ToString();

                            SqlCommand iteming1 = new SqlCommand("select sum(ItemIngQty) as totaling from Tbl_iteming where ItemID='" + id + "' and IngID='" + iid1 + "'", con);
                            using (SqlDataReader ingreader1 = iteming1.ExecuteReader())
                            {
                                while (ingreader1.Read())
                                {

                                    if (ingreader1["totaling"].ToString() == "" || ingreader1["totaling"].ToString() == null)
                                    {

                                        totalinguse1 = "0";
                                    }
                                    else
                                    {

                                        totalinguse1 = ingreader1["totaling"].ToString();
                                    }
                                    usestock1 = Convert.ToDouble((Convert.ToInt32(totalinguse1) * Convert.ToInt32(totalitemsale1)).ToString());

                                }
                            }
                        }

                        //======= Stock Report Change  ======
                        //======= /Stock Report Change ======
                        double s = Convert.ToInt32(totalstock) - Convert.ToInt32(usestock);
                        double b = Convert.ToDouble(totalinguse1) * Convert.ToDouble(qty);
                        avastock = s - b;
                        if ((s - b) < 0)
                        {
                            flag = true;
                            break;
                        }
                        obj.usestock = usestock.ToString();
                        obj.avastock = avastock.ToString();
                        ingstocklist.Add(obj);
                    }
                }
                //=======Stock repport======


                if (flag)
                {
                    msg = "stockunavailable";
                    return RedirectToAction("Index", "EmployeeDashboard", new { @msg = msg });
                }
                else
                {
                    msg = "";
                    int OrderID = 0;
                    SqlDataAdapter adpC = new SqlDataAdapter("select * from Tbl_order where TableID='" + oitem["txthfdid"] + "' and (OrderStatus='New' or OrderStatus='Preparing' or OrderStatus='Delivered')", con);
                    DataTable dtC = new DataTable();
                    adpC.Fill(dtC);
                    if (dtC.Rows.Count > 0)
                    {
                        SqlCommand cmd1 = new SqlCommand("SELECT MAX(OrderID) FROM Tbl_order where TableID='" + oitem["txthfdid"] + "' ", con);
                        OrderID = Convert.ToInt32(cmd1.ExecuteScalar());
                    }
                    else
                    {

                        string query = "insert into Tbl_order (TableID,UserID,OrderDate,OrderStatus,OrderType)";
                        query += " values('" + oitem["txthfdid"] + "','" + Session["EmployeeID"].ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "','New','In Restaurant')";

                        string query1 = "update Tbl_table set IsActive = 'Yes' where TableID='" + oitem["txthfdid"] + "'";


                        SqlCommand cmds = new SqlCommand(query1, con);
                        cmds.ExecuteNonQuery();

                        SqlCommand cmd = new SqlCommand(query, con);
                        string res = cmd.ExecuteNonQuery().ToString();

                        SqlCommand cmd1 = new SqlCommand("SELECT MAX(OrderID) FROM Tbl_order", con);
                        OrderID = Convert.ToInt32(cmd1.ExecuteScalar());

                        string querylog = "insert into Tbl_log (OrderID,LogStatus,LogDate)";
                        querylog += " values('" + OrderID + "','New','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "')";
                        SqlCommand cmdl = new SqlCommand(querylog, con);
                        cmdl.ExecuteNonQuery();
                    }

                    SqlDataAdapter adp2 = new SqlDataAdapter("select * from Tbl_item where ItemID='" + oitem["ItemID"] + "'", con);
                    DataTable dt = new DataTable();
                    adp2.Fill(dt);
                    string price = "0";
                    foreach (DataRow dr in dt.Rows)
                    {
                        price = dr["ItemPrice"].ToString();
                    }


                    string querys = "insert into Tbl_orderitem (OrderID,ItemID,OItemQty,OItemPrice,OItemStatus)";
                    querys += " values('" + OrderID + "','" + oitem["ItemID"] + "','" + oitem["Qty"] + "','" + price + "','New')";
                    SqlCommand cmd2 = new SqlCommand(querys, con);
                    string res1 = cmd2.ExecuteNonQuery().ToString();

                    return RedirectToAction("Index", "EmployeeDashboard", new { @msg = msg });
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                con.Close();
                return RedirectToAction("Index", "EmployeeDashboard", new { @msg = msg });
            }

        }


        public ActionResult Complete(FormCollection ord,Morder order)
        {
            con.Open();
            string odrid = "0";
            SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_order where TableID='" + ord["txthfdids"] + "' and OrderStatus='Delivered'", con);
            DataTable dts = new DataTable();
            adps.Fill(dts);
            if (dts.Rows.Count > 0)
            {
                
                foreach (DataRow dr in dts.Rows)
                {
                    odrid = dr["OrderID"].ToString();
                }
                string subtotal = "0";
                SqlDataAdapter adp = new SqlDataAdapter("select SUM(OItemQty * OItemPrice) as Subtotal from Tbl_orderitem where OrderID ='" + odrid + "' and OitemStatus='Complete'  GROUP BY OrderID", con);
                DataTable totla = new DataTable();
                adp.Fill(totla);
                if (totla.Rows.Count > 0)
                {
                   
                    foreach (DataRow dr in totla.Rows)
                    {
                        subtotal = dr["Subtotal"].ToString();
                    }
                }
                
                string query = "update Tbl_order set NoPerson = '" + ord["nop"] + "',OrderDiscount = '" + ord["discount"] + "',CustContact = '" + ord["contact"] + "',OrderTotalBill = '" + subtotal + "',OrderStatus='Complete' where OrderID = '" + odrid + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();

                string query2 = "update Tbl_orderitem set OitemStatus='Complete' where OrderID = '" + odrid + "' and OitemStatus='New' or OitemStatus='Preparing' or OitemStatus='Complete' ";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.ExecuteNonQuery();

                string query1 = "update Tbl_table set IsActive = 'No' where TableID='" + ord["txthfdids"] + "'";               
                SqlCommand cmds = new SqlCommand(query1, con);
                cmds.ExecuteNonQuery();

                string querylog = "insert into Tbl_log (OrderID,LogStatus,LogDate)";
                querylog += " values('" + odrid + "','Complete','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "')";

                SqlCommand cmdl = new SqlCommand(querylog, con);
                cmdl.ExecuteNonQuery();


            }            
            return RedirectToAction("Create", "Review", new { @id= odrid });
        }

        public ActionResult CompleteOrder(string id = "")
        {
           
            Morder mord = new Morder();

            DataTable OrderList1 = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from Tbl_table", con);
            adp.Fill(OrderList1);
            mord.Orderlist = OrderList1;

            DataTable OrderList3 = new DataTable();
            SqlDataAdapter adp3 = new SqlDataAdapter("select * from Tbl_order where OrderStatus='Complete' order by OrderID desc", con);
            adp3.Fill(OrderList3);
            mord.Orderitemlist = OrderList3;

            DataTable OrderLists = new DataTable();
            string sql = "";
            sql = "select ordi.*,ord.OrderID,item.ItemName from Tbl_orderitem as ordi LEFT JOIN Tbl_order as ord ON ord.OrderID=ordi.OrderID LEFT JOIN Tbl_item as item ON item.ItemID=ordi.ItemID";
            if (id != "" && id != null)
            {
                sql += "  where ord.OrderId=" + id;
            }
            SqlDataAdapter adps = new SqlDataAdapter(sql, con);
            adps.Fill(OrderLists);
            mord.orditem = OrderLists;

            return View(mord);
        }

        public ActionResult itemcancel(int id)
        {

            
                con.Open();               
                string query = "delete from Tbl_orderitem where OrderItemID=" + id;
                SqlCommand cmd = new SqlCommand(query, con);
                string res = cmd.ExecuteNonQuery().ToString();

                return RedirectToAction("Index", "EmployeeDashboard");
                       


            //con.Open();
            //SqlDataAdapter adps = new SqlDataAdapter("select * from Tbl_orderitem where OrderItemID='" + id + "' and OitemStatus='New'", con);
            //DataTable dts = new DataTable();
            //adps.Fill(dts);
            //if (dts.Rows.Count > 0)
            //{
            //    string query = "update Tbl_orderitem set OitemStatus='Cancel' where OrderItemID = " + id;
            //    SqlCommand cmd = new SqlCommand(query, con);
            //    cmd.ExecuteNonQuery();
            //}
            //return RedirectToAction("Index", "EmployeeDashboard");
        }
    }
}