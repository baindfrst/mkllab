using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mkllab
{
    public abstract class V2Data : IEnumerable<DataItem>
    {
        public string objectKey { get; set; }
        public DateTime timeData { get; set; }

        public abstract DataItem xMaxItem { get; }

        public V2Data(string objectKey, DateTime timeData)
        {
            this.objectKey = objectKey;
            this.timeData = timeData;
        }

        abstract public double MinField { get; }
        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return base.ToString();
        }
        public abstract string ToLongString(string format);
    }
}
