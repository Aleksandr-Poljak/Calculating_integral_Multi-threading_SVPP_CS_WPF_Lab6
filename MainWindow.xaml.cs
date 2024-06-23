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
using System.Windows.Threading;

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

        /// <summary>
        ///  Обработчик соыбтия для конпки ввода данных.
        /// </summary>
        private void Btn_InputData_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optWindow = new();

            if (optWindow.ShowDialog() is not true) return;
            integral = optWindow.integral;
        }
        
        /// <summary>
        /// Обработчик события для конпки Dispather.
        /// Вычисление интеграла в новом потоке с использованем диспетчера.
        /// </summary>
        private void Btn_Dispathcer_Click(object sender, RoutedEventArgs e)
        {
            if(integral != null)
            {
                // Добавление  объекту Integral обработчика события,
                // возникающему до начала вычисления. Обработчик отключает кнопки
                integral.EventBefore += ButtonsOff_Dispatcher;

                // Добавление  объекту Integral обработчика события,
                // возникающему после вычисления.Обработчик включает кнопки
                integral.EventCompleted += ButtonsOn_Dispatcher;

                // Добавление  объекту Integral обработчика события,
                // на каждой итерации цикла вычисления. Обработчик добавляет промежуточные
                // результаты в ListBox_Result
                integral.EventStep += WriteListBox_Dispatcher;

                // Добавление  объекту Integral обработчика события,
                // на каждой итерации цикла вычисления. Обработчик увеличивает ProgressBar.
                integral.EventStep += AddProgressBar;

                //Запуск нвого потока
                Thread th = new Thread(new ThreadStart(integral.Calculate));
                th.Start();
            }
                

        }


        private void Btn_Worker_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Btn_Async_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Добавляет данные в ListBox_Result. 
        /// </summary>
        private void WriteListBox(double x, double s)
        {
            ListBox_Result.Items.Add($"X= {x:0.00000}\nS= {s:0.00000}");
        }

        /// <summary>
        /// Обработчик события для добавления данных в ListBox_Result из других потоков.
        /// </summary>
        private void WriteListBox_Dispatcher(object? sender, IntegralStepEventArgs e)
        {
            Action writer = () => WriteListBox(e.X, e.S);
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, writer);
        }

        /// <summary>
        /// Включает или отключает все кнопки.
        /// </summary>
        private void AllButtons_OnOff(bool enabled = true)
        {
            foreach (var item in StackPanel_BtnsOperations.Children)
            {
                if (item is Button button)
                {
                    button.IsEnabled = enabled;
                }
            }
        }

        /// <summary>
        /// Обработчик события для включения всех кнопок из другого потока.
        /// </summary>
        private void ButtonsOn_Dispatcher(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(()=> AllButtons_OnOff(true)));
        }

        /// <summary>
        /// Обработчик события для отключения всех кнопок из другого потока.
        /// </summary>
        private void ButtonsOff_Dispatcher(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => AllButtons_OnOff(false)));
        }

        /// <summary>
        /// Увеличивает ProgressBar
        /// </summary>    
        private void InstallProgressBar(double value)
        {
            ProgressBar_Operation.Value = value;
        }

        /// <summary>
        ///  Обработчик события для увелечения ProgressBar из другого потока.
        /// </summary>
        private void AddProgressBar(object? sender, IntegralStepEventArgs e)
        {
            if (integral is null) return;

            double progressValue = (e.CurrentStep / (double)integral.Steps) * 100;
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() => InstallProgressBar(progressValue) ));
        }
    }
}