using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ADBaseLibrary.AdobeHDS
{
    public class AmfReader : BinaryReader
    {
        public AmfReader(Stream s) : base(s)
        {

        }
        //Only objects needed are decoded do not re-use.

        public T ReadObject<T>() where T : class,new()
        {
            T data = new T();
            int tp = ReadByte();
            if (tp != 2)
                return null;
            string type = EReadString();
            tp = ReadByte();
            if (tp != 8)
                return null;

            int cnt = EReadInt32();
            for (int x = 0; x < cnt; x++)
            {
                string propname = EReadString();
                object value=null;
                tp = ReadByte();
                switch (tp)
                {
                    case 0:
                        byte[] dta = ReadBytes(8);
                        Array.Reverse(dta);
                        value = BitConverter.ToDouble(dta, 0);
                        break;
                    case 1:
                        byte b = ReadByte();
                        value = b == 1;
                        break;

                }
                PropertyInfo prop = data.GetType().GetProperty(propname);
                if (prop!=null && prop.CanWrite)
                {
                    prop.SetValue(data, value, null);
                }
            }
            return data;
        }
        public int EReadInt32()
        {
            byte[] dta = ReadBytes(4);
            Array.Reverse(dta);
            return BitConverter.ToInt32(dta, 0);
        }
        public int EReadInt16()
        {
            int result = 0;
            for (int x = 0; x < 2; x++)
                result = result << 8 | ReadByte();
            return result;
        }

        public string EReadString()
        {
            int b= EReadInt16();
            byte[] data=ReadBytes(b);
            return Encoding.ASCII.GetString(data);
        }
    }
}
