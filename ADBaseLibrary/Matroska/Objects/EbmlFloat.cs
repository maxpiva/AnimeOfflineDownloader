using System.IO;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlFloat : EbmlGeneric
    {
        public double Value { get; set; }

        public override EbmlGeneric New()
        {
            EbmlFloat b = new EbmlFloat();
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
                return 4UL;
            }
        }

        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            OutputValueOffset = writer.Ebml_Write_Float_With_Id(Id, Value);
        }
        public override void Read(BinaryReader reader, ulong baselength = 0)
        {
            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length;
            double value;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            reader.Ebml_Read_Float((int)length, out value);
            Value = value;
        }
    }
}
