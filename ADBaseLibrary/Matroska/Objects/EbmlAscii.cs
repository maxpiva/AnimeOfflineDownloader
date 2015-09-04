using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlAscii : EbmlGeneric
    {
        public string Value { get; set; }

        public override EbmlGeneric New()
        {
            EbmlAscii b = new EbmlAscii();
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
                return (ulong)Encoding.ASCII.GetBytes(Value).Length;
            }
        }


        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            OutputValueOffset = writer.Ebml_Write_Ascii_With_Id(Id, Value);
        }
        public override void Read(BinaryReader reader, ulong baselength = 0)
        {
            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length;
            string value;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            reader.Ebml_Read_String((int)length, out value);
            Value = value;
        }
    }

}
