using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public enum DownloadStatus
    {
        Queue,
        Downloading,
        Cancelled,
        Error,
        Complete,
        None
    }
}
