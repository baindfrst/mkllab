using mkllab;
using System.Runtime.InteropServices;
internal class main
{
    static void Main(string[] args)
    {
        FValues rez = func;
        V2DataArray testArray = new V2DataArray("key", DateTime.Now, 3, -5, 5, rez, "x * x - 5 * x - 5");
        testArray.Save("C:\\Users\\main\\source\\repos\\mkllab\\DataArrayFile.txt");
    }
    static void func(double x, ref double y1, ref double y2)
    {
        y1 = x * x - 5 * x - 5;
        y2 = x - 1;
    }
}