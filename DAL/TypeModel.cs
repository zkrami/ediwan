using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data; 
namespace Office.DAL
{
    static class TypeModel
    {
        public static DataTable GetTypes()
        {
            return DbHelper.GetTable("Type"); 
        }
        public static bool DeleteType(int id)
        {

            return DbHelper.DeleteById("Type", id); 
        }
        public static int AddType(string name)
        {
           
            string query = string.Format("INSERT INTO [Type](name) VALUES(N'{0}') ", name);
            return DB.ExecuteNonQuery(query);
        }

        public static int AddTypeSelectId(string name)
        {

            string query = string.Format("INSERT INTO [Type](name) VALUES(N'{0}') SELECT SCOPE_IDENTITY() ", name);
            return int.Parse(DB.ExecuteScalar(query).ToString()); 
        }
        public static int AddType(string name , uint id)
        {

            string query = string.Format("SET IDENTITY_INSERT Type ON  INSERT INTO [Type](name , id ) VALUES(N'{0}' , {1} )  SET IDENTITY_INSERT Type OFF   ", name, id);
            return DB.ExecuteNonQuery(query);
        }
        public static int CountRecordType(int id)
        {
            int c1 = int.Parse(DB.ExecuteCommand("SELECT COUNT(*) AS result FROM incoming where type_index = " + id).Tables[0].Rows[0]["result"].ToString());
            int c2 = int.Parse(DB.ExecuteCommand("SELECT COUNT(*) AS result FROM outgoing where type_index = " + id).Tables[0].Rows[0]["result"].ToString());
            return c1 + c2; 
        }
        public static bool IsExist(uint id)
        {
            string query = string.Format("SELECT COUNT(*) AS counted FROM Type Where id = " + id);
            int result = int.Parse( DB.ExecuteCommand(query).Tables[0].Rows[0]["counted"].ToString());
            return result > 0; 
                
        }

        public static int IsExist(string  name)
        {
            string query = string.Format("SELECT TOP 1 id FROM Type Where LTRIM(RTRIM(name)) = N'" + name.Trim() + "'");
            DataTable dt =  DB.ExecuteCommand(query).Tables[0];

            if (dt.Rows.Count == 0) return -1; // NOT FOUND 
            return int.Parse(dt.Rows[0]["id"].ToString()); 


        }
    }
}
