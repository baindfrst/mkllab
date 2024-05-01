using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mkllab
{
    public class SplineDataItem
    {
        public double X {  get; set; }
        public double Y { get; set; }
        public double Y_spline { get; set; }
        public SplineDataItem(double x, double y, double y_spline)
        {
            X = x;
            Y = y;
            Y_spline = y_spline;
        }

        public string ToLongString(string format)
        {
            return $"X: {X.ToString(format)}, Y: {Y.ToString(format)}, Y Spline: {Y_spline.ToString(format)}";
        }
        public override string ToString() { 
        return "X: " + X.ToString("0.000") + " Y: " + Y.ToString("0.000") + " Y_Spline: " + Y_spline.ToString("0.000");
                }
    }
}
