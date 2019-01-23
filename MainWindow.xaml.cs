using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Data;
using Office.DAL;
using Office.BL;
using System.IO;
using Microsoft.Win32;  
namespace Office
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
         
            new SplashScreen(App.SplashMainResource).Show(true);
            Thread.Sleep(3500);
            InitializeComponent();
            

          
        }
      

        private void incomnigButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                IncomingWindow incoming = new IncomingWindow();
                incoming.ShowDialog();

                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("لقد حدث خطا غير متوقع");
                App.Log(ex.Message);
                
                Environment.Exit(Environment.ExitCode);

            }

        }

        private void outgoingButton_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                this.Hide();
                OutgoingWindow outgoin = new OutgoingWindow();

                outgoin.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("لقد حدث خطا غير متوقع");
                App.Log(ex.Message);
                Environment.Exit(Environment.ExitCode);

            }

        }
        public void Show()
        {
            base.Show();
            //Topmost = true;
            this.Activate(); 

        }

        private void settingsButton_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {

                this.Hide();

                OptionWindow opt = new OptionWindow();
                opt.ShowDialog();

                this.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show("لقد حدث خطا غير متوقع");
                App.Log(ex.Message);
                Environment.Exit(Environment.ExitCode);

            }
        }

        private void contactButton_Click_1(object sender, RoutedEventArgs e)
        {

            

            try
            {
                this.Hide();

                ContactsWindow contact = new ContactsWindow();

                contact.ShowDialog();

                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("لقد حدث خطا غير متوقع");
                App.Log(ex.Message);
                Environment.Exit(Environment.ExitCode);

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //32767.
      
        }



        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            return;

            
            
            Topmost = true;
            this.Activate(); 
        }

        

      

        private void export_Click_1(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Zip Files |*.zip";

            if (dlg.ShowDialog() == new Nullable<bool>(true))
            {

                Thread wait = new Thread(() =>
                {
                    WaitingForm waiting = new WaitingForm();
                    waiting.ShowDialog();
                    

                });
                wait.Start();
                Thread.Sleep(1); 
                try
                {


                    AppBackup.Export(dlg.FileName);
                    wait.Abort();
                    MessageBox.Show("لقد تمت عملية التصدير بنجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("الرجاء اعادة تشغيل البرنامج بوضع المسؤول");
                    App.Log(ex.Message);
                }
                finally
                {
                    if(wait.IsAlive)
                    wait.Abort();
                }
            }

        }

        private void import_Click_1(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Zip Files |*.zip";
            if (dlg.ShowDialog() == new Nullable<bool>(true))
            {
                Thread wait = new Thread(() =>
                {
                    WaitingForm waiting = new WaitingForm();
                    waiting.ShowDialog();


                });
                wait.Start();
                Thread.Sleep(1); 
                try
                {

                    AppBackup.Import(dlg.FileName);
                    wait.Abort();
                    MessageBox.Show("لقد تمت عمليت الاستيراد بنجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("الرجاء اعادة تشغيل البرنامج بوضع المسؤول");
                    App.Log(ex.Message);
                }
                finally
                {
                    if (wait.IsAlive)
                        wait.Abort();
                }
            }
        }
       
       
    }
}
