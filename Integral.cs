using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVPP_CS_WPF_Lab6_Calculating_integral_Multi_threading_
{
    public class Integral: IDataErrorInfo
    {
        double start;
        double end;
        int steps;

        public Func<double,double> func = (x) => Math.Pow(x, 3);
        public event EventHandler? EventBefore;
        public event EventHandler? EventCompleted;
        public event EventHandler? EventtStep;
        
        public double Start { get => start; set => start = value; }
        public double End { get => end; set => end = value; }
        public int Steps { get => steps; set => steps = value; }

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
                        if (End < Start || End < 0) 
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

    }
}
