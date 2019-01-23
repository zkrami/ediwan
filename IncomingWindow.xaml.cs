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
using Office.BL;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Windows.Media.Animation;
using Saraff.Twain;
using System.Threading;
using System.Drawing.Imaging;
using System.Diagnostics; 
namespace Office
{
    /// <summary>
    /// Interaction logic for IncomingWindow.xaml
    /// </summary>
    public partial class IncomingWindow : Window
    {
        OpenFileDialog imgDialog;
        //string imgPath = "";
        IBindingListView blv;
        
        void RefreshIncoming()
        {
            DataTable dt = RecordModel.GetIncoming();
            blv = dt.DefaultView;


            
            incomingGrid.ItemsSource = dt.DefaultView;
           
        }
      
      
        public IncomingWindow()
        {
            new SplashScreen(App.SplashResource).Show(true);
            Thread.Sleep(500);
            InitializeComponent();
            imgDialog = new OpenFileDialog();
            imgDialog.Filter = "Files  | *.jpg; *.jpeg; *.png; *.doc; *.docx; *.pdf";


            DataTable dt = SourceModel.GetSources();
            foreach (DataRow row in dt.Rows)
            {
                sourceList.Items.Add(row[1].ToString().Trim());
            }
            dt = TypeModel.GetTypes();
            foreach (DataRow row in dt.Rows)
            {
                typeList.Items.Add(new RecordType(int.Parse(row[0].ToString().Trim()) , row[1].ToString()));

            }




            RefreshIncoming();

            Style rowStyle = incomingGrid.RowStyle;
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(Row_DoubleClick)));

            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseRightButtonDownEvent,
                                    new MouseButtonEventHandler(Row_RightClick)));
            

            incomingGrid.SelectedCellsChanged += incomingGrid_SelectedCellsChanged;
            


         
        }


        int recordImageIndex = 0;
        List<ImageRecord> recordImages = new List<ImageRecord>();


        string showerFile;  
        void ImageChange()
        {

            imgShower.Source = null;
            showerFile = ""; 
            if (recordImageIndex >= recordImages.Count || recordImageIndex < 0 ) return;

             ImageRecord imageRec = recordImages[recordImageIndex]; 
             string img = recordImages[recordImageIndex].url;


            showerFile = img; 
            if (App.IsImage(img))
            {


                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.UriSource = new Uri(App.DefaultRecordsFolder + img, UriKind.Relative);
                    image.EndInit();
                    imgShower.Source = image;
                }
                catch (FileNotFoundException ex)
                {
                    App.Log(ex.Message);
                    RecordModel.DeleteIncomingImage(imageRec.id, imageRec.url);
                    App.FileNotFoundMessage();
                    LoadImages();
                }
                
            }
            else
            {
                imgShower.Source = App.fileIcon;
            }

           

            
        }
        void LoadImages()
        {
            imgShower.Source = null;
            showerFile = "";
            recordImageIndex = 0;
            recordImages.Clear();

            int id = GetCurrentRecord();
            if (id == -1) return;




            DataTable dt = RecordModel.GetIncomingImages(id);

            foreach (DataRow row in dt.Rows)
                recordImages.Add((ImageRecord)row); 


            ImageChange(); 

        }
        void incomingGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            LoadImages(); 

        }
        private void Row_RightClick(object sender, RoutedEventArgs e)
        {
            if (incomingGrid.SelectedIndex == -1) return;
            DataRowView current = (DataRowView)incomingGrid.SelectedItem;
            IncomingEditWindow window = new IncomingEditWindow();
            window.record = current;
            window.Init();
            window.ShowDialog();
            RefreshIncoming();
            LoadImages();

        }

        private int GetCurrentRecord()
        {
            if (incomingGrid.SelectedIndex == -1) return -1;
            DataRowView current = (DataRowView)incomingGrid.SelectedItem;
            int id = int.Parse(current.Row["id"].ToString());
            return id; 

        }
        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {

            if (incomingGrid.SelectedIndex == -1) return;
            if (MessageBox.Show("هل انت متاكد ؟ ", "تاكيد عملية الحذف", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            DataRowView current = (DataRowView)incomingGrid.SelectedItem;
            int id = int.Parse(current.Row["id"].ToString());
            imgShower.Source = null;
            
            
            if (RecordModel.DeleteIncoming(id) == true)
            {

                MessageBox.Show("لقد تم حذف السجل بنجاح");
            }
            else
            {
                MessageBox.Show("فشلت العملية! الرجاء اغلاق جميع البرامج التي تستخدم الملف والمحاولة مرة أخرى"); 
            }
            RefreshIncoming();
            LoadImages();


        }
        private void Row_Click(object sender, RoutedEventArgs e)
        {




        }
       
        private void Button_Click_1(object sender, RoutedEventArgs e)
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

            int number ;
            if (!int.TryParse(numberTxt.Text, out number) || number < 0 ) 
            {
                MessageBox.Show("الرقم جيب ان يكون بين 0 و " + int.MaxValue);
                return;

            }

            if (RecordModel.CheckNumberIncomingExsist(number))
            {
                MessageBox.Show("ان الرقم المدخل موجود مسبقا الرجاء ادخال رقم اخر");
                return; 
            }

            Record record = new Record();
            record.date = (DateTime)date.SelectedDate;
            record.bookmarker = bookmarkTxt.Text;
            record.number = number;
            record.source = sourceList.SelectedItem.ToString();
            record.title = titleTxt.Text;
            record.type = typeList.SelectedItem.ToString();
            record.typeIndex = (typeList.SelectedItem as RecordType).id; 
            

            try{
                int id = RecordModel.AddIncoming(record) ;
                idLabel.Content = "الرقم التسلسي: " + id.ToString();

                App.Success();
               // date.SelectedDate = null;
                bookmarkTxt.Text = "";
               // sourceList.SelectedIndex = -1;
               // typeList.SelectedIndex = -1; 
                titleTxt.Text = "";
                numberTxt.Text = ""; 
            }
            catch(Exception ex){
                App.Log(ex.Message);
                App.Error(); 
            }
            RefreshIncoming();
    

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int id = GetCurrentRecord();
            if (id == -1) return; 

            Nullable<bool> result = imgDialog.ShowDialog();
            if (result == true)
            {
                string imgPath = imgDialog.FileName;


                string extension = System.IO.Path.GetExtension(imgPath);
                string path = ""; 
                do
                {
                    path = Guid.NewGuid().ToString() + extension;

                } while (File.Exists(App.DefaultRecordsFolder + path));

                if (!Directory.Exists(App.DefaultRecordsFolder))
                    Directory.CreateDirectory(App.DefaultRecordsFolder);

                try
                {
                    File.Copy(imgPath, App.DefaultRecordsFolder + path);

                    if (RecordModel.AddIncomingImage(id, path) == 1)
                    {
                        App.Success();
                        LoadImages();
                    }
                    else
                    {
                        App.Error();
                    }
                }
                catch (Exception ex)
                {
                    App.Log(ex.Message);
                    App.Error(); 
                }
                //img.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(imgPath);
            }

        }

        private void filter_Changed(object sender, EventArgs e)
        {
            List<string> filters = new List<string>();
            int converted;
            if (!string.IsNullOrWhiteSpace(idFilter.Text) && int.TryParse(idFilter.Text.Trim(), out converted))
            {
                filters.Add("id = " + converted ); 
            }

            if (!string.IsNullOrWhiteSpace(typeIndexFilter.Text) && int.TryParse(typeIndexFilter.Text.Trim(), out converted))
            {
                filters.Add("type_index = " + converted);
            }
            
            if (!string.IsNullOrWhiteSpace(typeFilter.Text))
            {
                filters.Add("type LIKE '%" + typeFilter.Text.Trim() + "%'");
            }
            if (!string.IsNullOrWhiteSpace(sourceFilter.Text))
            {
                filters.Add("source LIKE '%" + sourceFilter.Text.Trim() + "%'");
            }
            if (!string.IsNullOrWhiteSpace(titleFilter.Text))
            {
                filters.Add("title LIKE '%" + titleFilter.Text.Trim() + "%'");
            }
            if (!string.IsNullOrWhiteSpace(bookmarkerFilter.Text))
            {
                filters.Add("bookmarker LIKE '%" + bookmarkerFilter.Text.Trim() + "%'");
            }
            int numConvert; 
            if (!string.IsNullOrWhiteSpace(numberFilter.Text) && int.TryParse(numberFilter.Text , out numConvert))
            {
                filters.Add("number   = " + numConvert );
            }
            if (dateFromFilter.SelectedDate != null)
            {

                filters.Add("date >= '" + dateFromFilter.SelectedDate.ToString() + "'" );
            }
            if (dateToFilter.SelectedDate != null)
            {

                filters.Add("date <= '" + dateToFilter.SelectedDate.ToString() + "'");
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
        bool animated = false;
        double searchGridHeight = 110; 
        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
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

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {

            if (imgShower.Source == null)
            {

                MessageBox.Show("الرجاء اختيار صورة اولا");
                return;
            }
            string titile = "image";
            if (incomingGrid.SelectedIndex != -1)
            {
                titile = (incomingGrid.SelectedItem as DataRowView)["title"].ToString();
            }

            if (App.IsImage(showerFile))
            {
                PrintDialog dlg = new PrintDialog();
                bool? result = dlg.ShowDialog();

                if (result.HasValue && result.Value)
                {

                    imgShower.Measure(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                    imgShower.Arrange(new Rect(new Point(0, 0), imgShower.DesiredSize));

                    dlg.PrintVisual(imgShower, titile);
                }
            }
            else
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    Verb = "print",
                    FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"img\" + showerFile
                    
                };
                p.Start();

            }


        }

        private void typeIndexFilter_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            filter_Changed(this, e); 

        }

        private void scanBtn_Click(object sendere, RoutedEventArgs er)
        {
            int idToChange = GetCurrentRecord();
            if (idToChange == -1) return;      
            try
            {
                List<string> resolutions = new List<string>();
                using (Twain32 _twain32 = new Twain32())
                {
                    _twain32.ShowUI = false;
                    _twain32.IsTwain2Enable = false;
                    _twain32.OpenDSM();



                    if (_twain32.SourcesCount == 0)
                    {
                        MessageBox.Show("الرجاء توصيل الماسح الضوئي والمحاولة مرة أخرى لاحقا");
                        return;

                    }
               

                    _twain32.CloseDataSource();
                    bool bb = _twain32.SelectSource();
                    if (bb == false) return;

                    bool b = _twain32.OpenDataSource();

                    if (b == false)
                    {
                        MessageBox.Show("لم يتمكن البرنامج من استخدام الماسح الرجاء المحاولة لاحقا");
                        return;
                    }

                   
                    var _resolutions = _twain32.Capabilities.XResolution.Get();
                    _twain32.Capabilities.Indicators.Set(false);
                    _twain32.Capabilities.FeederLoaded.Set(false);
                    _twain32.Capabilities.XResolution.Set(100);
                    _twain32.Capabilities.YResolution.Set(100);
                    _twain32.Capabilities.PixelType.Set(TwPixelType.RGB);
                  
                    _twain32.EndXfer += (object sender, Twain32.EndXferEventArgs e) =>
                    {
                        try
                        {
                            
                        }
                        catch (Exception ex)
                        {
                            App.Log(String.Format("{0}: {1}{2}{3}{2}", ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace));
                        }
                    };


                    _twain32.TwainStateChanged += (sender, e) => { int p = 0; };


                    _twain32.AcquireCompleted += (sender, e) =>
                    {
                        try
                        {

                            if (_twain32.ImageCount > 0)
                            {


                                System.Drawing.Image img = _twain32.GetImage(0);


                                string path = "";
                                string extension = ".jpg";
                                do
                                {
                                    path = Guid.NewGuid().ToString() + extension;

                                } while (File.Exists(App.DefaultRecordsFolder + path));

                              
                                try
                                {
                                    if (!Directory.Exists(App.DefaultRecordsFolder))
                                        Directory.CreateDirectory(App.DefaultRecordsFolder);

                                    img.Save(App.DefaultRecordsFolder + path);


                                    if (RecordModel.AddIncomingImage(idToChange, path) == 1)
                                    {
                                        App.Success();
                                        LoadImages();
                                    }
                                    else
                                    {
                                        App.Error();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    App.Log(ex.Message);
                                    App.Error();
                                }
                                
                            }

                        }
                        catch (Exception ex)
                        {
                            App.Log(ex.Message);
                        }

                        
                    };




                    _twain32.AcquireError += (object sender, Twain32.AcquireErrorEventArgs e) =>
                    {
                        try
                        {

                            App.Log(string.Format("Acquire Error: ReturnCode = {0}; ConditionCode = {1};", e.Exception.ReturnCode, e.Exception.ConditionCode));
                            App.Log(e.Exception.Message);
                        }
                        catch (Exception ex)
                        {
                            App.Log(ex.Message);
                        }
                    };

                    _twain32.Acquire();
                }

            }
            catch (Exception ex)
            {
                App.Log(ex.Message);
                MessageBox.Show("لم تتم العملية بنجاح"); 
            }
             
        }
        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
     
        }



        
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            if (recordImages.Count == 0) return; 
            
            recordImageIndex++;
            recordImageIndex %= recordImages.Count;

            ImageChange();
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            if (recordImages.Count == 0) return;

            recordImageIndex--;
            recordImageIndex = (recordImageIndex % recordImages.Count + recordImages.Count) % recordImages.Count; 

            ImageChange();
        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount != 2) return;
            if (string.IsNullOrWhiteSpace(showerFile)) return;

            Process wordProcess = new Process();
            wordProcess.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + App.DefaultRecordsFolder + showerFile;

            wordProcess.StartInfo.UseShellExecute = true;
            wordProcess.Start();
        }
    }
}
