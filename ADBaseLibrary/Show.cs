using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class Show
    {
        public string Id { get; set; }
        public ShowType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PluginName { get; set; }
        public Dictionary<string, string> PluginMetadata { get; set; } = new Dictionary<string, string>();

    }


}
