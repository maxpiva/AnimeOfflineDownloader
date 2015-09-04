using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class Episode : Response
    {        
        public string Id { get; set; }
        public string ShowId { get; set; }
        public string ShowName { get; set; }
        public string PluginName { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string SeasonAlpha { get; set; }
        public int SeasonNumeric { get; set; }
        public string EpisodeAlpha { get; set; }
        public int EpisodeNumeric { get; set; }
        public string Description { get; set; }
        public Uri ImageUri { get; set; }
        public string UniqueTag { get; set; }
        public ShowType Type { get; set; }
        public string DateTime { get; set; }
        public Dictionary<string,string> PluginMetadata { get; set; }=new Dictionary<string, string>();
    }

}
    