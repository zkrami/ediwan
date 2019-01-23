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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Office
{
    /// <summary>
    /// Interaction logic for NumricBox.xaml
    /// </summary>
    public partial class NumricBox : TextBox
    {
        public NumricBox()
        {
            InitializeComponent();
            this.AddHandler(NumricBox.KeyDownEvent, new RoutedEventHandler(HandleHandledKeyDown), true);


        }




        public void HandleHandledKeyDown(object sender, RoutedEventArgs e)
        {
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Key.Space)
            {
                ke.Handled = true;
            }
        }

        private void textBox_KeyDown_1(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void textBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length == 0) return;
            char c = e.Text[0];

            if (Text.EndsWith(" ")) e.Handled = true;
            if (Char.IsDigit(c) || Char.IsControl(c)) return;
            e.Handled = true;
        }
    }
}
