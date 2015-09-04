using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ADBaseLibrary.Matroska
{
    public static class MatroskaExtensions
    {
        private static readonly int[] FfLog2Tab ={
            0,0,1,1,2,2,2,2,3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
            5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
            6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
            6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
            7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
            7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
            7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
            7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7
        };

        private static IOException GenIo(string message, BinaryReader stream, params object[] pars)
        {
            long pos = stream.BaseStream.Position;
            object[] obj = new object[pars.Length + 1];
            Array.Copy(pars,0,obj,1,pars.Length);
            obj[0] = pos;
            return new IOException(string.Format(message,obj));
        }

        public static int Ebml_Read_Id(this BinaryReader reader, int maxSize, out ulong number)
        {
            int read = reader.Ebml_Read_Num(maxSize, out number);
            if (read >= 0)
                number |= 1UL << 7*read;
            return read;

        }
        public static int Ebml_Read_Num(this BinaryReader reader, int maxSize, out ulong number)
        {
            int n = 1;
            ulong total = reader.ReadByte();
            if (total==0)
                throw GenIo("Read error at pos {0} ({0:X})\n",reader);
            int read = 8 - FfLog2Tab[total];
            if (read > maxSize)
                throw GenIo("Invalid EBML number size tag 0x{1:X} at pos {0} ({0:X})\n", reader, total);
            total ^= (ulong)(1 << FfLog2Tab[total]);
            while (n++ < read)
            {
                byte b = reader.ReadByte();
                total = (total << 8) |  b;
            }
            number = total;
            return read;
        }

        public static int Ebml_Read_Length(this BinaryReader reader, out ulong number)
        {
            int res = Ebml_Read_Num(reader, 8, out number);
            if (res > 0 && number + 1 == 1UL << (7 * res))
                number = 0xffffffffffffffUL;
            return res;
        }

        public static int Ebml_Read_Uint(this BinaryReader reader, int size, out ulong number)
        {
            int n = 0;
            if (size > 8)
                throw new ArgumentException("Invalid size");            
            number = 0;
            while (n++ < size)
            {
                byte b = reader.ReadByte();
                number = (number << 8) |  b;
            }
            return 0;
        }
        public static int Ebml_Read_Sint(this BinaryReader reader, int size, out long number)
        {
            int n = 1;

            if (size > 8)
                throw new ArgumentException("Invalid size");
            if (size == 0)
            {
                number = 0;
            }
            else
            {
                number = (sbyte)reader.ReadByte();
                while (n++ < size)
                {
                    byte b = reader.ReadByte();
                    number = (number << 8) | b;
                }
            }

            return 0;
        }
        public static int Ebml_Read_Float(this BinaryReader reader, int size, out double num)
        {
            if (size == 0)
                num = 0;
            else if (size == 4)
            {
                byte[] floatBytes = reader.ReadBytes(4);
                Array.Reverse(floatBytes);
                num = BitConverter.ToSingle(floatBytes, 0);
            }
            else if (size == 8)
            {
                byte[] dBytes = reader.ReadBytes(8);
                Array.Reverse(dBytes);
                num = BitConverter.ToDouble(dBytes, 0);
            }
            else
                throw new ArgumentException("Invalid size");
            return 0;
        }
        public static int Ebml_Read_String(this BinaryReader reader, int size, out string str)
        {
            byte[] b = reader.ReadBytes(size);
            str = Encoding.UTF8.GetString(b);
            return 0;
        }
        public static int Ebml_Read_Binary(this BinaryReader reader, int length, out byte[] bin)
        {
            bin = reader.ReadBytes(length);
            return 0;
        }

        public static void CopyBytes(this BinaryReader reader, UInt64 length, BinaryWriter writer, HashAlgorithm algo=null)
        {

            byte[] buffer = new byte[32768];
            int read;
            int max = (int)(length < 32768 ? length : 32768);
            while ((read = reader.BaseStream.Read(buffer, 0, max)) > 0)
            {
                writer.BaseStream.Write(buffer, 0, read);
                algo?.TransformBlock(buffer, 0, read, buffer, 0);
                length -= (ulong)read;
                max = (int)(length < 32768 ? length : 32768);
            }
        }
        public static int NumSize(ulong num)
        {
            int bytes = 1;
            while (((num + 1) >> (bytes * 7)) != 0)
                bytes++;
            return bytes;
        }
        public static int IdSize(ulong id)
        {
            ulong mask = (id + 1) | 1;
            int index = 0;
            while (mask > 1)
            {
                mask >>= 1;
                index++;
            }
            return (index-1)/7 +1;
        }

        public static int UintSize(ulong val)
        {
            int bytes = 1;
            while ((val >>= 8) > 0)
                bytes++;
            return bytes;
        }
        public static int SintSize(long val)
        {
            int bytes = 1;
            ulong tmp = (ulong)(2 * (val < 0 ? val ^ -1 : val));
            while ((tmp >>= 8) > 0)
                bytes++;
            return bytes;
        }
        public static void Ebml_Write_Num(this BinaryWriter writer, ulong num, int length=0)
        {
            int i; 

            if (length == 0)
                length = NumSize(num);
            num |= 1UL << length * 7;
            for (i = length - 1; i >= 0; i--)
                writer.Write((byte)(num >> (i * 8)));
        }
        public static void Ebml_Write_Id(this BinaryWriter writer, ulong id)
        {
            int i = IdSize(id);
            while (i-->0)
                writer.Write((byte)(id >> (i * 8)));
        }


        public static ulong Ebml_Write_Uint_With_Id(this BinaryWriter writer, ulong id, ulong val)
        {
            int bytes = UintSize(val);
            writer.Ebml_Write_Id(id);
            writer.Ebml_Write_Num((ulong)bytes);
            ulong pos = (ulong)writer.BaseStream.Position;
            for (int i = bytes - 1; i >= 0; i--)
                writer.Write((byte)(val >> (i * 8)));
            return pos;
        }

        public static ulong Ebml_Write_Sint_With_Id(this BinaryWriter writer, ulong id, long val)
        {
            int bytes = SintSize(val); 
            writer.Ebml_Write_Id(id);
            writer.Ebml_Write_Num((ulong)bytes);
            ulong pos = (ulong)writer.BaseStream.Position;
            for (int i = bytes - 1; i >= 0; i--)
                writer.Write((byte)(val >> (i * 8)));
            return pos;
        }

        public static ulong Ebml_Write_Float_With_Id(this BinaryWriter writer, ulong id, double val)
        {
            writer.Ebml_Write_Id(id);
            writer.Ebml_Write_Num(4);
            ulong pos = (ulong)writer.BaseStream.Position;

            byte[] dBytes = BitConverter.GetBytes((float)val);
            Array.Reverse(dBytes);
            writer.Write(dBytes);
            return pos;
        }
        public static ulong Ebml_Write_Binary_With_Id(this BinaryWriter writer, ulong id, byte[] val)
        {
            writer.Ebml_Write_Id(id);
            writer.Ebml_Write_Num((ulong)val.Length);
            ulong pos = (ulong)writer.BaseStream.Position;
            writer.Write(val);
            return pos;
        }
        public static ulong Ebml_Write_Ascii_With_Id(this BinaryWriter writer, ulong id, string val)
        {
            byte[] data = Encoding.ASCII.GetBytes(val);
            writer.Ebml_Write_Id(id);
            writer.Ebml_Write_Num((ulong)data.Length);
            ulong pos = (ulong)writer.BaseStream.Position;
            writer.Write(data);
            return pos;
        }
        public static ulong Ebml_Write_Utf8_With_Id(this BinaryWriter writer, ulong id, string val)
        {
            byte[] data = Encoding.UTF8.GetBytes(val);
            writer.Ebml_Write_Id(id);
            writer.Ebml_Write_Num((ulong)data.Length);
            ulong pos = (ulong)writer.BaseStream.Position;
            writer.Write(data);
            return pos;
        }
    }
}
