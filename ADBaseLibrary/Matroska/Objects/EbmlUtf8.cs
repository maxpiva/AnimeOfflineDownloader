using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlUtf8 : EbmlAscii
    {
        public override EbmlGeneric New()
        {
            EbmlUtf8 b = new EbmlUtf8();
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
                return (ulong)Encoding.UTF8.GetBytes(Value).Length;
            }
        }

        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            OutputValueOffset = writer.Ebml_Write_Utf8_With_Id(Id, Value);
        }
    }
}
