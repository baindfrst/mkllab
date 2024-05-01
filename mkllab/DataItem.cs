using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mkllab
{
    public class DataItem
    {
        public double x { get; set; }
        public double[] doubles { get; set; }
        public DataItem(double x, double y1, double y2)
        {
            this.x = x;
            doubles = new double[] { y1, y2 };
        }
        public string ToLongString(string format)
        {
            return $"x: {x} y1: {doubles[0]} y2: {doubles[1]}";
        }
        public override string ToString() { return ToLongString("0.00"); }

    }


}
