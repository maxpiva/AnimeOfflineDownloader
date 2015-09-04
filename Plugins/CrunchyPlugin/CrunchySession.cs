using System.Collections.Generic;
using ADBaseLibrary;
using Newtonsoft.Json;

namespace CrunchyPlugin
{
    public class CrunchySession : Response, ISession
    {
        internal Dictionary<string, string> cookies { get; set; }

        public Dictionary<string,string> Serialize()
        {
            return cookies;
        }


    }


}
