using System;
using System.IO;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{

    public abstract class EbmlGeneric : IComparable
    {
        public ulong Id { get; set; }
        public ulong InputOffset { get; set; }
        public ulong InputValueOffset { get; set; }
        public ulong OutputOffset { get; set; }
        public ulong OutputValueOffset { get; set; }
        public ulong ExpectedOffset { get; set; }
        public ulong ExpectedValueOffset { get; set; }
        public int Weight { get; set; } = 0;

        public int LinkedId { get; set; }



        public abstract EbmlGeneric New();
        public abstract void Read(BinaryReader reader, ulong baselength = 0);
        public abstract void Write(BinaryWriter writer, HashAlgorithm algo = null);


        public virtual ulong TotalSize { get { return HeaderSize + ContentSize; } }

        public virtual ulong HeaderSize
        {
            get
            {
                return (ulong)MatroskaExtensions.IdSize(Id) + (ulong)MatroskaExtensions.NumSize(ContentSize);
            }
        }
        public abstract ulong ContentSize { get; }


        public virtual int CompareTo(object obj)
        {

            EbmlGeneric m = (EbmlGeneric) obj;
            int res = Weight.CompareTo(m.Weight);
            if (res != 0)
                return res;
            if (Weight==-1)
                return InputOffset.CompareTo(m.InputOffset);
            res = Id.CompareTo(m.Id);
            if (res != 0)
                return res;
            return InputOffset.CompareTo(m.InputOffset);
        }
    }
}
