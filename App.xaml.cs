using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Office
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string SplashMainResource = @"Sources\splash.jpg";
        public static string SplashResource = @"Sources\splash.jpg";
        static string[] imgExt = new string[] { ".jpg", ".jpeg", ".png" };
        public static ImageSource fileIcon = null; 
        
        public static string DefaultRecordsFolder = @"img\"; 
        public  App():base()
        {
            fileIcon = new ImageSourceConverter().ConvertFromString("pack://application:,,,/Sources/papers-icon.png") as ImageSource;            

        }

        static bool logging = true;
        public static void Log(string message)
        {
            if (!logging) return;
            try
            {
                TextWriter tw = new StreamWriter("log2.txt", true); 

                tw.WriteLine(message);
                tw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }
        public static void FileNotFoundMessage()
        {
            MessageBox.Show("الملف غير موجود لفد تم حذف الارتباط بين الملف والسجل الرجاء اضافة الملف من جديد");
        }
        public static void Error()
        {

            MessageBox.Show("لقد حدث خطأ ما الرجاء المحاولة لاحقا");

        }

        public static void Success()
        {
            Console.Beep(2000, 200);

        }
        static public bool IsImage(string imgPath)
        {

            string extension = System.IO.Path.GetExtension(imgPath).ToLower();
            foreach (string ext in imgExt)
                if (ext == extension) return true;
            return false;



        }

        
       
    }
}
