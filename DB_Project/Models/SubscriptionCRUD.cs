﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DB_Project.Models
{
    public class SubscriptionCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //public static string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        public static bool AddSubscription(int bid, int uid)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                try
                {
                    cmd.CommandText = "addSubscription";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@uid", uid));
                    cmd.Parameters.Add(new SqlParameter("@itemid", bid));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();  //run procedure
                }
                catch (SqlException ex)
                {
                    Console.Write("SQL Server Error" + ex);
                }
                finally
                {
                    ServerConnection.Close();
                }
                int Flag = (int)cmd.Parameters["@flag"].Value;

                return Flag == 1;

            }
        }

        public static bool UnSubscribe(int bid, int uid)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                try
                {
                    cmd.CommandText = "UnSubscribe";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@uid", uid));
                    cmd.Parameters.Add(new SqlParameter("@itemid", bid));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.Write("SQL Server Error" + ex);
                }
                finally
                {
                    ServerConnection.Close();
                }
                int Flag = (int)cmd.Parameters["@flag"].Value;

                return Flag == 1;

            }
        }

        public static List<Account> GetSubscribers(int bid)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                List<Account> UsersList = new List<Account>();
                DataTable users = new DataTable(); //stores IDs returned by db
                try
                {
                    cmd.CommandText = "ItemsSubscribers";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@itemid", bid));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(users);    //execute procedure
                }
                catch (SqlException ex)
                {
                    Console.Write("SQL Server Error" + ex);
                }
                finally
                {
                    ServerConnection.Close();
                }

                int Flag = (int)cmd.Parameters["@flag"].Value;

                if (Flag == 1)
                {                   
                    foreach (DataRow row in users.Rows)
                    {
                        Account getAcc = AccountCRUD.GetAccount((int)row["SubscriberID"]);
                        if (getAcc != null)
                            UsersList.Add(getAcc);
                    }
                    return UsersList;
                }
                else
                    return null;                
            }
        }

        public static List<Book> GetSubscribedItems(int uid)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                List<Book> BooksList = new List<Book>();
                DataTable users = new DataTable(); //stores IDs returned by db
                try
                {
                    cmd.CommandText = "UsersSubscription";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@uid", uid));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;


                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(users);    //execute procedure
                }
                catch (SqlException ex)
                {
                    Console.Write("SQL Server Error" + ex);
                }
                finally
                {
                    ServerConnection.Close();
                }

                int Flag = (int)cmd.Parameters["@flag"].Value;

                if (Flag == 1) //if found
                {
                    
                    foreach (DataRow row in users.Rows)
                    {
                        Book getBook = BookCRUD.GetBook((int)row["ItemID"]);
                        if (getBook != null)
                            BooksList.Add(getBook);
                    }
                    return BooksList;
                }
                else
                    return null;
            }
        }
    }
}