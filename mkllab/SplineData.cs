
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
            int numEter = 0;

            double[] y = new double[DataArray.x.Length];
            for (int i = 0; i < m; i++)
            {
                y[i] = DataArray.field[0,i];
            }
            double[] x = new double[DataArray.x.Length];
            for (int i = 0; i < m; i++)
            {
                x[i] = DataArray.x[i];
            }
            stopCode = CubicSpline(DataArray.x.Length, x, 1, y, m, x[0], x[DataArray.x.Length - 1],splinerez);
            Console.WriteLine($"производная начала: {splinerez[2]}, производная в конце: {splinerez[splinerez.Length - 1]}");
            minNevuaz = 0;
            for (int i = 0; i < m; i++)
            {
                minNevuaz += (splinerez[i] - DataArray.field[0, i]) * (splinerez[i] - DataArray.field[0, i]);
                SplineProxRezult.Add(new SplineDataItem(DataArray.x[i], y[i], splinerez[i*3]));
                Console.WriteLine(DataArray.x[i]);
                Console.WriteLine(splinerez[i*3]);
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
                eps,
                jac_eps,
                maxCountIter,
                maxCountIter,
                10,
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
            Console.WriteLine(minNevuaz);
            Console.WriteLine(DataArray);
            Console.WriteLine(DataArray.ToLongString("0.000"));
            Console.WriteLine("oas");
            return 0;
        }

        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern int CubicSpline(int nX, double[] X, int nY, double[] Y, int Sn, double Lx, double Rx, double[] splineValues);
        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        //public static extern bool TrustRegion(
        //int n, // число независимых переменных
        //int m, // число компонент векторной функции
        //double[] x, // начальное приближение и решение
        //double[] y, //значения поля У
        //double[] eps, // массив с 6 элементами, определяющих критерии
        //// остановки итерационного процесса
        //double jac_eps, // точность вычисления элементов матрицы Якоби
        //int niter1, // максимальное число итераций
        //int niter2, // максимальное число итераций при выборе пробного шага

        //double rs, // начальный размер доверительной области
        //ref int ndoneIter, // число выполненных итераций
        //ref double resInitial, // начальное значение невязки
        //ref double resFinal, // финальное значение невязки
        //ref int stopCriteria,// выполненный критерий остановки
        //int[] checkInfo, // информация об ошибках при проверке данных
        //ref int error,  // информация об ошибках
        //USRFCNXD funcinp); // функция для Якоби

        public static extern bool TrustRegion(
        int n, // число независимых переменных
        int m, // число компонент векторной функции
        double[] x, // начальное приближение и решение
        double[] y,
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
    }
}
delegate void USRFCNXD(int m, int n, double[] x, double[] f, double[] y);
delegate void USRFCND(int m, int n, double[] x, double[] f);