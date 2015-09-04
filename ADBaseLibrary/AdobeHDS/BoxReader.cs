using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ADBaseLibrary.AdobeHDS
{
    public class BoxReader : BinaryReader
    {
        public BoxReader(Stream s) : base(s)
        {

        }

        public long ReadHeader(out string name)
        {
            long size = EReadInt32();
            byte[] dta = ReadBytes(4);
            name = Encoding.ASCII.GetString(dta);
            if (size == 1)
            {
                size = EReadInt64();
                size -= 8;
            }
            size -= 8;
            return size;
        }

        public int EReadInt16()
        {
            int result = 0;
            for (int x = 0; x < 2; x++)
                result = result << 8 | ReadByte();
            return result;
        }

        public int EReadInt24()
        {
            int result = 0;
            for (int x = 0; x < 3; x++)
                result = result << 8 | ReadByte();
            return result;
        }

        public int EReadInt32()
        {
            byte[] dta = ReadBytes(4);
            Array.Reverse(dta);
            return BitConverter.ToInt32(dta, 0);
        }

        public long EReadInt64()
        {
            byte[] dta = ReadBytes(8);
            Array.Reverse(dta);
            return BitConverter.ToInt64(dta, 0);
        }

        public string EReadString()
        {
            StringBuilder bld = new StringBuilder();
            byte b = ReadByte();
            while (b != 0)
            {
                bld.Append((char) b);
                b = ReadByte();
            }
            return bld.ToString();
        }

        public Dictionary<int, Segment> ReadSegment()
        {
            Dictionary<int, Segment> segments = new Dictionary<int, Segment>();
            ReadByte();
            EReadInt24();
            int cnt = ReadByte();
            for (int x = 0; x < cnt; x++)
                ReadString();
            cnt = EReadInt32();
            for (int x = 0; x < cnt; x++)
            {
                int first = EReadInt32();
                int numf = EReadInt32();
                if ((numf & 0x80000000) == 0x80000000)
                    numf = 0;
                segments.Add(first, new Segment {FirstEntry = first, NumFragments = numf});
            }
            return segments;
        }

        public Dictionary<int, Fragment> ReadFragment()
        {
            Dictionary<int, Fragment> frags = new Dictionary<int, Fragment>();
            ReadByte();
            EReadInt24();
            EReadInt32();
            int cnt = ReadByte();
            for (int x = 0; x < cnt; x++)
                ReadString();
            cnt = EReadInt32();
            for (int x = 0; x < cnt; x++)
            {
                Fragment fe = new Fragment();
                fe.FirstFragment = EReadInt32();
                fe.FirstFragmentTimestamp = EReadInt64();
                fe.FragmentDuration = EReadInt32();
                if (fe.FragmentDuration == 0)
                    fe.DiscontinuityIndicator = ReadByte();
                frags.Add(fe.FirstFragment, fe);
            }
            return frags;
        }

        public bool ReadBootStrap(BootStrap info)
        {
            info.Version = ReadByte();
            info.Flags = EReadInt24();
            info.BootStrapVersion = EReadInt32();
            byte flags = ReadByte();
            info.ProfileType = (flags & 0xc0) >> 6;
            if ((flags & 0x20) == 0x20)
            {
                info.Live = true;
                info.HasMetadata = false;
            }
            info.IsUpdate = ((flags & 0x10) == 0x10);
            if (!info.IsUpdate)
            {
                info.SegmentTables = new Dictionary<int, Segment>();
                info.FragmentTables = new Dictionary<int, Fragment>();
            }
            info.Timescale = EReadInt32();
            info.CurrentMediaTime = EReadInt64();
            info.SmpteTimeCodeOffset = EReadInt64();
            info.MovieIdentifier = EReadString();
            int cnt = ReadByte();
            info.ServerEntryTable = new List<string>();
            for (int x = 0; x < cnt; x++)
                info.ServerEntryTable.Add(EReadString());
            cnt = ReadByte();
            info.QualityEntryTable = new List<string>();
            for (int x = 0; x < cnt; x++)
                info.QualityEntryTable.Add(EReadString());
            info.DrmData = ReadString();
            info.MetaData = ReadString();
            cnt = ReadByte();
            for (int x = 0; x < cnt; x++)
            {
                string name;
                long left = ReadHeader(out name);
                if (name == "asrt")
                    ReadSegment().ToList().ForEach(a => info.SegmentTables[a.Key] = a.Value);
                else
                    BaseStream.Seek(left, SeekOrigin.Current);
            }
            cnt = ReadByte();
            for (int x = 0; x < cnt; x++)
            {
                string name;
                long left = ReadHeader(out name);
                if (name == "afrt")
                    ReadFragment().ToList().ForEach(a => info.FragmentTables[a.Key] = a.Value);
                else
                    BaseStream.Seek(left, SeekOrigin.Current);
            }
            info.SegmentTables = info.SegmentTables.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
            info.FragmentTables = info.FragmentTables.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
            info.InitFragments();
            return true;
        }
    }
}
