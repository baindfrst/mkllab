using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace WPF2LabDop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TestData DataProg;
        public MainWindow()
        {
            InitializeComponent();
            DataProg = new TestData();
            DataContext = DataProg;

        }
        public class TestData : IDataErrorInfo
        {
            public double a { get; set; }
            public double b { get; set; }
            public double[] X { get; set; }
            public double[] Y { get; set; }
            public TestData()
            {
                this.X = new double[100];
                this.Y = new double[100];
                for (int i = 0; i < 100; i++)
                {
                    this.X[i] = this.a + ((this.b - this.a) / 99) * i;
                    this.Y[i] = Math.Sin(this.X[i]);
                }
            }
            public void recal()
            {
                for (int i = 0; i < 100; i++)
                {
                    this.X[i] = this.a + ((this.b - this.a) / 99) * i;
                    this.Y[i] = Math.Sin(this.X[i]);
                }
            }
            public string Error { get { return "Error text"; } }
            public string this[string property]
            {
                get
                {
                    string msg = null;
                    if (a >= b)
                    {
                        msg = "must be a < b";
                    }
                    return msg;
                }
            }
        }
        public void DrawBut(object sender, System.EventArgs e)
        {
            DataProg.recal();
            DrawPlot();
        }
        public void CanDrawBut(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Validation.GetHasError(ABox) == true || Validation.GetHasError(BBox) == true)
            {
                e.CanExecute = false; return;
            }
            else
            { e.CanExecute = true; return; }
        }
        private void DrawPlot()
        {
            PlotModel model = new PlotModel();
            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = DataProg.a,
                Maximum = DataProg.b,
                Title = "Coordinations"
            });
            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = DataProg.Y.Min(),
                Maximum = DataProg.Y.Max(),
                Title = "Value"
            });

            LineSeries sinfun = new LineSeries()
            {
                Title = "sin(X)",
                Color = OxyColors.Black,
            };
            for(int i = 0; i < 100; i++)
            {
                sinfun.Points.Add(new DataPoint(
                    DataProg.X[i],
                    DataProg.Y[i]
                    ));
            }

            model.Series.Add(sinfun);
            plot.Model = model;
        }
    }
    public static class CustomCommands
    {
        public static RoutedCommand DrawBut = new RoutedCommand("DrawBut", typeof(CustomCommands));
    }
}
