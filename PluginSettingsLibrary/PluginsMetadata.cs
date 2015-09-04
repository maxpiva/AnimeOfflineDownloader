using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADBaseLibrary;
using Newtonsoft.Json;

namespace PluginSettingsLibrary
{
    public static class PluginsMetadata 
    {
        public static Dictionary<string, Dictionary<string, object>> FromString(string str)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(str);            
        }
    }
}
