using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class DownloadInfo
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public List<string> Languages { get; set; }
        public Quality Quality { get; set; }
        public Format Format { get; set; }
        public string Status { get; set; }
        public double Percent { get; set; }
        public string Size { get; set; }
    }
}
