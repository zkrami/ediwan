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
using System.Data;
using System.Threading;
namespace Office
{
    /// <summary>
    /// Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        
        public OptionWindow()
        {
            new SplashScreen(App.SplashResource).Show(true);
            Thread.Sleep(500);
            InitializeComponent();
            

            Style rowStyle = sourceGrid.RowStyle;
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(SourceGrid_DoubleClick)));


            rowStyle = typeGrid.RowStyle;
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(TypeGrid_DoubleClick)));



            RefreshSources();
            RefreshTypes();
        }


        private void SourceGrid_DoubleClick(object sender, RoutedEventArgs e)
        {

            if (sourceGrid.SelectedIndex == -1) return;

            int id = int.Parse((sourceGrid.SelectedItem as DataRowView)[0].ToString());
            if (SourceModel.DeleteSource(id))
            {
                MessageBox.Show("لقد تم الحذف بنجاح");
            }
            else
            {
                MessageBox.Show("لقد حدث خطأ ما");

            }



            RefreshSources(); 
        
        }

        private void TypeGrid_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (typeGrid.SelectedIndex == -1) return;
            int id = int.Parse((typeGrid.SelectedItem as DataRowView)[0].ToString());

            if (TypeModel.CountRecordType(id) > 0)
            {

                MessageBox.Show("الرجاء حذف جميع السجلات المتعلقة بهذا الكتاب اولا");
                return; 
            }

            if (TypeModel.DeleteType(id))
            {
                MessageBox.Show("لقد تم الحذف بنجاح");
            }
            else
            {
                MessageBox.Show("لقد حدث خطأ ما");
            }
            RefreshTypes(); 

        }
        void RefreshSources()
        {

            sourceGrid.ItemsSource = SourceModel.GetSources().DefaultView; 
        }
        void RefreshTypes()
        {
            typeGrid.ItemsSource = TypeModel.GetTypes().DefaultView; 
        }
        private void sourceButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sourceName.Text)) return;
            if (SourceModel.AddSource(sourceName.Text.Trim()) == 1)
            {
                Console.Beep(2000, 200);
                sourceName.Text = ""; 
            }
            else
            {
                MessageBox.Show("لقد حدث خطأ ما "); 

            }
            
            RefreshSources(); 

        }

        private void typeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(typeName.Text)) return;
            string idNum = typeId.Text.Trim();
            if (string.IsNullOrWhiteSpace(idNum))
            {


                if (TypeModel.AddType(typeName.Text.Trim()) == 1)
                {
                    Console.Beep(2000, 200);
                 
                }
                else
                {
                    MessageBox.Show("لقد حدث خطأ ما ");
                }


            }
            else
            {

                uint id;
                if (!uint.TryParse(idNum, out id) || id > 2000000000)
                {
                    MessageBox.Show("الرجاء ادخال رقم صالح بين 0 وال 2000000000");
                    return;
                }
                if (TypeModel.IsExist(id))
                {

                    MessageBox.Show("إن الرقم المدخل موجود مسبقا الرجاء المحاولة مرة أخرى");
                    typeId.Focus(); 
                    return;
                }
                if (TypeModel.AddType(typeName.Text.Trim() , id) == 1)
                {
                    Console.Beep(2000, 200);
                    
                }
                else
                {
                    MessageBox.Show("لقد حدث خطأ ما ");
                }



            }


            typeName.Text = "";
            typeId.Text = "";

            RefreshTypes();

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
