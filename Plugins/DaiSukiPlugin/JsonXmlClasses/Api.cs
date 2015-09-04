using Newtonsoft.Json;

namespace DaiSukiPlugin.JsonXmlClasses
{
    public class Api
    {
        [JsonProperty("ss_id")]
        public string SS_Id { get; set; }
        [JsonProperty("mv_id")]
        public string MV_Id { get; set; }
        [JsonProperty("device_cd")]
        public string Device_CD { get; set; }
        [JsonProperty("ss1_prm")]
        public string SS1_PRM { get; set; }
        [JsonProperty("ss2_prm")]
        public string SS2_PRM { get; set; }
        [JsonProperty("ss3_prm")]
        public string SS3_PRM { get; set; }

    }
}
