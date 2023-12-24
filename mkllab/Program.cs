using mkllab;
using System.Runtime.InteropServices;
internal class main
{
    static void Main(string[] args)
    {
        FValues rez = func;
        V2DataArray testArray = new V2DataArray("key", DateTime.Now, 8, -5, 5, rez);
        for(int i = 0; i < 8; i++)
        {
            Console.WriteLine(testArray.field[0, i]);
        }
        SplineData splines = new SplineData(testArray, 8, 10);
        int inter = splines.CallSpline();
        Console.WriteLine(splines.ToLongString("0.000"));
    }
    static void func(double x, ref double y1, ref double y2)
    {
        y1 = x * x - 2 - 4;
        y2 = x - 1;
    }
}
delegate void FValues(double x, ref double y1, ref double y2);