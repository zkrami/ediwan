using Microsoft.Win32;
using Office.BL;
using Office.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Office
{
    /// <summary>
   
    /// </summary>
    public partial class OutgoingEditWindow : Window
    {
        public Record record = null; 
        
        public OutgoingEditWindow()
        {
            InitializeComponent();

            DataTable dt = SourceModel.GetSources();
            foreach (DataRow row in dt.Rows)
            {
                sourceList.Items.Add(row[1].ToString().Trim());
            }
            dt = TypeModel.GetTypes();
            foreach (DataRow row in dt.Rows)
            {
                typeList.Items.Add(new RecordType(int.Parse(row[0].ToString().Trim()), row[1].ToString()));

            }



        }

        public void refreshImages()
        {

            imgShower.Source = null;
            imageList.Items.Clear();
            DataTable dt = RecordModel.GetOutgoingImages(record.id);
            foreach (DataRow row in dt.Rows)
                imageList.Items.Add((ImageRecord)row);

        }
        public void Init()
        {

            sourceList.SelectedItem = record.source;
            typeList.SelectedItem = new RecordType(record.typeIndex, "");
            bookmarkTxt.Text = record.bookmarker;
            numberTxt.Text = record.number.ToString() ;
            titleTxt.Text = record.title;
            date.SelectedDate = record.date;
            idLabel.Content = "الرقم التسلسلي  " + record.id.ToString();

            refreshImages();








        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {


            if (sourceList.SelectedIndex == -1)
            {
                MessageBox.Show("الرجاء اختيار مصدر اولا");
                return;
            }
            if (typeList.SelectedIndex == -1)
            {
                MessageBox.Show("الرجاء اختيار نوع الكتاب اولا");
                return;
            }
            if (String.IsNullOrWhiteSpace(bookmarkTxt.Text))
            {
                MessageBox.Show("الرجاء ادخال المصنف اولا");
                return;
            }
            if (String.IsNullOrWhiteSpace(titleTxt.Text))
            {
                MessageBox.Show("الرجاء ادخال العنوان اولا");
                return;
            }
            if (date.SelectedDate == null)
            {
                MessageBox.Show("الرجاء ادخال التاريخ اولا");
                return;
            }
            int number = 0;
            if (!int.TryParse(numberTxt.Text, out number) || number < 0 )
            {
                MessageBox.Show("الرقم جيب ان يكون بين 0 و " + int.MaxValue);
                return;

            }
            if (number != record.number && RecordModel.CheckNumberOutgoingExsist(number))
            {

                MessageBox.Show("ان الرقم المدخل موجود مسبقا الرجاء ادخال رقم اخر");
                return; 
            }
          




            record.date = (DateTime)date.SelectedDate;
            record.bookmarker = bookmarkTxt.Text;
            record.number =(int) number;
            record.source = sourceList.SelectedItem.ToString();
            record.title = titleTxt.Text;
            record.typeIndex = (typeList.SelectedItem as RecordType).id;
            try
            {

                RecordModel.UpdateOutgoing(record);
                MessageBox.Show("لقد تم تحديث السجل بنجاح");
                record.number = number;
                this.Close();
            }
            catch (Exception ex)
            {
                App.Error();
                App.Log(ex.Message);
            }

        }

        private void imageList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (imageList.SelectedItem == null) return;
            if (MessageBox.Show("هل انت متاكد ؟ ", "تاكيد عملية الحذف", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            ImageRecord image = (ImageRecord)imageList.SelectedItem;
            if (RecordModel.DeleteOutgoingImage(image.id , image.url))
            {
                MessageBox.Show("لقد تم حذف الصورة بنجاح");

            }
            else
            {
                MessageBox.Show("فشلت العملية! الرجاء اغلاق جميع البرامج التي تستخدم الملف والمحاولة مرة أخرى"); 
            }
            refreshImages();

        }

        private void imageList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (imageList.SelectedItem == null) return;
            ImageRecord img = (ImageRecord)imageList.SelectedItem;

            imgShower.Source = null;


            if (App.IsImage(img.url))
            {
                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.UriSource = new Uri(App.DefaultRecordsFolder + img.url, UriKind.Relative);
                    image.EndInit();
                    imgShower.Source = image;
                }
                catch (FileNotFoundException ex)
                {

                    App.Log(ex.Message);
                    RecordModel.DeleteOutgoingImage(img.id, img.url);
                    App.FileNotFoundMessage(); 
                    refreshImages(); 
                }

            }
            else
            {
                imgShower.Source = App.fileIcon;
            }




        }

    }
}
