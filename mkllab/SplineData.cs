
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace mkllab
{
    internal class SplineData
    {
        V2DataArray DataArray { get; set; }
        int m { get; set; }
        double[] arraySpline { get; set; }
        int maxCountIter { get; set; }
        int stopCode { get; set; }
        double minNevuaz { get; set; }

        int CountEter {  get; set; }
        List<SplineDataItem> SplineProxRezult { get; set; }

        public SplineData(V2DataArray DataArray, int count_m, int countIter) {
            this.DataArray = DataArray;
            this.m = count_m;
            this.maxCountIter = countIter;
            this.SplineProxRezult = new List<SplineDataItem>();
        }

        public int CallSpline()
        {
            double[] splinerez = new double[m * 3];

            double[] y = new double[DataArray.x.Length];
            for (int i = 0; i < DataArray.x.Length; i++)
            {
                y[i] = DataArray.field[0,i];
            }
            double[] x = new double[DataArray.x.Length];
            for (int i = 0; i < DataArray.x.Length; i++)
            {
                x[i] = DataArray.x[i];
            }
            stopCode = CubicSpline(DataArray.x.Length, x, 1, y, m,splinerez);
            Console.WriteLine($"производная начала: {splinerez[2]}, производная в конце: {splinerez[m* 3 - 1]}");
            double y2 = 0;
            for (int i = 0; i < m; i++)
            {
                double y1 = 0;
                DataArray.f(DataArray.x[0] + ((DataArray.x[DataArray.x.Length - 1] - DataArray.x[0]) / (m - 1)) * i, ref y1, ref y2);
                SplineProxRezult.Add(new SplineDataItem(DataArray.x[0] + ((DataArray.x[DataArray.x.Length - 1] - DataArray.x[0]) /(m-1)) * i, y1, splinerez[i*3]));
            }
            double[] eps =
            {
                1.0E-12,
                1.0E-12,
                1.0E-12,
                1.0E-12,
                1.0E-12,
                1.0E-12,
            };
            double jac_eps = 1.0E-8;
            double res_initial = 0;
            double res_final = 0;
            int stop_criteria = 0;
            int[] check_data_info = new int[4];
            int iter_count = 0;
            int error_code = 0;
            int Nx = DataArray.x.Length;
            bool fin = TrustRegion(
                Nx,
                m,
                x,
                y,
                DataArray.f,
                eps,
                jac_eps,
                maxCountIter,
                maxCountIter/10,
                1,
                ref iter_count,
                ref res_initial,
                ref res_final,
                ref stop_criteria,
                check_data_info,
                ref error_code);
            
            Console.WriteLine("________________");
            minNevuaz = res_final;
            CountEter = iter_count;
            stopCode = stop_criteria;
            return 0;
        }

        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern int CubicSpline(int nX, double[] X, int nY, double[] Y, int Sn, double[] splineValues);
        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool TrustRegion(
        int n, // число независимых переменных
        int m, // число компонент векторной функции
        double[] x, // начальное приближение и решение
        double[] y,
        FValues funcY,
        double[] eps, // массив с 6 элементами, определяющих критерии
                      // остановки итерационного процесса
        double jac_eps, // точность вычисления элементов матрицы Якоби
        int niter1, // максимальное число итераций
        int niter2, // максимальное число итераций при выборе пробного шага

        double rs, // начальный размер доверительной области
        ref int ndoneIter, // число выполненных итераций
        ref double resInitial, // начальное значение невязки
        ref double resFinal, // финальное значение невязки
        ref int stopCriteria,// выполненный критерий остановки
        int[] checkInfo, // информация об ошибках при проверке данных

        ref int error); // информация об ошибках


        public string ToLongString(string format)
        {
            string ret = DataArray.ToLongString(format) + "\n";
            for (int i = 0; i < SplineProxRezult.Count; i++)
            {
                ret += SplineProxRezult[i].ToLongString(format) + "\n";
            }
            ret += '\n';
            ret += $"minNev =  {minNevuaz}";
            ret += $"\nStopCode = {stopCode}";
            ret += $"\n CountEter = {CountEter}";
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