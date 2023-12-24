using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mkllab
{
    internal struct SplineDataItem
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
        return X.ToString() + " " + Y.ToString() + " " + Y_spline.ToString();
                }
    }
}
