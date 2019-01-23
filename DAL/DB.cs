using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Office.DAL
{
    public static class DB
    {
        private static SqlConnection sConnection;
        private static SqlCommand sCommand;
        private static SqlDataAdapter sAdapter;


        static DB()
        {
            sConnection = new SqlConnection();
            sConnection.ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\office.mdf;Integrated Security=False; User Instance=False;";            
          //  sConnection.ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\office.mdf;Integrated Security=False; User Instance=False; User Id='temp_user'; password='p@ssw0rd'";            
            
            sCommand = sConnection.CreateCommand();
            sAdapter = new SqlDataAdapter(sCommand);
           
        }

        public static DataSet ExecuteCommand(string sql)
        {
            lock (sConnection)
            {
                DataSet dataSet = null;
                {
                    try
                    {
                        sConnection.Open();
                        sCommand.CommandText = sql;
                        dataSet = new DataSet();
                        sAdapter.Fill(dataSet);
                    }
                    catch (Exception e)
                    {
                        sConnection.Close();
                        throw e;
                        //error**********
                    }
                    finally
                    {
                        sConnection.Close();
                    }
                }
                return dataSet;
            }
        }
        public static int ExecuteNonQuery(string sql)
        {
            lock (sConnection)
            {
                int rowsUpdated = 0;
                try
                {
                    sConnection.Open();
                    sCommand.CommandText = sql;
                    rowsUpdated = sCommand.ExecuteNonQuery();
                    
                }
                catch (Exception e)
                {
                    sConnection.Close();
                    //error************
                    throw e;
                }
                finally
                {
                    sConnection.Close();
                }
                return rowsUpdated;
            }
        }



        public static object ExecuteScalar(SqlCommand sql)
        {
            lock (sConnection)
            {

                object ret = null;
                try
                {
                    sConnection.Open();
                    
                    sql.Connection = sConnection; 
                    ret = sql.ExecuteScalar();
                }
                catch (Exception e)
                {
                    //error************
                    sConnection.Close();
                    throw e;
                }
                finally
                {
                    sConnection.Close();
                }
                return ret;
            }
        }

        public static object ExecuteScalar(string sql)
        {
            lock (sConnection)
            {

                object ret = null;
                try
                {
                    sConnection.Open();
                    sCommand.CommandText = sql;
                    ret = sCommand.ExecuteScalar();
                }
                catch (Exception e)
                {
                    //error************
                    sConnection.Close();
                    throw e;
                }
                finally
                {
                    sConnection.Close();
                }
                return ret;
            }
        }
        static public void Dispose(){
            sConnection.Dispose();
            sCommand.Dispose();
            sAdapter.Dispose();
        }
    }
}