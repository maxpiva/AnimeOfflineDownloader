using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DaiSukiPlugin.JsonXmlClasses
{
    public class MetaEncrypt
    {
        [JsonProperty("rcd")]
        public string Status { get; set; }

        [JsonProperty("rtn")]
        public string EncryptedData { get; set; }
    }
}
