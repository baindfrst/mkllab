using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Asn1;
using System.Text.Json;

namespace mkllab
{
    public delegate void FValues(double x, ref double y1, ref double y2);
    public class V2DataArray : V2Data
    {
        int position = -1;
        public double[] x { get; set; }
        public double[,] field { get; set; }
        public FValues f {  get; set; }
        public string fonstr { get; set; }
        public bool typemesh { get; set; }
        public string thisClassInString {  get { return this.ToLongString("f4"); } set { } }

        public V2DataArray(string key, DateTime date) : base(key, date)
        {
            this.x = new double[0];
            this.field = new double[2, 0];

        }
        public V2DataArray(string key, DateTime date, double[] x, FValues F, string fonstr) : base(key, date)
        {
            f = F;
            this.x = (double[])x.Clone();
     
            this.field = new double[2, x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                F(x[i], ref this.field[0, i], ref this.field[1, i]);
            }
            this.fonstr = fonstr;
            this.typemesh = false;
        }
        public V2DataArray(string key, DateTime date, int nX, double xL, double xR, FValues F, string fonstr) : base(key, date)
        {
            f = F;
            this.x = new double[nX];
            this.field = new double[2, x.Length];
            for (int i = 0; i < nX; i++)
            {
                x[i] = xL + ((xR - xL) / (nX - 1)) * i;
                F(x[i], ref this.field[0, i], ref this.field[1, i]);
            }
            this.fonstr = fonstr;
            this.typemesh = true;
        }
        public double[] this[int index] 
        {
            get
            {
                double[] ret = new double[this.field.GetLength(index)];
                for(int i = 0; i < this.field.GetLength(index); i++)
                {
                    ret[i] = this.field[index, i];
                }
                return ret;
            }
        }
        override public double MinField
        {
            get
            {
                double minFiel = field.Length > 0 ? field[0,0] : -1;          
                for (int i = 0; i < x.Length; i++)
                {
                    if (field[0,i] < minFiel)
                    {
                        minFiel = field[0, i];
                    }
                    if (field[1,i] < minFiel)
                    {
                        minFiel = field[1, i];
                    }
                }
                return minFiel;
            }
        }

        public override DataItem xMaxItem { get
            {
                double max = x.Length > 0 ? x[0] : -1;
                int ind = 0;
                for(int i =0; i<x.Length;i++)
                {
                    if (x[i] > max)
                    {
                        max = x[i];
                        ind = i;
                    }
                }
                return x.Length > 0 ? new DataItem(x[ind], field[0, ind], field[1, ind]) : new DataItem(-1, -1, -1);
            }
        } 
        public override string ToString()
        {
            return $"\nV2DataArray:" +
                $"\n\tbase.key = {this.objectKey}" +
                $"\n\tbase.date = {this.timeData}";
        }
        override public string ToLongString(string format)
        {
            string outs = $"\nV2DataArray:" +
                $"\n\tV2Data.objectKey: {this.objectKey}" +
                $"\n\tV2Data.timeData: {this.timeData}" +
                $"\n\tfields.vals:";
            for (int i = 0; i < x.Length; i++)
            {
                outs += $"\n x = {x[i]}, y1 = {field[0,i]}, y2 = {field[1, i]}";
            }
            return outs;
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            for (int i = 0; i < x.Length; i++)
            {
                yield return new DataItem(x[i], field[0, i], field[1, i]);
            }
        }

        public bool Save(string filename)
        {
            try
            {
                StreamWriter filestream = new StreamWriter(File.Open(filename, FileMode.Create));
                filestream.WriteLine(this.objectKey);
                filestream.WriteLine(this.timeData);
                filestream.WriteLine(this.x.Length);
                for(int i = 0;i < this.x.Length;i++)
                {
                    filestream.WriteLine(this.x[i]);
                    filestream.WriteLine(this.field[0, i]);
                    filestream.WriteLine(this.field[1, i]);
                }
                filestream.WriteLine(this.fonstr);
                filestream.WriteLine(this.typemesh);
                filestream.Close();
            } 

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File save end");
            }
            return true;
        }

        public static bool Load(string filename, ref V2DataArray obj)
        {
            try
            {
                StreamReader streamfile = new StreamReader(filename);
                obj.objectKey = streamfile.ReadLine();
                obj.timeData = Convert.ToDateTime(streamfile.ReadLine());
                int lenField = Convert.ToInt32(streamfile.ReadLine());
                obj.x = new double[lenField];
                obj.field = new double[2, lenField];
                for(int i = 0; i < lenField;i++)
                {
                    obj.x[i] = Convert.ToDouble(streamfile.ReadLine());
                    obj.field[0, i] = Convert.ToDouble(streamfile.ReadLine());
                    obj.field[1, i] = Convert.ToDouble(streamfile.ReadLine());
                }
                obj.fonstr = streamfile.ReadLine();
                obj.typemesh = Convert.ToBoolean(streamfile.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            finally { Console.WriteLine("Read File and write Class complete"); }
            return true;
        }
    }

    
}
