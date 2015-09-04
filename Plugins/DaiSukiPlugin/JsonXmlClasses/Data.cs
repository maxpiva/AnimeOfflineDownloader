using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DaiSukiPlugin.JsonXmlClasses
{
    public class Data
    {
        public string ssrcd { get; set; }
        public string init_id { get; set; }
        public string play_url { get; set; }
        public string sttmv_url { get; set; }
        public string vl_enable_f { get; set; }
        public string vl_url { get; set; }
        public string vl_interval_sec { get; set; }
        public string vl_timeout_sec { get; set; }
        public string vl_errlimit_cnt { get; set; }
        public List<string> bw_label { get; set; }
        public List<string> adque_msec { get; set; }
        public string caption_url { get; set; }
        public List<Language> caption_lang { get; set; }
        public string copyright_str { get; set; }
        public object preimg_url { get; set; }
        public string autoplay_flag { get; set; }
        public string series_str { get; set; }
        public string title_str { get; set; }
        public SsExt ss_ext { get; set; }
    }

    public class Prev
    {
        public string movie_product_id { get; set; }
    }

    public class Next
    {
        public string movie_product_id { get; set; }
    }

    public class Language
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> Lang { get; set; }=new Dictionary<string, JToken>();

    }
    public class SsExt
    {
        public string series { get; set; }
        public string product { get; set; }
        public string fov { get; set; }
        public string vast { get; set; }
        public Prev prev { get; set; }
        public Next next { get; set; }
    }

}
