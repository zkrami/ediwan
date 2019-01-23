using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using Office.BL; 
namespace Office.DAL
{
    static class DbHelper
    {
        
         static string[] tables = new string[]{"Contact" , "outgoing" , "incoming" , "IncomingFiles" , "OutgoingFiles" , "Type" , "Source"}; 

        static public DataTable GetTable(string tableName )
        {

            string query = "SELECT * FROM  " + tableName ;
            DataSet ds = DB.ExecuteCommand(query);
            if (ds.Tables.Count == 0) return new DataTable();
            return ds.Tables[0];
        }
        public static bool DeleteById(string tableName , int id)
        {
            string query = "DELETE FROM " + tableName + " WHERE id = " + id;
            return DB.ExecuteNonQuery(query) == 1;
        }
        static public DataRow GetById(string table, int id)
        {
            string query = "SELECT * FROM  " + table + " WHERE id = " + id;
            DataSet ds = DB.ExecuteCommand(query);            
            return ds.Tables[0].Rows[0];

        }

        static public void Export(string distination)
        {

            foreach(string tableName in tables){
               DataTable dt =  DB.ExecuteCommand(String.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}'" , tableName)).Tables[0]; 
                
                
                StringBuilder concat = new StringBuilder("CONCAT("); 
                
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    concat.Append(dt.Rows[i][0].ToString());
                    concat.Append(", ',' , "); 
                }
                concat.Append(dt.Rows[dt.Rows.Count - 1][0].ToString());
                concat.Append(")");
               


                dt = DB.ExecuteCommand(String.Format("SELECT {0} AS Concat FROM {1}" , concat.ToString() , tableName)).Tables[0];

                concat.Clear();

                foreach (DataRow row in dt.Rows)
                    concat.AppendLine(row[0].ToString());



                File.WriteAllText(distination + tableName + ".csv", concat.ToString()); 





                
            }
        }

        static public Dictionary<int, int> ImportType(string source)
        {

            Dictionary<int, int> typeMap = new Dictionary<int, int>(); 
            foreach (string s in File.ReadAllLines(source + "Type.csv"))
            {

                string[] type = s.Split(',');
                int id = int.Parse(type[0].Trim());
                string name = type[1].Trim();

                int result = TypeModel.IsExist(name) ;
                if (result != -1)
                {
                    //Already exist
                    typeMap[id] = result;
                    continue; 
                }
                result = TypeModel.AddTypeSelectId(name);
                typeMap[id] = result; 


            }
            return typeMap; 
        }

        static void ImportIncomingFiles(string source , Dictionary<int ,int > IncomingMap )
        {

            foreach (string s in File.ReadAllLines(source + "IncomingFiles.csv"))
            {
                string[] img = s.Split(',');
                int recordId = int.Parse(img[1].Trim());
                string url = img[2].Trim();

                int res = IncomingMap[recordId];
                if (res == -1) continue;
                RecordModel.AddIncomingImage(res, url); 

            }

        }
        static public Dictionary<int, int> ImportIncoming(string source, Dictionary<int, int> typeMap)
        {
            Dictionary<int, int> incomingMap = new Dictionary<int, int>();
            foreach (string s in File.ReadAllLines(source + "incoming.csv"))
            {
                Record r = Record.CreateFromCsv(s);
                if (RecordModel.CheckNumberIncomingExsist(r.number))
                {
                    incomingMap[r.id] = -1; // Already Exist 

                    continue; 
                }
                r.typeIndex = typeMap[r.typeIndex];                 
                incomingMap[r.id] = RecordModel.AddIncoming(r); 
                

            }
            return incomingMap; 

        }

        static void ImportOutgoingFiles(string source, Dictionary<int, int> OutgoingMap)
        {

            foreach (string s in File.ReadAllLines(source + "OutgoingFiles.csv"))
            {
                string[] img = s.Split(',');
                int recordId = int.Parse(img[1].Trim());
                string url = img[2].Trim();

                int res = OutgoingMap[recordId];
                if (res == -1) continue;
                RecordModel.AddOutgoingImage(res, url);

            }

        }
        static public Dictionary<int, int> ImportOutgoing(string source, Dictionary<int, int> typeMap)
        {
            Dictionary<int, int> outgoingMap = new Dictionary<int, int>();
            foreach (string s in File.ReadAllLines(source + "outgoing.csv"))
            {
                Record r = Record.CreateFromCsv(s);
                if (RecordModel.CheckNumberOutgoingExsist(r.number))
                {
                    outgoingMap[r.id] = -1; // Already Exist 

                    continue;
                }
                r.typeIndex = typeMap[r.typeIndex];
                outgoingMap[r.id] = RecordModel.AddOutgoing(r);


            }
            return outgoingMap;

        }

        static public void ImportContact(string source)
        {

            foreach (string s in File.ReadAllLines(source + "Contact.csv"))
            {

                Contact con = Contact.CreateFromCsv(s);
                if (ContactModel.Exist(con)) continue;
                ContactModel.Add(con); 

            }
        }
        static public void ImportSource(string source)
        {


            foreach (string s in File.ReadAllLines(source + "Source.csv"))
            {

                string name = s.Split(',')[1].Trim();
                if (SourceModel.Exist(name)) continue;
                SourceModel.AddSource(name);
            }
        }
        static public void Import(string source)
        {
            Dictionary<int, int> typeMap = ImportType(source);
            Dictionary<int, int> IncomingMap = ImportIncoming(source, typeMap);
            Dictionary<int, int> OutgoingMap = ImportOutgoing(source, typeMap);
            ImportIncomingFiles(source, IncomingMap);
            ImportOutgoingFiles(source, OutgoingMap);
            ImportContact(source);
            ImportSource(source);


        }



        static public void RestoreBackup(string source)
        {
            DB.ExecuteNonQuery(@"RESTORE DATABASE [" + Directory.GetCurrentDirectory() + @"\office.mdf] " + 
            @"FROM DISK = N'" + source + "'  WITH FILE = 1 , NOUNLOAD, REPLACE, STATS = 10 , NORECOVERY , " +
            @" MOVE 'office_log' TO '" +  Directory.GetCurrentDirectory() + @"\office_log.ldf'" +
            @" MOVE 'office' TO '" + Directory.GetCurrentDirectory() + @"\office.mdf'");
        }


    }
}
