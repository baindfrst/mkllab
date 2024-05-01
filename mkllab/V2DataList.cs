using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace mkllab
{
    public delegate DataItem FDI(double x);
    public class V2DataList : V2Data
    {
        int position = -1;
        public string thisClassInString { get { return this.ToLongString("f4"); } set { } }
        public List<DataItem> Data { get; set; }
        public V2DataList(string key, DateTime date) : base(key, date)
        {
            this.Data = new List<DataItem>();
        }
        public V2DataList(string key, DateTime date, double[] x, FDI F): base(key, date)
        {
            this.Data = new List<DataItem>();
            List<double> xWas = new List<double>();
            for(int i  = 0; i < x.Length; i++)
            {
                if (!xWas.Contains(x[i]))
                {
                    Data.Add(F(x[i]));
                    xWas.Add(x[i]);
                }
            }
        }
        override public double MinField
        {
            get
            {
                double minFiel = Data.Count() > 0 ? Data[0].doubles[0] : -1;
                for (int i = 0; i < Data.Count(); i++)
                {
                    if (Data[i].doubles[0] < minFiel)
                    {
                        minFiel = Data[i].doubles[0];
                    }
                    if (Data[i].doubles[1] < minFiel)
                    {
                        minFiel = Data[i].doubles[1];
                    }
                }
                return minFiel;

            }
        }
        public V2DataArray V2DataArray {
            get
            {
                V2DataArray ret = new V2DataArray(this.objectKey, this.timeData);
                ret.field = new double[2, Data.Count()];
                ret.x = new double[Data.Count()];
                for(int i = 0;i < this.Data.Count;i++) {
                    ret.x[i] = this.Data[i].x;
                    ret.field[0,i] = this.Data[i].doubles[0];
                    ret.field[1, i] = this.Data[i].doubles[1];
                }
                return ret;
            } 
        }

        public override DataItem xMaxItem
        {
            get
            {
                DataItem max = Data.Count > 0 ? Data[0] : new DataItem(-1, -1, -1);
                int ind = 0;
                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].x > max.x)
                    {
                        max = Data[i];
                    }
                }
                return Data.Count > 0 ? max: new DataItem(-1, -1, -1);
            }
        }


        public override string ToString()
        {
            return $"\nV2DataList:" +
                $"\n\tV2Data.objectKey: {this.objectKey}" +
                $"\n\tV2Data.timeData: {this.timeData}" +
                $"\n\tData.Count: {Data.Count()}";
        }
        override public string ToLongString(string format)
        {
            string outs = $"\nV2DataList:" +
                $"\n\tV2Data.objectKey: {this.objectKey}" +
                $"\n\tV2Data.timeData: {this.timeData}" +
                $"\n\tData.Count: {Data.Count()}" +
                $"\nData.vals:";
            for (int i = 0;i < Data.Count();i++)
            {
                outs += "\n\t" + Data[i].ToLongString(format);
            }
            return outs;
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            foreach (var ret in Data)
            {
                yield return ret;
            }
        }
    }
}
