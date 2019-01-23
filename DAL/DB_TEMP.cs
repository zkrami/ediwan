using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Office.DAL
{
    public  class DB_TEMP
    {
        private  SqlConnection sConnection;
        private  SqlCommand sCommand;
        private  SqlDataAdapter sAdapter;


        public DB_TEMP()
        {
            sConnection = new SqlConnection();
            sConnection.ConnectionString = sConnection.ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\temp\office_backup.mdf;Integrated Security=False; User Instance=False;"; 
            //sConnection.ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\R\documents\visual studio 2012\Projects\Portage\Portage\Portage.mdf;Integrated Security=False; User Instance=False;";

            sCommand = sConnection.CreateCommand();
            sAdapter = new SqlDataAdapter(sCommand);
        }

        public  DataSet ExecuteCommand(string sql)
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
        public  int ExecuteNonQuery(string sql)
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


        public  object ExecuteScalar(SqlCommand sql)
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

        public  object ExecuteScalar(string sql)
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
    }
}