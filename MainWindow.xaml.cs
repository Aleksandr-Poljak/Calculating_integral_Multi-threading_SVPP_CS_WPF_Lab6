using System.ComponentModel;
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
        BackgroundWorker BgWorker;

        public MainWindow()
        {
            InitializeComponent();
            BgWorker = (BackgroundWorker) this.Resources["BgWorker"];
        }

        /// <summary>
        ///  Обработчик события для конпки ввода данных.
        /// </summary>
        private void Btn_InputData_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optWindow = new();

            if (optWindow.ShowDialog() is not true) return;
            integral = optWindow.integral;
        }
        
        /*
         * _______________Вычисление через диспетчер___________________________________
         */

        /// <summary>
        /// Обработчик события для конпки Dispather.
        /// Вычисление интеграла в новом потоке с использованем диспетчера.
        /// </summary>
        private void Btn_Dispathcer_Click(object sender, RoutedEventArgs e)
        {
            if(integral != null)
            {
                ListBox_Result.Items.Clear();

                // Добавление  объекту Integral обработчика события,
                // возникающему до начала вычисления. Обработчик отключает кнопки
                integral.EventBefore += ButtonsOff_Dispatcher;

                // Добавление  объекту Integral обработчика события,
                // возникающему после вычисления.Обработчик включает кнопки
                integral.EventCompleted += ButtonsOn_Dispatcher;
                // Удаление всех обработчиков у всех событий объекта
                integral.EventCompleted += (s, e) => (s as Integral)!.ClearEventsHandlers();

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

        /*
         * _______________Вычисление через BackgroundWorker ___________________________________
         */

        /// <summary>
        ///  Обработчик события кнопки BackgroundWorker
        /// Очищает ListBox, Добавляет интегралу обрабочик события для записи в ListBox,
        /// Запускает worker. 
        /// </summary>
        private void Btn_Worker_Click(object sender, RoutedEventArgs e)
        {
            if (integral is null) return;

            ListBox_Result.Items.Clear();
            // Добавляем обрабочик события интегралу для записи в ListBox.
            integral.EventStep += WriteListBox_Dispatcher;
            BgWorker.RunWorkerAsync(integral); // Передаем объект инетеграла.
        }


        /// <summary>
        /// Обработчик события DoWork BackgroundWorker.
        /// Отключает кнопки, добавляет обработчик для установки прогресса. Запускает вычисление
        /// </summary>
        private void BgWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            // Отключаем кнопки UI главном потоке через Диспетчер. 
            ButtonsOff_Dispatcher(sender, e);
            // Устанавливаем объект интеграла в результат, для доступа к нему
            // из обработчика BgWorker_Completed
            Integral integral = (Integral)e.Argument!;
            e.Result = integral;

            // Добавляем событию объекту интеграла обработчик для вызова прогресса
            integral.EventStep += (s, args) =>
            {
                BgWorker.ReportProgress((int)((args.CurrentStep / (double)integral.Steps) * 100));
            };
            // Запуск вычисления.
            integral.Calculate();
        }

        /// <summary>
        /// Обработчик события RunWorkerCompleted BackgroundWorker.
        /// Включает кнопки, вызывает метод удаления всех обработчиков у объекта инетграла.
        /// </summary>
        private void BgWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() => AllButtons_OnOff(true)); // Включаем кнопки в UI потоке
            Integral integral = (e.Result as Integral)!;
            integral.ClearEventsHandlers();
        }

        /// <summary>
        /// Обработчик события ProgressChanged BackgroundWorker. Заполянет ProgressBar.
        /// </summary>
        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InstallProgressBar(e.ProgressPercentage);
                  
        }

        /*
         * _______________Вычисление через асинхронный стрим ___________________________________
         */
        private void Btn_Async_Click(object sender, RoutedEventArgs e)
        {

        }

        /*
         * _______________Вспомогательные методы для разных способов вычисления ___________________
         */

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
            Dispatcher.Invoke(()=> AllButtons_OnOff(true));
        }

        /// <summary>
        /// Обработчик события для отключения всех кнопок из другого потока.
        /// </summary>
        private void ButtonsOff_Dispatcher(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() => AllButtons_OnOff(false));
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