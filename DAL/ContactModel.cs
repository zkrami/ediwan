using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;  
using Office.BL; 
namespace Office.DAL
{
    static class ContactModel
    {

        static public DataTable GetContacts()
        {
            return DbHelper.GetTable("Contact"); 
        }
        static public bool Deletet(int id)
        {
            return DbHelper.DeleteById("Contact", id); 
        }

        static public int Add(Contact contact)
        {
            string query = string.Format("INSERT INTO CONTACT(name , email , website , address , phone , mobile) VALUES(N'{0}' ,N'{1}' ,N'{2}' ,N'{3}' ,N'{4}' ,N'{5}' ) ",
                                                           contact.name, contact.email, contact.website, contact.address, contact.phone, contact.mobile);

            return DB.ExecuteNonQuery(query); 
                    

        }

        static public bool Exist(Contact contact)
        {


            string query = string.Format("SELECT COUNT(*) AS result FROM CONTACT WHERE LTRIM(RTRIM(name)) = N'{0}' AND LTRIM(RTRIM(email)) = N'{1}' AND LTRIM(RTRIM(website)) = N'{2}' AND LTRIM(RTRIM(address)) = N'{3}' AND LTRIM(RTRIM(phone)) = N'{4}' AND LTRIM(RTRIM(mobile)) = N'{5}'  ",
                                                                                                contact.name , contact.email , contact.website , contact.address , contact.phone ,  contact.mobile );


            int result = int.Parse(DB.ExecuteCommand(query).Tables[0].Rows[0]["result"].ToString());
            return result > 0; 


        }


    }
}
