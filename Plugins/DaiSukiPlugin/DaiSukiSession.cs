using System.Collections.Generic;
using ADBaseLibrary;

namespace DaiSukiPlugin
{
    public class DaiSukiSession : Response, ISession
    {
        internal Dictionary<string, string> cookies { get; set; }

        public Dictionary<string,string> Serialize()
        {
            return cookies;
        }


    }


}
