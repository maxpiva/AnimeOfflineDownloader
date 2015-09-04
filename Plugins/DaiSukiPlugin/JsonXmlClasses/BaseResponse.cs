using System.Collections.Generic;
using Newtonsoft.Json;

namespace DaiSukiPlugin.JsonXmlClasses
{
    public class BaseResponse<T>
    {
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("response")]
        public List<T> Response { get; set; }
    }
}
