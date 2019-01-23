using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Office.BL;
using System.Data;
using System.Data.SqlClient; 
namespace Office.DAL
{
    static class RecordModel
    {

        static public int AddIncoming(Record record)
        {

            SqlCommand sql = new SqlCommand();

            sql.CommandText = "INSERT INTO incoming( bookmarker , date , title,  number , source , type_index  )  VALUES( @bookmarker  , @date, @title , @number , @source , @type_index  ) SELECT SCOPE_IDENTITY()"; 


            //VALUES( @bookmarker  , @date, @D , @number , @source , @type_index , @img ) SELECT SCOPE_IDENTITY()"; 

            sql.Parameters.AddWithValue("@bookmarker" , record.bookmarker );
            sql.Parameters.AddWithValue("@date", record.date);
            sql.Parameters.AddWithValue("@title", record.title);
            sql.Parameters.AddWithValue("@number", record.number);
            sql.Parameters.AddWithValue("@source", record.source);
            sql.Parameters.AddWithValue("@type_index", record.typeIndex);
           
            
            return int.Parse(DB.ExecuteScalar(sql).ToString());
                                
        }
        static public int UpdateIncoming(Record record)
        {

            string query = string.Format("UPDATE incoming SET bookmarker =  N'{0}' , date =   N'{1}', title =  N'{2}',  number  = N'{3}' , source  =  N'{4}', type_index = {5}   WHERE id = {6}  "
                                                       , record.bookmarker, record.date.ToShortDateString(), record.title, record.number, record.source, record.typeIndex,  record.id);
            return DB.ExecuteNonQuery(query); 
        }
        static public int AddOutgoing(Record record)
        {

            SqlCommand sql = new SqlCommand();

            sql.CommandText = "INSERT INTO outgoing( bookmarker , date , title,  number , source  , type_index  )  VALUES( @bookmarker  , @date, @title , @number , @source , @type_index  ) SELECT SCOPE_IDENTITY()";


            
            sql.Parameters.AddWithValue("@bookmarker", record.bookmarker);
            sql.Parameters.AddWithValue("@date", record.date);
            sql.Parameters.AddWithValue("@title", record.title);
            sql.Parameters.AddWithValue("@number", record.number);
            sql.Parameters.AddWithValue("@source", record.source);
            sql.Parameters.AddWithValue("@type_index", record.typeIndex);
            

            return int.Parse(DB.ExecuteScalar(sql).ToString());
            
        }
        static public int AddOutgoingImage(int id, string image)
        {


            string query = string.Format("INSERT INTO [OutgoingFiles] (record_id , url) VALUES( {0} , '{1}' )", id, image);
            return DB.ExecuteNonQuery(query);


        }



        static public int AddIncomingImage(int id, string image)
        {


            string query = string.Format("INSERT INTO [IncomingFiles] (record_id , url) VALUES( {0} , '{1}' )", id, image);
            return DB.ExecuteNonQuery(query);


        }

    
       
        static public int UpdateOutgoing(Record record)
        {

            string query = string.Format("UPDATE outgoing SET bookmarker =  N'{0}' , date =   N'{1}', title =  N'{2}',  number  = N'{3}' , source  =  N'{4}', type_index = {5}  WHERE id = {6}  "
                                                       , record.bookmarker, record.date.ToShortDateString(), record.title, record.number, record.source, record.typeIndex, record.id);
            return DB.ExecuteNonQuery(query);
        }
        static public bool DeletePhysical(string url)
        {
            try
            {
                System.IO.File.Delete(App.DefaultRecordsFolder + url);
                return true;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return true; 
            }
            catch (Exception ex)
            {
                App.Log(ex.Message);

            }
            return false; 
        }
        static public bool DeleteIncomingImages(int id)
        {
            DataTable dt =  GetIncomingImages(id); 
            bool result = true;
            foreach (DataRow row in dt.Rows)
            {
                result &= DeleteIncomingImage(int.Parse(row["id"].ToString()), row["url"].ToString()); 
            }

            return result; 
        }
        static public bool DeleteIncoming(int id)
        {
            bool result = DeleteIncomingImages(id);
            if (!result) return false;
            return DbHelper.DeleteById("incoming", id) ; 

        }
        static public bool CheckNumberIncomingExsist(int number)
        {
            return DB.ExecuteCommand("SELECT * FROM INCOMING WHERE number = " + number).Tables[0].Rows.Count > 0; 
        }
        static public bool CheckNumberOutgoingExsist(int number)
        {
            return DB.ExecuteCommand("SELECT * FROM outgoing WHERE number = " + number).Tables[0].Rows.Count > 0;
        }
        static public bool DeleteIncomingImage(int id , string url )
        {
            if (!DeletePhysical(url)) return false; 
            return DbHelper.DeleteById("IncomingFiles", id);
        }
      


        static public bool DeleteOutgoingImages(int id)
        {
            DataTable dt = GetOutgoingImages(id);
            bool result = true;
            foreach (DataRow row in dt.Rows)
            {
                result &= DeleteOutgoingImage(int.Parse(row["id"].ToString()), row["url"].ToString()); 
            }

            return result;
        }
        static public bool DeleteOutgoing(int id)
        {
            bool result = DeleteOutgoingImages(id);
            if (!result) return false;

            return DbHelper.DeleteById("outgoing", id); 

        }

        static public bool DeleteOutgoingImage(int id , string url )
        {
            if (!DeletePhysical(url)) return false; 
            return DbHelper.DeleteById("OutgoingFiles", id);
        }


        
        static public DataTable GetIncoming()
        {
            DataSet ds = DB.ExecuteCommand("SELECT incoming.* , Type.name as type  FROM incoming INNER JOIN Type ON Type.id = incoming.type_index");
            return ds.Tables[0]; 
        }
        
        static public DataTable GetOutgoing()
        {
            DataSet ds = DB.ExecuteCommand("SELECT outgoing.* , Type.name as type  FROM outgoing INNER JOIN Type ON Type.id = outgoing.type_index");
            return ds.Tables[0]; 
        }

        static public DataTable GetIncomingImages(int id){
            DataSet ds = DB.ExecuteCommand("SELECT * FROM IncomingFiles WHERE record_id = " + id); 
            return ds.Tables[0]; 
        }
        static public DataTable GetOutgoingImages(int id)
        {
            DataSet ds = DB.ExecuteCommand("SELECT * FROM OutgoingFiles WHERE record_id = " + id);
            return ds.Tables[0];
        }

    }
}
