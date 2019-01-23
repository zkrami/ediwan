using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office.BL
{
    class Contact
    {
        public int id; 
        public string name, email, website, address, phone, mobile;

        static public Contact CreateFromCsv(string csv)
        {
            Contact c = new Contact();
            string[] data = csv.Split(','); 

            c.id = int.Parse(data[0].Trim());
            c.name = data[1].Trim();
            c.email = data[2].Trim();
            c.website = data[3].Trim();
            c.address = data[4].Trim();
            c.mobile = data[5].Trim(); 


            return c; 

        }
    }
}
