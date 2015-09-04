using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    [Flags]
    public enum Format
    {
        Mkv=1,
        Mp4=2
    }

    public static class FormatExtensions
    {
        public static string ToExtension(this Format f)
        {
            switch (f)
            {
                case Format.Mkv:
                    return "mkv";
                case Format.Mp4:
                    return "mp4";
            }
            return String.Empty;

        }

        public static string ToText(this Format f)
        {
            StringBuilder bld = new StringBuilder();
            foreach (Format ff in Enum.GetValues(typeof(Format)))
            {
                if ((ff & f) == ff)
                {
                    switch (f)
                    {
                        case Format.Mkv:
                            bld.Append("Matroska, ");
                            break;
                        case Format.Mp4:
                            bld.Append("MP4, ");
                            break;
                    }
                }
            }
            string b = bld.ToString();
            if (b.Length > 0)
            {
                b = b.Substring(0, b.Length - 2);
                return b;
            }
            return String.Empty;
        }
    }
}
