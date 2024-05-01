using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;  
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mkllab;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static WPFmklusr.MainWindow;

namespace WPFmklusr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class doublelisttostr : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double[] ret = new double[2];
                return ret;
            }
            catch { MessageBox.Show("Can`t convert doublelisttostr", "Error message", MessageBoxButton.OK, MessageBoxImage.Error); return -1; }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                {
                    return null;
                }
                double[] v = value as double[];
                return $"x: {v[0]} y: {v[1]}";
            }
            catch { MessageBox.Show("Can`t convert doublelisttostr", "Error message", MessageBoxButton.OK, MessageBoxImage.Error); return -1; }
        }
    }
    public class strtodouble : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble(value);
            }
            catch { MessageBox.Show("Can`t convert doublelisttostr", "Error message", MessageBoxButton.OK, MessageBoxImage.Error); return -1; }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value.ToString();
            }
            catch { MessageBox.Show("Can`t convert doublelisttostr", "Error message", MessageBoxButton.OK, MessageBoxImage.Error); return -1; }
        }
    }
    public class strtoint: IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Debug.WriteLine("WORK");
                return System.Convert.ToInt32(value);
            }
            catch { MessageBox.Show("Can`t convert doublelisttostr", "Error message", MessageBoxButton.OK, MessageBoxImage.Error); return -1; }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value.ToString();
            }
            catch { MessageBox.Show("Can`t convert doublelisttostr", "Error message", MessageBoxButton.OK, MessageBoxImage.Error); return -1; }
        }
    }

    public class TypeMeshToBool : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            Debug.WriteLine($"sdfsdfsdfsdfs - {str}");
            if (str != null)
            {
                bool mesh = false;
                if (str == "uniform")
                {
                    mesh = true;
                }
                return mesh;
            }
            else
            {
                return true;
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                bool val_bool = (bool)value;
                if(val_bool)
                {
                    return "uniform";
                }
                return "uneven";
                
            }
            else
            {
                return "uniform";
            }
        }
    }
public class ConverterLRBorders : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            double[] LRdouble = new double[2];
            if (str != null)
            {
                string[] LRB = str.Split(";");
                LRdouble[0] = System.Convert.ToDouble(LRB[0]);
                LRdouble[1] = System.Convert.ToDouble(LRB[1]);
                return LRdouble;
            }
            else
            {
                return LRdouble;
            }
        }

        public object Convert(double[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double[] LRdouble = value as double[];
                return LRdouble[0].ToString() + ";" + LRdouble[1].ToString();
            }
            else
            {
                return "";
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double[] LRdouble = value as double[];
                return $"{LRdouble[0].ToString()};{LRdouble[1].ToString()}";
            }
            else
            {
                return "";
            }
        }
    }
    public partial class MainWindow : Window
    {
        static void func(double x, ref double y1, ref double y2)
        {
            y1 = Math.Sin(x);
            y2 = x - 1;
        }
        static void func2(double x, ref double y1, ref double y2)
        {
            y1 = 2 * x * x + 4 * x - 6;
            y2 = x - 1;
        }
        static void func3(double x, ref double y1, ref double y2)
        {
            y1 = x - 2;
            y2 = x - 1;
        }
        static void func4(double x, ref double y1, ref double y2)
        {
            y1 = 3 * x * x - 2;
            y2 = x - 1;
        }
        public ViewData dataBack;
        public MainWindow()
        {
            dataBack = new ViewData();
            InitializeComponent();
            functioninp.SelectedIndex = 0;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataBack = new ViewData();
            functioninp.ItemsSource = dataBack.funclist;
            DataContext = dataBack;
        }
        public class ViewData: IDataErrorInfo
        {
            public double[] LRBorderscl { get; set; }
            public int CountNcl { get; set; }
            public bool typeofmeshcl { get; set; }
            public ObservableCollection<string> funclist;
            public ObservableCollection<FValues> fValueslist;
            public int countMeshKnotcl { get; set; }
            public int countSmollestMeshKnotcl { get; set; }
            public double minNevcl { get; set; }
            public int countItercl { get; set; }
            public bool loadedDatacl { get; set; }

            public mkllab.V2DataArray DataArray { get; set; }
            public mkllab.SplineData SplineData { get; set; }
            public List<SplineDataItem> splinedatageter1 {get{ if (SplineData != null) return SplineData.SplineProxRezult; else return null; }set { }}
            public List<double[]> splinedatageter2 { get { if (SplineData != null) return SplineData.Ysplines; else return null; } set { } }

            public string Error { get { return "Error text"; } }
            public string this[string property]
            {
                get
                {
                    string msg = null;
                    Debug.WriteLine("ANDFIHIUADFHIUADFHPIUADFH");
                    switch (property)
                    {
                        case "LRBorderscl":
                            if (LRBorderscl[0] >= LRBorderscl[1])
                            {
                                msg = "ошибка в границах";
                            }
                            break;
                        case "CountNcl":
                            if (CountNcl < 3)
                            {
                                msg = "кол-во узлов дискретных значений должно быть больше 2";
                            }
                            break;
                        case "countSmollestMeshKnotcl":
                            if (countSmollestMeshKnotcl < 3)
                            {
                                msg = "кол-во узлов равномерной сетки должно быть больше 2";
                            }
                            break;
                        case "countMeshKnotcl":
                            if (countMeshKnotcl > CountNcl || countMeshKnotcl < 1)
                            {
                                msg = "кол-во узлов сглаживающего сплайна должно быть больше 1 и меньше или равно кол-ва узлов дискретных данных";
                            }
                            break;

                        default:
                                break;
                    }
                    return msg;
                }
            }

            public void Save(string filename) 
            {
                this.DataArray.Save(filename);
            }
            public void Load(string filename) 
            {
                this.loadedDatacl = true;
                mkllab.V2DataArray DataArrayTime = new V2DataArray("var", DateTime.Now);
                mkllab.V2DataArray.Load(filename, ref DataArrayTime);
                this.DataArray = DataArrayTime;
                this.LRBorderscl[0] = this.DataArray.x[0];
                this.LRBorderscl[1] = this.DataArray.x[this.DataArray.x.Length - 1];
                this.CountNcl = this.DataArray.x.Length;
                this.typeofmeshcl = this.DataArray.typemesh;
                this.DataArray.f = this.fValueslist[this.funclist.IndexOf(this.DataArray.fonstr)];
            }
            public ViewData()
            {
                LRBorderscl = new double[2];
                //LRBorderscl[0] = 0;
                //LRBorderscl[1] = 0;
                //countMeshKnotcl = 0;
                //minNevcl = 0;
                //countItercl = 0;
                funclist = new ObservableCollection<string>();
                fValueslist = new ObservableCollection<FValues>();
                funclist.Add("x * x - 6");
                funclist.Add("2 * x * x + 4 * x - 6");
                funclist.Add("x - 2");
                funclist.Add("3 * x * x - 2");
                fValueslist.Add(func);
                fValueslist.Add(func2);
                fValueslist.Add(func3);
                fValueslist.Add(func4);
                loadedDatacl = false;
            }
            public ViewData(double[] borders, int N, bool mesh, int countMeshKnot, int countSmollestMeshKnot) 
            {
                LRBorderscl = new double[2];
                LRBorderscl[0] = borders[0];
                LRBorderscl[1] = borders[1];
                CountNcl = N;
                typeofmeshcl = mesh;
                this.countMeshKnotcl = countMeshKnot;
                this.countSmollestMeshKnotcl = countSmollestMeshKnot;
                this.DataArray = new V2DataArray("var", DateTime.Now);
            }
            public void CreateDataArray(FValues function, object funcstr) 
            {
                string functionstr = funcstr as string;
                Debug.WriteLine("entry");
                if (typeofmeshcl == true)
                {
                    this.DataArray = new V2DataArray("V2DataArray", DateTime.Now, CountNcl, LRBorderscl[0], LRBorderscl[1], function, functionstr);
                    Debug.WriteLine(DataArray.fonstr);
                    Debug.WriteLine(countItercl);
                    Debug.WriteLine(LRBorderscl[0]);
                    Debug.WriteLine(LRBorderscl[1]);
                }
                else
                {
                    double[] mesharray = new double[this.CountNcl];
                    Debug.WriteLine($"count - {this.CountNcl}");
                    string showa = "";
                    for (int i = 0; i < CountNcl; i++)
                    {
                        Random random = new Random();
                        mesharray[i] = random.NextDouble() * (this.LRBorderscl[1] - this.LRBorderscl[0]) + this.LRBorderscl[0];
                        showa += mesharray[i].ToString() + " ";
                    }
                    Array.Sort(mesharray);
                    
                    this.DataArray = new V2DataArray("V2DataArray", DateTime.Now, mesharray, function, functionstr);
                }
                loadedDatacl = true;
            }
            public void CreateSplineData() 
            {
                this.SplineData = new mkllab.SplineData(this.DataArray, countMeshKnotcl, countItercl, minNevcl, countSmollestMeshKnotcl);
                this.SplineData.CallSpline();
            }
        }

        private void SaveClick(object sender, System.EventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                
                dataBack.Save(filename);
            }
        }
        private void LoadDataFromFileClick(object sender, System.EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                try
                {
                    // Open document
                    string filename = dlg.FileName;
                    dataBack.Load(filename);
                    functioninp.SelectedIndex = dataBack.funclist.IndexOf(dataBack.DataArray.fonstr);
                    BindingOperations.GetBindingExpression(LRBordersTextBox, TextBox.TextProperty).UpdateTarget();
                    BindingOperations.GetBindingExpression(CountN, TextBox.TextProperty).UpdateTarget();
                    BindingOperations.GetBindingExpression(typeofmesh, ComboBox.TextProperty).UpdateTarget();
                    //LRBordersTextBox.Text = dataBack.DataArray.x[0].ToString() + ";" + dataBack.DataArray.x[dataBack.DataArray.x.Length - 1].ToString();
                    //CountN.Text = dataBack.DataArray.x.Length.ToString();
                    //dataBack.DataArray.f = fValueslist[funclist.IndexOf(dataBack.DataArray.fonstr)];
                    //if (dataBack.DataArray.typemesh)
                    //{
                    //    typeofmesh.SelectedIndex = 0; 
                    //}
                    //else
                    //{
                    //    typeofmesh.SelectedIndex = 1;
                    //}
                    //functioninp.SelectedIndex = funclist.IndexOf(dataBack.DataArray.fonstr);

                    // SplineDataItemOut.ItemsSource = dataBack.SplineData.ToLongString("0.000");
                }
                catch 
                {
                    surend_error("error load data from file");
                }
            }
            else
            {
                surend_error("file not found");
            }
        }
        private void LoadDataFromControls(object sender, System.EventArgs e)
        {
            try
            {
                if (dataBack.LRBorderscl[0] >= dataBack.LRBorderscl[1]) { surend_error("error input"); return; }
                if (dataBack.LRBorderscl[0] == double.NaN || dataBack.LRBorderscl[1] == double.NaN || dataBack.CountNcl <2) { surend_error("error input"); return; }

                dataBack.CreateDataArray(dataBack.fValueslist[functioninp.SelectedIndex], functioninp.SelectedItem);
                Debug.WriteLine(dataBack.DataArray.ToLongString("f4"));
                //MessageBox.Show(dataBack.LRBorderscl[0].ToString() +" "+ dataBack.LRBorderscl[1].ToString()+" "+  dataBack.CountNcl.ToString() +" "+ dataBack.typeofmeshcl.ToString());
            }
            catch 
            {
                surend_error("error on getting data from control");
            }
        }

        private void Execut_Click(object sender, RoutedEventArgs e)
        {
            if (dataBack.loadedDatacl)
            {
                try
                {
                    if(int.Parse(countMeshKnot.Text) < 0 || int.Parse(maxCountIter.Text) < 0 || double.Parse(normNevaz.Text) < 0) { surend_error("spline input error"); return; }
                    dataBack.CreateSplineData();
                    //SplineDataItemOut.ItemsSource = dataBack.SplineData.SplineProxRezult;
                    SplineDataItemOut.ItemsSource = dataBack.SplineData.SplineProxRezult;
                    SplineDataDoubleMeshOut.ItemsSource = dataBack.splinedatageter2;
                    DrawPlot();
                }
                catch
                {
                    surend_error("error creating spline");
                }
            }
            else
            {
                surend_error("data not loaded");
            }    
        }

        private void DataFromFile_Click(object sender, RoutedEventArgs e)
        {
            LoadDataFromFileClick(sender, e);
        }

        private void DataFromControls_Click(object sender, RoutedEventArgs e)
        {
            LoadDataFromControls(sender, e);
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
            if (dataBack.loadedDatacl == false)
            {
                surend_error("data not init");
            }
            else
            {
                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    try
                    {
                        string filename = dlg.FileName;
                        dataBack.Save(filename);
                        MessageBox.Show("saved");
                    }
                    catch
                    {
                        surend_error("save failed!!!");
                    }

                }
                else
                {
                    surend_error("file not found");
                }
            }
        }

        public void surend_error(string msg)
        {
            MessageBox.Show(msg, "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if(Validation.GetHasError(LRBordersTextBox) == true || Validation.GetHasError(CountN) == true || Validation.GetHasError(countSplineKnot) == true || Validation.GetHasError(countMeshKnot) == true || dataBack.loadedDatacl == false)
            {
                e.CanExecute = false;
                return; 
            }
            else { e.CanExecute = true; return;}
        }
        private void CanSaveHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataBack != null)
            {
                if (dataBack.loadedDatacl != true)
                {
                    e.CanExecute = false;
                    return;
                }
                else { e.CanExecute = true; return; }
            }
            else
            { e.CanExecute= false; return; }
        }
        private void DrawPlot()
        {
                PlotModel model = new PlotModel();
                model.Axes.Add(new LinearAxis()
                {
                    Position = AxisPosition.Bottom,
                    Minimum = dataBack.LRBorderscl[0],
                    Maximum = dataBack.LRBorderscl[1],
                    Title = "Coordinations"
                });
                model.Axes.Add(new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    Minimum = dataBack.SplineData.SplineProxRezult.Min(x => x.Y),
                    Maximum = dataBack.SplineData.SplineProxRezult.Max(x => x.Y),
                    Title = "Value"
                });

                LineSeries rawline = new LineSeries()
                {
                    Title = "RawData",
                    Color = OxyColors.Transparent,
                    MarkerType = MarkerType.Circle,
                    MarkerFill = OxyColors.Black
                };
                for (int i = 0; i < dataBack.CountNcl; i++)
                {
                    rawline.Points.Add(new DataPoint(dataBack.DataArray.x[i], dataBack.DataArray.field[0, i]));
                }

                LineSeries splineline = new LineSeries()
                {
                    Title = "SplineData",
                    Color = OxyColors.Green,
                };
                foreach (double[] elem in dataBack.SplineData.Ysplines)
                {
                    splineline.Points.Add(new DataPoint(
                        elem[0],
                        elem[1]
                        ));
                }

                model.Series.Add(splineline);
                model.Series.Add(rawline);
                plot.Model = model;
        }
    }
    public static class CustomCommands
    {
        public static RoutedCommand SaveCom = new RoutedCommand("SaveCom", typeof(CustomCommands));
        public static RoutedCommand ExecuteCom = new RoutedCommand("ExecuteCom", typeof(CustomCommands));
    }
}
