using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SVPP_CS_WPF_Lab6_Calculating_integral_Multi_threading_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Integral? integral;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_InputData_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optWindow = new();

            if (optWindow.ShowDialog() is not true) return;
            integral = optWindow.integral;
            MessageBox.Show(integral?.ToString());
        }

        private void Btn_Thread_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}