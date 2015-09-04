using System.IO;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlUint : EbmlGeneric
    {
        public ulong Value { get; set; }

        public override EbmlGeneric New()
        {
            EbmlUint b = new EbmlUint();
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
                return (ulong)MatroskaExtensions.UintSize(Value);
            }
        }
        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            OutputValueOffset = writer.Ebml_Write_Uint_With_Id(Id, Value);
        }
        public override void Read(BinaryReader reader, ulong baselength = 0)
        {
            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length, value;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            InputValueOffset = (ulong)reader.BaseStream.Position;
            reader.Ebml_Read_Uint((int)length, out value);
            Value = value;
        }
    }
}
