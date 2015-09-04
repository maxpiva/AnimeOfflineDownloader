using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ADBaseLibrary.Matroska.Objects
{
    public class EbmlMaster : EbmlGeneric
    {
        public List<EbmlGeneric> Value { get; set; } = new List<EbmlGeneric>();
        internal List<EbmlGeneric> Available { get; set; }      
        internal bool NoHead { get; set; }


        public override ulong ContentSize
        {
            get
            {
                ulong bsize = 0;
                foreach (EbmlGeneric g in Value)
                    bsize += g.TotalSize;
                return bsize;
            }
        }
        public override ulong HeaderSize
        {
            get
            {
                if (NoHead)
                    return 0;
                return base.HeaderSize;

            }
        }

        public void PropagateExpectedOffset()
        {
            ExpectedValueOffset = ExpectedOffset + HeaderSize;
            ulong bsize = ExpectedValueOffset;

            foreach (EbmlGeneric g in Value)
            {
                g.ExpectedOffset = bsize;
                if (g is EbmlMaster)
                {
                    EbmlMaster h = g as EbmlMaster;
                    h.PropagateExpectedOffset();
                }
                else
                    g.ExpectedValueOffset = bsize + g.HeaderSize;
                bsize += g.TotalSize;
            }
        }
        public override void Write(BinaryWriter writer, HashAlgorithm algo = null)
        {
            OutputOffset = (ulong)writer.BaseStream.Position;
            if (!NoHead)
            {
                writer.Ebml_Write_Id(Id);
                writer.Ebml_Write_Num(ContentSize);
            }
            OutputValueOffset = (ulong)writer.BaseStream.Position;
            foreach (EbmlGeneric g in Value)
                g.Write(writer);
        }

        public override EbmlGeneric New()
        {
            EbmlMaster b = new EbmlMaster();
            b.Id = Id;
            b.InputOffset = InputOffset;
            b.Available = Available;
            b.Weight = Weight;

            return b;
        }
        public override void Read(BinaryReader reader, ulong baselength = 0)
        {
            InputOffset = (ulong)reader.BaseStream.Position - (ulong)MatroskaExtensions.IdSize(Id);
            ulong length;
            if (baselength == 0)
                reader.Ebml_Read_Length(out length);
            else
                length = baselength;
            InputValueOffset = (ulong)reader.BaseStream.Position;
            while (length > 0)
            {
                ulong coffset = (ulong)reader.BaseStream.Position;
                ulong head;
                reader.Ebml_Read_Id(4, out head);
                bool found = false;
                List<EbmlGeneric> ls=new List<EbmlGeneric>();
                ls.AddRange(Available);
                ls.AddRange(Container.General);
                foreach (EbmlGeneric k in ls)
                {
                    if (k.Id == head)
                    {
                        EbmlGeneric c = k.New();
                        Value.Add(c);
                        c.Read(reader);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new IOException(string.Format("Unknown header {0:X}", head));
                length -= (ulong)reader.BaseStream.Position - coffset;
            }
            Value.Sort();
        }



        internal EbmlGeneric FindRecursive(ulong id, List<EbmlGeneric> list)
        {
            foreach (EbmlGeneric m in list)
            {
                if (m.Id == id)
                    return m;
                if (m is EbmlMaster)
                {
                    EbmlGeneric res = FindRecursive(id, ((EbmlMaster)m).Value);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }

        public EbmlGeneric FindFirst(ulong id)
        {
            return FindRecursive(id, Value);
        }
        public List<EbmlGeneric> FindAll(ulong id)
        {
            return FindAllRecursive(id, Value);
        }
        internal List<EbmlGeneric> FindAllRecursive(ulong id, List<EbmlGeneric> list)
        {
            List<EbmlGeneric> lst = new List<EbmlGeneric>();
            foreach (EbmlGeneric m in list)
            {
                if (m.Id == id)
                    lst.Add(m);
                if (m is EbmlMaster)
                {
                    lst.AddRange(FindAllRecursive(id, ((EbmlMaster)m).Value));
                }
            }
            return lst;
        }
        internal void RemoveRecursive(ulong id, EbmlMaster master)
        {
            foreach (EbmlGeneric m in master.Value.ToList())
            {
                if (m.Id == id)
                {
                    master.Value.Remove(m);
                }
                else if (m is EbmlMaster)
                {
                    RemoveRecursive(id, (EbmlMaster)m);
                }
            }
        }

        public void RemoveAll(ulong id)
        {
            RemoveRecursive(id,this);
        }
    }

}
