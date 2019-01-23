using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using Office.DAL;
using System.Data;
using System.Windows;
using System.Data.SqlClient;

namespace Office.BL
{
    static class AppBackup
    {



       

        public static void Import(string source)
        {
          try
            {
            
          
                if (Directory.Exists("temp"))
                    Directory.Delete("temp", true);




                var zip = new ZipFile(source);
                Directory.CreateDirectory("temp");
                zip.ExtractAll("temp");
                //Directory.Delete(App.DefaultRecordsFolder , true);
                if(!Directory.Exists(App.DefaultRecordsFolder))
                Directory.CreateDirectory(App.DefaultRecordsFolder);



                //TODO Check if the distination already has the file 
                foreach (string f in Directory.GetFiles(@"temp\img"))
                    File.Copy(f, App.DefaultRecordsFolder + @"\" + Path.GetFileName(f), true); 


                DbHelper.Import(Directory.GetCurrentDirectory() + @"\temp\");


                Directory.Delete("temp", true); 
 

                




                

 


            }
            catch (Exception ex)
            {
                

                App.Log(ex.Message);
                throw new Exception("The import proccess has failed"); 

            }
        }


        public static void Export(string distination)
        {
            
            
              try
                {
                    if (Directory.Exists("temp"))
                    {

                        Directory.Delete("temp", true);
                    }

                    Directory.CreateDirectory("temp");  
                    Directory.CreateDirectory(@"temp\img");
                    DbHelper.Export(Directory.GetCurrentDirectory() + @"\temp\");

                    foreach (string f in Directory.GetFiles(App.DefaultRecordsFolder))
                    {
                        File.Copy(f, @"temp\" + f); 
                    }
                    var zip = new ZipFile();
                    zip.AddDirectory("temp");
                    
                    zip.Save(distination);
                    Directory.Delete("temp", true); 
                    
                

                }
                catch (Exception ex)
                {
                    App.Log(ex.Message);
                    throw new Exception("Try to run the program with administrator privileges"); 
                }
           
            
            



            

        }
    }
}
