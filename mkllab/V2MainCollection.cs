using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net.Sockets;

namespace mkllab
{
    public class V2MainCollection : System.Collections.ObjectModel.ObservableCollection<V2Data>
    {
        public bool Contains(string key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].objectKey == key)
                    return true;
            }
            return false;
        }
        public bool Add(V2Data v2Data)
        {
            if (this.Contains(v2Data.objectKey))
            {
                return false;
            }
            this.Insert(this.Count, v2Data);
            return true;
        }
        public V2MainCollection(int nV2DataArray, int nV2DataList)
        {
            void test(double x, ref double y1, ref double y2) { y1 = x + 1; y2 = x + 1; }
            for (int i = 0; i < nV2DataArray; i++)
            {
                double[] inp = new double[i + 1];
                Random rand = new Random();
                for (int j = 0; j < i + 1; j++)
                {
                    inp[j] = rand.NextDouble();
                }
                Console.WriteLine(this.Add(new V2DataArray("Arr" + i.ToString(), DateTime.Now, inp, test, "x + 1")));
            }
            for (int i = 0; i < nV2DataList; i++)
            {
                double[] inp = new double[i + 1];
                Random rand = new Random();
                for (int j = 0; j < i + 1; j++)
                {
                    inp[j] = rand.NextDouble();
                }
                Console.WriteLine(this.Add(new V2DataList("List" + i.ToString(), DateTime.Now, inp, (x) => new DataItem(x, x, x))));
            }
        }
        public string ToLongString(string format)
        {
            string ret = "";
            for (int i = 0; i < this.Count; i++)
            {
                ret += this[i].ToLongString(format);
            }
            return ret;
        }

        public List<DataItem> xMaxItem
        {
            get
            {
                List<DataItem> ret = new List<DataItem>();
                for (int i = 0; i < this.Count; i++)
                {
                    ret.Add(this[i].xMaxItem);
                }
                return ret;
            }
        }

        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < this.Count; ++i)
            {
                ret += this[i].ToString();
            }
            return ret;
        }

        public int MaxZeroFieldFromCollection
        {
            get
            {
                if (this.Count < 0)
                {
                    return -1;
                }
                else
                {
                    var answ = (from data in this.Items select data).Where(Item => vectorize(Item.Select(Item => Item.doubles[0]).Sum(), Item.Select(Item => Item.doubles[1]).Sum()) == 0);
                    return (from classes in answ select classes.Count()).Max();
                }
            }
        }
        public DataItem? MaxVecFromCollection
        {
            get
            {
                if(this.Count < 0)
                {
                    return null;
                }
                else
                {
                    return (from data in this.Items from dataItem in data select dataItem).MaxBy(Item => Math.Abs(vectorize(Item.doubles[0], Item.doubles[1])));

                }
            }
        }

        public string Thisstring { get { return this.ToLongString("0.000"); } set { } }

        public IEnumerable<double> MaxUniqResultFromCollection
        {
            get
            {
                if(this.Count < 0)
                {
                    return null;
                }
                else
                return (from data in this.Items from dataItem in data select dataItem.x).Where(Item => (from Items in this.Items from dataItems in Items select dataItems.x).Count(x => x == Item) == 1).OrderBy(Item => Item);
            }
        }

        public IEnumerable<double> V2DArrayLes
        {
            get
            {
                if (this.Count < 0)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine((from data in this.Where(Item => Item is V2DataList) from dataList in data select dataList.x).Min());
                    Console.WriteLine("_____");
                    return (from data in this.Where(Item => Item is V2DataArray) from dataArray in data select dataArray.x).Where(Item => Item < (from data in this.Where(Item => Item is V2DataList) from dataList in data select dataList.x).Min());
                }
            }
        }

        double sqrt(double x) => x * x;
        double vectorize(double x, double y) => Math.Pow(sqrt(x) + sqrt(y), 0.5);

    }
}
