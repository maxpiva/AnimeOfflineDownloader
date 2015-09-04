using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class EpisodeWithDownloadSettings : Episode
    {
        public Quality Quality { get; set; }
        public Format Format { get; set; }

        public static EpisodeWithDownloadSettings FromEpisode(Episode ep, Quality q, Format f)
        {
            EpisodeWithDownloadSettings e=new EpisodeWithDownloadSettings();
            ep.CopyTo(e);
            e.Quality = q;
            e.Format = f;
            return e;
        }
    }


}
