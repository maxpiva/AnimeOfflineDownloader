using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class EpisodeWithDownloadSettingAndStatus : EpisodeWithDownloadSettings
    {
        public DownloadStatus DownloadStatus { get; set; }
    }
}
