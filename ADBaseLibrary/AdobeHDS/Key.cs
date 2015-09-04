using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary.HDS
{
    public class Key
    {
        public string KeyUrl { get; set; }
        public byte[] KeyData { get; set; }
        public byte Version { get; set; }
        public int EcmId { get; set; }
        public int EcmTimeStamp { get; set; }
        public short KdfVersion { get; set; }
        public byte DccAccReserver { get; set; }
        public byte[] KeyIV { get; set; }

    }
}
