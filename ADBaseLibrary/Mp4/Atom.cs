using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary.Mp4
{
    public class Atom : List<Atom>
    {
        //Totally hacked do not reuse. only moov taken as parent

        public long Size { get; set; }
        public long Position { get; set; }
        public long ExpectedPosition { get; set; }
        public string Name { get; set; }
        private BinaryReader _reader;
        public byte[] Data { get; set; }

        public void ReadHeader(BinaryReader reader)
        {
            _reader = reader;
            Size = 0;
            Position = reader.BaseStream.Position;
            for (int x = 0; x < 4; x++)
            {
                Size = Size << 8 | reader.ReadByte();
            }
            byte[] nm = reader.ReadBytes(4);
            if (Size == 1)
            {
                for (int x = 0; x < 8; x++)
                {
                    Size = Size << 8 | reader.ReadByte();
                }
            }
            Name = Encoding.ASCII.GetString(nm);
        }

        public void Write(BinaryWriter writer)
        {
            if ((Data==null) && (this.Count==0))
            {
                _reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                Matroska.MatroskaExtensions.CopyBytes(_reader, (ulong) Size, writer);
            }
            else if ((Data!=null) && (this.Count==0))
            {
                Size = Data.Length+8;
                for (int i = 3; i >= 0; i--)
                    writer.Write((byte)(Size >> (i * 8)));
                byte[] dta = Encoding.ASCII.GetBytes(Name);
                writer.Write(dta);
                writer.Write(Data);
            }
            else
            {
                for (int i = 3; i >= 0; i--)
                    writer.Write((byte)(Size >> (i * 8)));
                byte[] dta = Encoding.ASCII.GetBytes(Name);
                writer.Write(dta);
                foreach (Atom a in this)
                {
                    a.Write(writer);
                }
            }
        }



        public void SeekEnd(BinaryReader reader)
        {
            reader.BaseStream.Seek(Size - 8, SeekOrigin.Current);
        }

        public void ReadData(BinaryReader reader)
        {
            _reader.BaseStream.Seek(Position+8, SeekOrigin.Begin);
            Data = reader.ReadBytes((int)Size - 8);
        }
        public void Read(BinaryReader reader)
        {
            long Length = Size-8;
            while (Length > 0)
            {
                Atom a = new Atom();
                a.ReadHeader(reader);
                if (a.Name == "moov")
                    a.Read(reader);
                else
                    a.SeekEnd(reader);
                Add(a);
                Length -= a.Size;
            }
        }
        public void ReCalcSize()
        {
            Size = 8 + this.Sum(a => a.Size);
        }

        public void ReCalcPosition()
        {
            long s = ExpectedPosition;
            foreach (Atom a in this)
            {
                a.ExpectedPosition = s;
                s += a.Size;
            }
        }
    }


}
