﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DB_Project.Models
{
    public class ReviewCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //public static string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        //methods
        public static bool CreateReview(Review newReview)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                try
                {
                    cmd.CommandText = "AddBookReview";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@BID", newReview.BookID));
                    cmd.Parameters.Add(new SqlParameter("@uid", newReview.UserID));
                    cmd.Parameters.Add(new SqlParameter("@rate", newReview.Rating));
                    cmd.Parameters.Add(new SqlParameter("@BookReview", newReview.Description));

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

        public static List<Review> GetReviews(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                List<Review> ReviewList = new List<Review>(); //store objects returned from db    
                try
                {
                    cmd.CommandText = "GetBookReview";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@bid", id));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    DataTable bookReviews = new DataTable(); //stores IDs returned by db                          
                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(bookReviews);    //execute procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;

                    if (Flag == 1) //if found
                    {
                        foreach (DataRow row in bookReviews.Rows)
                        {
                            Review getReview = new Review();
                            getReview.BookID = id;
                            getReview.UserID = (int)row["UserID"];
                            getReview.Rating = Convert.ToInt32(row["Rating"]);
                            getReview.UserName = (string)row["UserName"];
                            getReview.Description = (string)row["Review"];
                            getReview.DatePosted = Convert.ToString(row["Review_Date"]);

                            ReviewList.Add(getReview);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.Write("SQL Server Error" + ex);
                }
                finally
                {
                    ServerConnection.Close();
                }

                return ReviewList;
            }
        }

        public static bool RemoveReview(int bID, int uID)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                try
                {
                    cmd.CommandText = "RemoveReview";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@bid", bID));
                    cmd.Parameters.Add(new SqlParameter("@uid", uID));

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
    }
}