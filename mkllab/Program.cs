using mkllab;
using System.Runtime.InteropServices;
internal class main
{
    static void Main(string[] args)
    {
        FValues rez = func;
        V2DataArray testArray = new V2DataArray("key", DateTime.Now, 3, -5, 5, rez);
        SplineData splines = new SplineData(testArray, 5, 100);
        int inter = splines.CallSpline();
        Console.WriteLine(splines.ToLongString("0.000"));
        splines.Save("out.txt", "0.000");
    }
    static void func(double x, ref double y1, ref double y2)
    {
        y1 = x * x - 6;
        y2 = x - 1;
    }
}
delegate void FValues(double x, ref double y1, ref double y2);