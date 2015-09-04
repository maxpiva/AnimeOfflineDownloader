using System.IO;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlBinary : EbmlGeneric
    {
        public byte[] Value { get; set; }

        public override EbmlGeneric New()
        {
            EbmlBinary b = new EbmlBinary();
            b.Id = Id;
            b.InputOffset = InputOffset;
            b.Value = Value;
            b.Weight = Weight;

            return b;
        }
        public override ulong ContentSize
        {
            get
            {
                return (ulong)Value.Length;
            }
        }
        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            writer.Ebml_Write_Id(Id);
            writer.Ebml_Write_Num((ulong)Value.Length);
            OutputValueOffset = (ulong)writer.BaseStream.Position;
            writer.Write(Value);
        }
        public override void Read(BinaryReader reader, ulong baselength = 0)
        {

            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length;
            byte[] value;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            InputValueOffset = (ulong)reader.BaseStream.Position;
            reader.Ebml_Read_Binary((int)length, out value);
            Value = value;

        }
    }
}
