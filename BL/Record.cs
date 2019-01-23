using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office.BL
{
    public class ImageRecord{
        public int id {set; get;}  
        public string url {set ; get; } 
        int record_id { set; get; }  
        public ImageRecord(int id , string url , int record_id){
            this.id = id; 
            this.url = url; 
            this.record_id = record_id ; 
        }
        public ImageRecord() { }
        public static implicit operator ImageRecord(DataRow row)
        {
            ImageRecord record = new ImageRecord();
            record.id = int.Parse(row["id"].ToString());
            record.record_id = int.Parse(row["record_id"].ToString());
            record.url = row["url"].ToString();
            return record; 

        }
        public override string ToString()
        {
            return this.url; 
        }
    }
    public class Record
    {


        public int id; 
        public string bookmarker;
        public DateTime date;
        public string title;        
        public int number;
        public string source;
        public string type;
        public string img;
        public int typeIndex;

        public static implicit operator Record(DataRowView row)
        {
            Record r = new Record();
            r.id = int.Parse(row.Row["id"].ToString());
            r.title = row.Row["title"].ToString();
            r.source = row.Row["source"].ToString();
            
            r.date = DateTime.Parse(row.Row["date"].ToString());
            
            r.bookmarker = row.Row["bookmarker"].ToString();

            r.number = int.Parse(row.Row["number"].ToString()); 
            
            r.type = row.Row["type"].ToString(); 
            r.typeIndex = int.Parse(row.Row["type_index"].ToString());
           

            return r;  
            
        }
        public static Record CreateFromCsv(string row)
        {

            string[] columns = row.Split(','); 
            Record r = new Record();
            r.id = int.Parse(columns[0].Trim());
            r.bookmarker = columns[1].Trim();
            r.date = DateTime.Parse(columns[2]);
            r.title = columns[3];
            r.number = int.Parse(columns[4].Trim());
            r.source = columns[5].Trim();
            r.typeIndex = int.Parse(columns[6].Trim()); 







            return r; 

        }


    }
}
