
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace mkllab
{
    public class SplineData
    {
        public V2DataArray Data { get; }
        public List<double[]> Ysplines { get; }
        public int M { get; }
        public double[] YSpline { get; }
        public int MaxIter { get; }
        public int Status { get; set; }
        public double MinRes { get; set; }
        public int Msmall { get; set; }
        public List<SplineDataItem> SplineProxRezult { get; }
        public int CountIter { get; set; }

        public SplineData(V2DataArray data, int m, int maxIter, double MinREs, int Msmall)
        {
            MinRes = MinREs;
            this.Msmall = Msmall;
            Ysplines = new List<double[]>();
            SplineProxRezult = new List<SplineDataItem>();
            Data = data;
            M = m;
            YSpline = new double[data.x.Length];
            Status = 0;
            MaxIter = maxIter;
            CountIter = 0;
        }
      
        public void CallSpline()
        {
            try
            {
                int countIter = 0;
                int status = 0;
                double minRes = MinRes;
                double[] y = new double[Data.x.Length];
                for (int i = 0; i != Data.x.Length; i++)
                {
                    y[i] = Data.field[0, i];
                }
                int ret = CubicSpline(Data.x.Length,
                                      M,
                                      MaxIter,
                                      Data.x,
                                      y,
                                      YSpline,
                                      ref minRes,
                                      ref countIter,
                                      ref status);
                CountIter = countIter;
                Status = status;
                MinRes = minRes;
                double[] x_sup = new double[Msmall+5];
                double[] y_sup = new double[Msmall+5];
                for (int i = 0; i != Data.x.Length; i++)
                {
                    SplineProxRezult.Add(new SplineDataItem(Data.x[i], y[i], YSpline[i]));
                }
                for(int i = 0; i != Msmall+5; i++)
                {
                    double ns = 0;
                    x_sup[i] = Data.x[0] + ((Data.x[Data.x.Length - 1] - Data.x[0]) / (Msmall - 1)) * i;
                    Data.f(x_sup[i], ref y_sup[i], ref ns);
                }
                double[] Y_res_out = new double[Msmall*3];
                ret = CubicSplineSupport(Data.x.Length,
                                      Data.x,
                                      1,
                                      YSpline,
                                      Msmall,
                                      Data.typemesh,
                                      Y_res_out);
                for (int i = 0; i != Msmall*3; i++)
                {
                    Debug.WriteLine(Y_res_out[i]);
                }
                for (int i = 0; i != Msmall; i++)
                {
                    double[] elem = new double[2];
                    elem[0] = x_sup[i];
                    elem[1] = Y_res_out[i*3];
                    Ysplines.Add(elem);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Func Save. Exception: {ex.Message}\n");
            }
        }
        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern int CubicSplineSupport(
            int nX, // число узлов сплайна

            double[] X, // массив узлов сплайна

            int nY, // размерность векторной функции

            double[] Y,// массив заданных значений векторной функции

            int Sn,

            bool type,

            double[] splineValues); // массив вычисленных значений сплайна и производных

        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern
        int CubicSpline(
            int nX, int m, int maxIter,
            double[] X, double[] Y,
            double[] YSpline, ref double minRes, ref int countIter,
            ref int status);
        public string ToLongString(string format)
        {
            string ret = Data.ToLongString(format) + "\n";
            ret += "spline out: \n";
            for (int i = 0; i < SplineProxRezult.Count; i++)
            {
                ret += SplineProxRezult[i].ToLongString(format) + "\n";
            }
            ret += '\n';
            ret += $"minNev =  {MinRes}";
            ret += $"\nStopCode = {Status}";
            ret += $"\n CountEter = {CountIter}";
            return ret;
        }
        public void Save(string file, string format)
        {
            try
            {
                FileStream fs = File.Create(file);
                StreamWriter fileStream = new StreamWriter(fs);
                fileStream.WriteLine(this.ToLongString(format));
                fileStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Saving ended.");
            }
        }
    }
}