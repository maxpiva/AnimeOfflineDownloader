using System.Collections.Generic;
using System.Linq;

namespace ADBaseLibrary.AdobeHDS
{
    public class BootStrap
    {
        public string Profile { get; set; }

        public string Id { get; set; }

        public int Version { get; set; }

        public int Flags { get; set; }

        public int BootStrapVersion { get; set; }

        public int ProfileType { get; set; }

        public bool Live { get; set; }

        public bool HasMetadata { get; set; }

        public bool IsUpdate { get; set; }

        public int Timescale { get; set; }

        public long CurrentMediaTime { get; set; }

        public long SmpteTimeCodeOffset { get; set; }

        public string MovieIdentifier { get; set; }

        public List<string> ServerEntryTable { get; set; }=new List<string>();

        public List<string> QualityEntryTable { get; set; }=new List<string>();

        public string DrmData { get; set; }

        public string MetaData { get; set; }

        public Dictionary<int,Segment> SegmentTables { get; set; }=new Dictionary<int, Segment>(); 

        public Dictionary<int,Fragment> FragmentTables { get; set; }= new Dictionary<int, Fragment>();

        public bool InvalidFragCount { get; set; } = false;

        public int FragmentCount { get; set; }

        public int FragmentStart { get; set;}  = -1;

        public int SegmentStart { get; set;  }= -1;

        public int CurrentFragmentRead { get; set; }

        public int CurrentFragmentWrite { get; set; }

        public void InitFragments()
        {
            Segment first_segment = SegmentTables[SegmentTables.Keys.Min()];
            Segment last_segment = SegmentTables[SegmentTables.Keys.Max()];
            Fragment first_fragment = FragmentTables[FragmentTables.Keys.Min()];
            Fragment last_fragment = FragmentTables[FragmentTables.Keys.Max()];
            if ((last_fragment.FragmentDuration == 0) && (last_fragment.DiscontinuityIndicator.HasValue && last_fragment.DiscontinuityIndicator == 0))
            {
                Live = false;
                FragmentTables.Remove(FragmentTables.Keys.Max());
                last_fragment = FragmentTables[FragmentTables.Keys.Max()];
            }
            InvalidFragCount = false;
            Segment prev = null;
            FragmentCount = 0;
            foreach (int n in SegmentTables.Keys)
            {
                Segment seg = SegmentTables[n];
                if (prev != null)
                    FragmentCount += (seg.FirstEntry - prev.FirstEntry - 1) * prev.NumFragments;
                FragmentCount += seg.NumFragments;
                prev = seg;
            }
            if ((FragmentCount & 0x80000000) != 0x80000000)
            {
                FragmentCount += first_fragment.FirstFragment - 1;
            }
            else
            {
                FragmentCount = 0;
                InvalidFragCount = true;
            }
            if (FragmentCount < last_fragment.FirstFragment)
                FragmentCount = last_fragment.FirstFragment;
            if (SegmentStart == -1)
            {
                SegmentStart = Live ? last_segment.FirstEntry : first_segment.FirstEntry;
                if (SegmentStart < 1)
                    SegmentStart = 1;
            }
            if (FragmentStart == -1)
            {
                if (Live && !InvalidFragCount)
                    FragmentStart = FragmentCount - 2;
                else
                {
                    FragmentStart = first_fragment.FirstFragment - 1;
                    if (FragmentStart < 0)
                        FragmentStart = 0;
                }
            }
        }

        public Segment SegmentFromFragmentNumber(int fragment)
        {
            Segment first_segment = SegmentTables[SegmentTables.Keys.Min()];
            Segment last_segment = SegmentTables[SegmentTables.Keys.Max()];
            if (SegmentTables.Count == 1)
                return SegmentTables.ElementAt(0).Value;
            Segment prev = first_segment;
            int start = FragmentTables[FragmentTables.Keys.Min()].FirstFragment;
            for (int i = first_segment.FirstEntry; i <= last_segment.FirstEntry; i++)
            {
                Segment seg = SegmentTables.ContainsKey(i) ? SegmentTables[i] : prev;
                int end = start + seg.NumFragments;
                if ((fragment >= start) && (fragment < end))
                    return SegmentTables[i];
                prev = seg;
                start = end;
            }
            return last_segment;
        }

        private Fragment FragmentFromFragmentNumber(int fragment)
        {
            if (FragmentTables.Count == 1)
                return FragmentTables.ElementAt(0).Value;
            for (int x = 0; x < FragmentTables.Count - 1; x++)
            {
                Fragment cu = FragmentTables.ElementAt(x).Value;
                Fragment ne = FragmentTables.ElementAt(x + 1).Value;
                if (fragment >= cu.FirstFragment && fragment < ne.FirstFragment)
                    return cu;
            }
            return FragmentTables.ElementAt(FragmentTables.Count - 1).Value;
        }

        private object fraglock=new object();

        public KeyValuePair<int, int>? GetNextRead()
        {
            lock (fraglock)
            {
                if (CurrentFragmentRead == FragmentCount)
                    return null;
                KeyValuePair<int, int>? v = new KeyValuePair<int, int>(SegmentFromFragmentNumber(CurrentFragmentRead).FirstEntry, CurrentFragmentRead);
                CurrentFragmentRead = NextFragment(CurrentFragmentRead);
                return v;
            }
        }
        public void IncreaseFragmentWrite()
        {
            lock (fraglock)
            {
                if (CurrentFragmentWrite == FragmentCount)
                    return;
                CurrentFragmentWrite = NextFragment(CurrentFragmentWrite);
            }
        }
        private int NextFragment(int prevfragment)
        {
            if (prevfragment == FragmentCount)
                return prevfragment;
            prevfragment++;
            Fragment f = FragmentFromFragmentNumber(prevfragment);
            if (f.DiscontinuityIndicator.HasValue)
            {
                int idx = FragmentTables.IndexOf(f)+1;
                prevfragment = idx < FragmentTables.Count ? FragmentTables.ElementAt(idx).Value.FirstFragment : FragmentCount;
            }
            return prevfragment;
        }

    }

    public class Segment
    {
        public int FirstEntry { get; set; }

        public int NumFragments { get; set; }
    }


    public class Fragment
    {
        public int FirstFragment { get; set; }

        public long FirstFragmentTimestamp { get; set;  }

        public int FragmentDuration { get; set; }

        public byte? DiscontinuityIndicator { get; set; }


    }
}
