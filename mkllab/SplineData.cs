
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
            double[] splinerez = new double[m];
            int numEter = 0;

            stopCode = CubicSpline(m, DataArray.x, 1, DataArray[0],splinerez);
            minNevuaz = 0;
            for (int i = 0; i < m; i++)
            {
                minNevuaz += (splinerez[i] - DataArray.field[0, i]) * (splinerez[i] - DataArray.field[0, i]);
                SplineProxRezult.Add(new SplineDataItem(DataArray.x[i], DataArray.field[0, i], splinerez[i]));
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
            USRFCNXD del = func;
            double jac_eps = 1.0E-8;
            double res_initial = 0;
            double res_final = 0;
            int stop_criteria = 0;
            int[] check_data_info = new int[4];
            int iter_count = 0;
            int error_code = 0;
            bool fin = TrustRegion(m, m, DataArray.x, DataArray[0], eps, jac_eps, maxCountIter, maxCountIter, 100, ref iter_count, ref res_initial, ref res_final, ref stop_criteria, check_data_info, ref error_code, del);
            Console.WriteLine(error_code); Console.WriteLine(stop_criteria);
            Console.WriteLine(res_initial); Console.WriteLine(res_final);
            minNevuaz = res_final;
            CountEter = iter_count;
            return 0;
        }

        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern int CubicSpline(int nX, double[] X, int nY, double[] Y, double[] splineValues);
        [DllImport("..\\..\\..\\..\\x64\\DEBUG\\Dll1.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool TrustRegion(
        int n, // число независимых переменных
        int m, // число компонент векторной функции
        double[] x, // начальное приближение и решение
        double[] y, //значения поля У
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
        ref int error,  // информация об ошибках
        USRFCNXD funcinp); // функция для Якоби

        //    public static extern bool TrustRegion(
        //int n, // число независимых переменных
        //int m, // число компонент векторной функции
        //double[] x, // начальное приближение и решение
        //USRFCND FValues, // указатель на функцию, вычисляющую векторную
        //                 // функцию в заданной точке
        //double[] eps, // массив с 6 элементами, определяющих критерии
        //              // остановки итерационного процесса
        //double jac_eps, // точность вычисления элементов матрицы Якоби
        //int niter1, // максимальное число итераций
        //int niter2, // максимальное число итераций при выборе пробного шага

        //double rs, // начальный размер доверительной области
        //ref int ndoneIter, // число выполненных итераций
        //ref double resInitial, // начальное значение невязки
        //ref double resFinal, // финальное значение невязки
        //ref int stopCriteria,// выполненный критерий остановки
        //int[] checkInfo, // информация об ошибках при проверке данных

        //ref int error); // информация об ошибках



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
        static void func(int m, int n, double[] x, double[] f, double[] y)
        {
            for (int i = 0; i < 0; i++)
            {
                f[i] = y[i];
            }
        }
    }
}
delegate void USRFCNXD(int m, int n, double[] x, double[] f, double[] y);
delegate void USRFCND(int m, int n, double[] x, double[] f);