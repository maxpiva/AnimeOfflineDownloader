using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class Proxy
    {
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }

        public WebProxy CreateProxy()
        {
            NetworkCredential nt = null;
            if (!string.IsNullOrEmpty(ProxyUsername) || !string.IsNullOrEmpty(ProxyPassword))
                nt = new NetworkCredential(ProxyUsername, ProxyPassword);
            UriBuilder bld = new UriBuilder(ProxyAddress);
            bld.Port = ProxyPort;
            return new WebProxy(bld.Uri, false, null, nt);
        }

        public Proxy(string address, int port, string username, string password)
        {
            ProxyUsername = username;
            ProxyPassword = password;
            ProxyAddress = address;
            ProxyPort = port;
        }
    }
}
