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


        [JsonProperty("imageURL_l")]
        public string LargeImage { get; set; }

        [JsonProperty("imageURL_s")]
        public string SmallImage { get; set; }


        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }
    }

}
