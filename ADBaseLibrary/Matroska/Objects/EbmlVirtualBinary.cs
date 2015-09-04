using System.IO;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlVirtualBinary : EbmlGeneric
    {
        private BinaryReader _inputReader;

        public ulong Length { get; private set; }

        public override EbmlGeneric New()
        {
            EbmlVirtualBinary b = new EbmlVirtualBinary();
            b.Id = Id;
            b.InputOffset = InputOffset;
            b.Weight = Weight;
            return b;
        }
        public override ulong ContentSize
        {
            get
            {
                return Length;
            }
        }

        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            writer.Ebml_Write_Id(Id);
            writer.Ebml_Write_Num(Length);
            OutputValueOffset = (ulong)writer.BaseStream.Position;
            _inputReader.BaseStream.Seek((long)InputValueOffset, SeekOrigin.Begin);
            _inputReader.CopyBytes(Length, writer, algo);
        }


        public override void Read(BinaryReader reader, ulong baselength = 0)
        {
            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            Length = length;
            InputValueOffset = (ulong)reader.BaseStream.Position;
            _inputReader = reader;
            reader.BaseStream.Seek((long)length, SeekOrigin.Current);
        }

    }
}
