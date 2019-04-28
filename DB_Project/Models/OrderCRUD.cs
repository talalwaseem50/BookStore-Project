﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using DB_Project.Models;

namespace DB_Project.Models
{
    public class OrderCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //public static string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        public static List<Order> GetAllOrders()
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                DataTable orderIDs = new DataTable(); //stores IDs of all books in db
                List<Order> OrdersList = new List<Order>(); //store order objects from db           
                SqlDataAdapter Data = new SqlDataAdapter("Select OrderID From [Orders]", ServerConnection);
                Data.Fill(orderIDs);

                //using procedure that returns book info for one book
                cmd.CommandText = "GetOrder";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //output parameters
                cmd.Parameters.Add(new SqlParameter("@oid",0));
                cmd.Parameters.Add(new SqlParameter("@uid", SqlDbType.Int));
                cmd.Parameters["@uid"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime));
                cmd.Parameters["@date"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.VarChar, 10));
                cmd.Parameters["@status"].Direction = ParameterDirection.Output;             
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                foreach (DataRow row in orderIDs.Rows)
                {
                    Order getOrder = new Order();
                    DataTable Items = new DataTable();
                    SqlDataAdapter data = new SqlDataAdapter(cmd);
                    getOrder.OrderID = (int)row["OrderID"];

                    //input para
                    cmd.Parameters["@oid"].Value = getOrder.OrderID;
                    data.Fill(Items);  //run procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required book id retrieved 

                    if (Flag == 1)
                    {
                        //intializing book obj 
                        getOrder.UserID = (int) cmd.Parameters["@uid"].Value;
                        getOrder.Date = Convert.ToString(cmd.Parameters["@date"].Value);
                        getOrder.OrderStatus = (string) cmd.Parameters["@status"].Value;
                        getOrder.Items = new List<KeyValuePair<int, int>>();

                        foreach (DataRow Row in Items.Rows)
                            getOrder.Items.Add(new KeyValuePair<int, int>((int)Row["ItemID"], (int)Row["Quantity"]));

                        OrdersList.Add(getOrder);
                    }

                }

                ServerConnection.Close();

                return OrdersList;
            }

        }

        public static List<Order> GetUserOrders(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //setting up command to get orderIDs of passed user
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandText = "UsersOrderInfo";
                sqlcmd.Connection = ServerConnection;
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlcmd.Parameters.Add(new SqlParameter("@uid", id));
                sqlcmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                sqlcmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                DataTable orderIDs = new DataTable(); //stores IDs returned by db
                List<Order> OrdersList = new List<Order>(); //store books objects for all books in db            
                SqlDataAdapter Data = new SqlDataAdapter(sqlcmd);
                Data.Fill(orderIDs);    //execute procedure

                int flag = (int)sqlcmd.Parameters["@flag"].Value;

                //populate orders list if user's orders found
                if (flag == 1)
                {
                    //using procedure that returns order info
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetOrder";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //output parameters
                    cmd.Parameters.Add(new SqlParameter("@oid", 0));
                    cmd.Parameters.Add(new SqlParameter("@uid", SqlDbType.Int));
                    cmd.Parameters["@user"].Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime));
                    cmd.Parameters["@date"].Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.VarChar, 10));
                    cmd.Parameters["@status"].Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    //return order info for every returned orderID
                    foreach (DataRow row in orderIDs.Rows)
                    {
                        Order getOrder = new Order();
                        DataTable Items = new DataTable();
                        SqlDataAdapter data = new SqlDataAdapter(cmd);
                        getOrder.OrderID = (int)row["OrderID"];

                        //input para
                        cmd.Parameters["@oid"].Value = getOrder.OrderID;
                        data.Fill(Items);  //run procedure

                        int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required order id retrieved 

                        if (Flag == 1)
                        {
                            //intializing order obj 
                            getOrder.UserID = (int)cmd.Parameters["@user"].Value;
                            getOrder.Date = (string)cmd.Parameters["@date"].Value;
                            getOrder.OrderStatus = (string)cmd.Parameters["@status"].Value;
                            getOrder.Items = new List<KeyValuePair<int, int>>();

                            foreach (DataRow Row in Items.Rows)
                                getOrder.Items.Add(new KeyValuePair<int, int>((int)Row["ItemID"], (int)Row["Quantity"]));

                            OrdersList.Add(getOrder);
                        }
                    }

                    ServerConnection.Close();
                }

                return OrdersList;
                    
            }

        }

        public static List<Book> GetOrderItems(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();                 
                //using procedure that returns book info for one book
                cmd.CommandText = "GetOrderedItems";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add(new SqlParameter("@oid", id));                
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required order id found

                if (Flag == 1)
                {                    
                    DataTable itemIDs = new DataTable(); //stores IDs      
                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(itemIDs);  //get procedure result set

                    foreach (DataRow row in itemIDs.Rows)
                    {
                        books.Add(BookCRUD.GetBook((int)row["ItemID"]));
                    }
                }

                ServerConnection.Close();

                return books;
            }
        }

        public static bool CreateOrder(Order newOrder)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                
                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "placeOrder";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", newOrder.UserID));
                
                //passing table paras
                cmd.Parameters.Add(new SqlParameter("@ITEMTABLE", ListtoDataTableConverter.ListToDataTable<KeyValuePair<int,int>>(newOrder.Items)));
                cmd.Parameters["@ITEMTABLE"].SqlDbType = SqlDbType.Structured;               

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static bool UpdateStatus(int id, string status)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UpdateOrderStatus";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@oid", id));
                cmd.Parameters.Add(new SqlParameter("@status", status));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static bool DeleteOrder(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "RemoveOrder";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@oid", id));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }


    }
}