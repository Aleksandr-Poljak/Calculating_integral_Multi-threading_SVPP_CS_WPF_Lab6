using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVPP_CS_WPF_Lab6_Calculating_integral_Multi_threading_
{

    public class IntegralStepEventArgs: EventArgs
    {
        private int currentStep;
        private double x;
        private double s;

        public IntegralStepEventArgs() :base()
        {
        }

        public IntegralStepEventArgs(int currentStep, double x, double s): base()
        {
            CurrentStep = currentStep;
            X = x;
            S = s;
        }

        public int CurrentStep { get => currentStep; set => currentStep = value; }
        public double X { get => x; set => x = value; }
        public double S { get => s; set => s = value; }
    }

    public class Integral: IDataErrorInfo
    {
        double start;
        double end;
        int steps;

        //public Func<double,double> func = (x) => Math.Pow(x, 3); // Функция интеграла
        public Func<double, double> func = (x) => Math.Sqrt(x); // Функция интеграла
        public event EventHandler? EventBefore; // Событие до начала вычислений
        public event EventHandler? EventCompleted; // Событие после окончания вычислений
        // Соыбтие на каждой итерации цикла вычислений
        public event EventHandler<IntegralStepEventArgs>? EventStep; 
        
        public double Start { get => start; set => start = value; } // Начало диапазона
        public double End { get => end; set => end = value; } // Конец диапазона
        public int Steps { get => steps; set => steps = value; } // Количество разбиений

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName) 
                {
                    case "Start":
                        if (Start < -1 || Start > 1) 
                            error = "Некорректное значение. Значение должно быть в диапазоне от -1 до 1";
                        break;
                    case "End":
                        if (End < Start ) 
                            error = "Некорректное значение.";
                        break;
                    case "Steps":
                        if (Steps < 100) error = "Некорректное значение";
                        break;
                }
                return error;

            }
        }

        public Integral()
        {
        }

        public Integral(double start, double end, int steps) :this() 
        {
            Start = start;
            End = end;
            Steps = steps;
        }

        public override string ToString()
        {
            return $"Start: {Start}\nEnd: {End}\nSteps: {Steps}";
        }
        /// <summary>
        /// Вычисляет инетеграл методом прямоугольников.
        /// Возвращает результат через события
        /// </summary>
        public void Calculate()
        {
            EventBefore?.Invoke(this, new EventArgs());

            double h = (End - Start) / Steps;
            double S = 0;
            for (int i = 0; i < Steps; i++)
            {
                Thread.Sleep(100);
                double x = Start + i * h;
                S += func(x) * h;

                EventStep?.Invoke(this, new IntegralStepEventArgs(i+1, x, S));               
            }

            EventCompleted?.Invoke(this, new EventArgs());
        }

       

        /// <summary>
        /// Возвращает результат через выходной параметр.
        /// </summary>
        public void Calculate( out double result)
        {
            List<double> results =  new();
            EventHandler<IntegralStepEventArgs> SaveValue =
                (object? sender, IntegralStepEventArgs e) => results.Add(e.S);

            EventStep += SaveValue;

            Calculate();
            if(results.Any()) result = results.Last();
            else result = 0;

            EventStep -= SaveValue;
        }

        /// <summary>
        /// Вычисляет инетеграл методом прямоугольников.
        /// Возвращает результат в словаре асинхронно.
        /// </summary>
        public async IAsyncEnumerable<Dictionary<string, double>> CalculateAsync()
        {
            double h = (End - Start) / Steps;
            double S = 0;

            for (int i = 0; i < Steps; i++)
            {
                double x = Start + i * h;
                S += func(x) * h;
                await Task.Delay(100); // OR await Task.Yield(); 
                yield return new Dictionary<string, double>
                {
                    {"i", i+1},
                    {"S", S},
                    {"X", x },
                };
            }
        }

        /// <summary>
        /// Удаляет все обработчики у всех событий.
        /// </summary>
        public void ClearEventsHandlers()
        {
            EventBefore = null;
            EventStep = null;
            EventCompleted = null;
        }

    }
}
