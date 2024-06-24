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

namespace SVPP_CS_WPF_Lab6_Calculating_integral_Multi_threading_
{
    /// <summary>
    /// Логика взаимодействия для OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public Integral integral = new(0, 5.7, 120);

        public OptionsWindow()
        {
            InitializeComponent();
            this.DataContext = integral;
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();   
        }

        /// <summary>
        /// Обработчик события ввода данных. Отключает кнопку OK, если вводятся некорректные данные.
        /// </summary>
        private void TextInput_Validate(object sender, TextChangedEventArgs e)
        {

            TextBox tb = (TextBox)sender;
            if (tb.GetBindingExpression(TextBox.TextProperty).HasValidationError is true)
                Btn_Ok.IsEnabled = false;
            else Btn_Ok.IsEnabled = true;
        }
    }
}
