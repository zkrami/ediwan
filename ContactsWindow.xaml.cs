using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Office.DAL;
using Office.BL;
using System.Data;
using System.Threading;
using System.Windows.Media.Animation;
using System.ComponentModel; 
namespace Office
{
    /// <summary>
    /// Interaction logic for ContactsWindow.xaml
    /// </summary>
    public partial class ContactsWindow : Window
    {
        IBindingListView blv;
       
        public void RefreshContact()
        {

            DataTable dt = ContactModel.GetContacts();

            
            blv = dt.DefaultView;

            contactGrid.ItemsSource = dt.DefaultView; 

        }
        
        
        public ContactsWindow()
        {
            new SplashScreen(App.SplashResource).Show(true);
            Thread.Sleep(500);
            InitializeComponent();


            Style rowStyle = contactGrid.RowStyle;
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(Row_DoubleClick)));

            RefreshContact();
        }
        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (contactGrid.SelectedIndex == -1) return;
            int id = int.Parse((contactGrid.SelectedItem as DataRowView)["id"].ToString());

            if (ContactModel.Deletet(id))
            {
                MessageBox.Show("لقد تم الحذف بنجاح ");
            }
            else
            {
                MessageBox.Show("لقد حدث خطأ ما");
            }

            RefreshContact(); 

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Contact contact = new Contact();
            contact.name = nameTxtr.Text;
            contact.address = addressTxt.Text;
            contact.mobile = mobileTxt.Text;
            contact.phone = phoneText.Text;
            contact.website = websiteText.Text;
            contact.email = emailTxt.Text;
            if (ContactModel.Add(contact) == 1)
            {
                MessageBox.Show("لقد تم الاضافة بنجاح");
            }
            else
            {
                MessageBox.Show("لقد حدث خطا ما "); 
            }
            RefreshContact(); 
            

        }
        void filter_Changed(object sender , EventArgs ev)
        {


            /*
             [name]    NVARCHAR (100) NOT NULL,
    [email]   NVARCHAR (100) NOT NULL,
    [website] NVARCHAR (100) NOT NULL,
    [address] NVARCHAR (100) NOT NULL,
    [phone]   NVARCHAR (100) NOT NULL,
    [mobile]  NVARCHAR (100) NOT NULL,
             */
            List<string> filters = new List<string>();
           
            if (!string.IsNullOrEmpty(nameTxtrFilter.Text))
            {

                filters.Add("name LIKE '%" + nameTxtrFilter.Text + "%'"); 
            }



            if (!string.IsNullOrEmpty(emailTxtFilter.Text))
            {

                filters.Add("email LIKE '%" + emailTxtFilter.Text + "%'");
            }

            if (!string.IsNullOrEmpty(websiteTextFilter.Text))
            {

                filters.Add("website LIKE '%" + websiteTextFilter.Text + "%'");
            }

            if (!string.IsNullOrEmpty(addressTxtFilter.Text))
            {

                filters.Add("address LIKE '%" + addressTxtFilter.Text + "%'");
            }
            if (!string.IsNullOrEmpty(phoneTextFilter.Text))
            {

                filters.Add("phone LIKE '%" + phoneTextFilter.Text + "%'");
            }
            if (!string.IsNullOrEmpty(mobileTxtFilter.Text))
            {

                filters.Add("mobile LIKE '%" + mobileTxtFilter.Text + "%'");
            }


            if (filters.Count == 0)
            {
                blv.Filter = null;
                return;
            }

            string filter = "";
            for (int i = 0; i < filters.Count - 1; i++)
                filter += filters[i] + " AND ";
            filter += filters[filters.Count - 1];
            blv.Filter = filter; 
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }

        bool animated = false;
        double searchGridHeight = 140; 
      
        private void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if (animated)
            {
                DoubleAnimation animation = new DoubleAnimation(searchGrid.ActualHeight, 0, TimeSpan.FromSeconds(0.4));
                searchGrid.BeginAnimation(Rectangle.HeightProperty, animation);
                animated = !animated;
            }
            else
            {

                DoubleAnimation animation = new DoubleAnimation(0, searchGridHeight, TimeSpan.FromSeconds(0.4));
                searchGrid.BeginAnimation(Rectangle.HeightProperty, animation);
                animated = !animated;
            }
        }
    }
}
