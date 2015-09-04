using Newtonsoft.Json;

namespace DaiSukiPlugin.JsonXmlClasses
{
    [JsonObject("response")]
    public class Anime
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("ad_id")]
        public string AdId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("release_open")]
        public string ReleaseOpen { get; set; }
        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }
    }

}
