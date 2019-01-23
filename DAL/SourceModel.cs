using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Office.DAL
{
    static class SourceModel
    {


        public static DataTable GetSources()
        {
            return DbHelper.GetTable("Source");
        }
        public static bool DeleteSource(int id)
        {

            return DbHelper.DeleteById("Source", id);
        }
        public static int AddSource(string name)
        {
            string query = string.Format("INSERT INTO Source(name) VALUES(N'{0}') ", name);
            return DB.ExecuteNonQuery(query); 
        }
        public static bool Exist(string name)
        {

            string query = string.Format("SELECT COUNT(*) AS result FROM Source WHERE LTRIM(RTRIM(name)) = N'{0}' ", name);
            int res = int.Parse( DB.ExecuteCommand(query).Tables[0].Rows[0]["result"].ToString());
            return res > 0; 

        }
    }
}
