using System.IO;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlSint : EbmlGeneric
    {
        public long Value { get; set; }

        public override EbmlGeneric New()
        {
            EbmlSint b = new EbmlSint();
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
                return (ulong)MatroskaExtensions.SintSize(Value);
            }
        }
        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            OutputValueOffset = writer.Ebml_Write_Sint_With_Id(Id, Value);
        }
        public override void Read(BinaryReader reader, ulong baselength = 0)
        {
            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length;
            long value;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            reader.Ebml_Read_Sint((int)length, out value);
            Value = value;
        }
    }
}
