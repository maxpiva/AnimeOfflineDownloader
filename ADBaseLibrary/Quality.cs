using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public enum Quality
    {
        Quality360P,
        Quality480P,
        Quality720P,
        Quality1080P
    }

    public static class QualityExtensions
    {
        public static int ToHeight(this Quality q)
        {
            if (q == Quality.Quality360P)
                return 360;
            if (q == Quality.Quality480P)
                return 480;
            if (q == Quality.Quality720P)
                return 720;
            if (q == Quality.Quality1080P)
                return 1080;
            return 0;
        }

        public static Quality ToQuality(this string height)
        {
            if (height == "360")
                return Quality.Quality360P;
            if (height == "480")
                return Quality.Quality480P;
            if (height == "720")
                return Quality.Quality720P;
            if (height == "1080")
                return Quality.Quality1080P;
            return Quality.Quality360P;
        }
        public static Quality ToQuality(this int height)
        {
            if (height == 360)
                return Quality.Quality360P;
            if (height == 480)
                return Quality.Quality480P;
            if (height == 720)
                return Quality.Quality720P;
            if (height == 1080)
                return Quality.Quality1080P;
            return Quality.Quality360P;
        }
        public static string ToText(this Quality q)
        {
            switch (q)
            {
                case Quality.Quality360P:
                    return "SD";
                case Quality.Quality480P:
                    return "480p";
                case Quality.Quality720P:
                    return "720p";
                case Quality.Quality1080P:
                    return "1080p";
            }
            return string.Empty;
        }
    }
}
