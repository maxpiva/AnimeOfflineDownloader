using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ADBaseLibrary.Helpers;
using ADBaseLibrary.Internal;
using Newtonsoft.Json;

namespace ADBaseLibrary
{
    public static class Extensions
    {





        public static Response VerifyRequiredKeys(Dictionary<string, object> metadata, List<Requirement> requirements)
        {
            foreach (string r in requirements.Where(a => (a.RequirementType & RequirementType.Required) == RequirementType.Required).Select(a=>a.Name))
            {
                if (!metadata.ContainsKey(r))
                    return new Response { ErrorMessage = "'" + r + "' metadata is missing", Status = ResponseStatus.MissingRequirement };
            }
            return new Response { Status = ResponseStatus.Ok,ErrorMessage = "OK"};
        }

        public static async Task<Response> VerifyBaseAuthentication(this DownloadPluginInfo pinfo, Dictionary<string, object> authmetadata)
        {
            return await Task.Run(() =>
            {
                Response r = VerifyRequiredKeys(authmetadata, pinfo.AuthenticationRequirements);
                if (r.Status != ResponseStatus.Ok)
                    return r;
                if (VerifyRequirementsList(pinfo.AuthenticationRequirements, DownloadPluginInfo.Username, DownloadPluginInfo.Password))
                {
                    string username = authmetadata.GetStringFromMetadata(DownloadPluginInfo.Username);
                    if (string.IsNullOrEmpty(username))
                        return new Response { ErrorMessage = "'" + DownloadPluginInfo.Username + "' is required", Status = ResponseStatus.MissingRequirement };
                    string password = authmetadata.GetStringFromMetadata(DownloadPluginInfo.Password);
                    if (string.IsNullOrEmpty(password))
                        return new Response { ErrorMessage = "'" + DownloadPluginInfo.Password + "' is required", Status = ResponseStatus.MissingRequirement };
                }
                return r;
            });
        }
        public static async Task<Response> VerifyGlobalRequirements(this DownloadPluginInfo pinfo, Dictionary<string, object> globalmetadata)
        {
            Response r=VerifyRequiredKeys(globalmetadata, pinfo.GlobalRequirements);
            if (r.Status != ResponseStatus.Ok)
                return r;
            if (VerifyRequirementsList(pinfo.GlobalRequirements, DownloadPluginInfo.ProxyEnabled,DownloadPluginInfo.ProxyAddress,DownloadPluginInfo.ProxyPort,DownloadPluginInfo.ProxyUsername,DownloadPluginInfo.ProxyPassword))
            {
                bool? enabled = globalmetadata.GetBoolFromMetadata(DownloadPluginInfo.ProxyEnabled);
                if (enabled.HasValue && enabled.Value)
                {
                    string address = globalmetadata.GetStringFromMetadata(DownloadPluginInfo.ProxyAddress);
                    if (string.IsNullOrEmpty(address))
                         return new Response { ErrorMessage = "'" + DownloadPluginInfo.ProxyAddress + "' is invalid", Status = ResponseStatus.MissingRequirement };
                    int? port = globalmetadata.GetIntFromMetadata(DownloadPluginInfo.ProxyPort);
                    if (!port.HasValue)
                        return new Response { ErrorMessage = "'" + DownloadPluginInfo.ProxyPort + "' is invalid", Status = ResponseStatus.MissingRequirement };
                    if (port.Value<1 || port.Value>65534)
                        return new Response { ErrorMessage = "'" + DownloadPluginInfo.ProxyPort + "' is invalid", Status = ResponseStatus.InvalidArgument };
                    IpInfo ipnfo = null;
                    IWebProxy proxy = pinfo.ProxyFromGlobalRequirements(globalmetadata);
                    if (proxy==null)
                        return new Response { ErrorMessage = "Unable to create proxy", Status = ResponseStatus.InvalidArgument };
                    WebStream s = await WebStream.Get("http://ipinfo.io/json", null, null, null, null, 10000, false, null, proxy);
                    if (s != null && s.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(s, Encoding.UTF8);
                        string json = reader.ReadToEnd();
                        ipnfo = JsonConvert.DeserializeObject<IpInfo>(json);
                        reader.Dispose();
                    }
                    s?.Dispose();
                    if ((s == null) || (s.StatusCode != HttpStatusCode.OK))
                        return new Response { ErrorMessage = "Unable to Connect",Status=ResponseStatus.InvalidArgument};
                    r.ErrorMessage="IP: " + ipnfo.ip + " " + ipnfo.city + "/" + ipnfo.country;
                }
            }
            return r;
        }

        public static bool VerifyRequirementsList(List<Requirement> requirements, params string[] pars)
        {
            foreach (string s in pars)
            {
                if (!requirements.Any(a => a.Name == s))
                    return false;
            }
            return true;
        }
        public static string GetStringFromMetadata(this Dictionary<string, object> metadata, string key)
        {
            if (metadata.ContainsKey(key))
            {
                if ((metadata[key] != null) && (metadata[key] is string))
                    return (string)metadata[key];
            }
            return null;
        }
        public static Dictionary<string, object> JoinMetadata(this Dictionary<string, object> metadata, Dictionary<string, object> defaultvalues)
        {
            Dictionary<string, object> dict = metadata.ToDictionary(a => a.Key, a => a.Value);
            foreach (string k in defaultvalues.Keys)
            {
                if (!dict.ContainsKey(k))
                {
                    dict.Add(k, defaultvalues[k]);
                }
            }
            return dict;
        }

        public static bool? GetBoolFromMetadata(this Dictionary<string, object> metadata, string key)
        {
            if (metadata.ContainsKey(key))
            {
                if ((metadata[key] != null) && (metadata[key] is bool))
                    return (bool)metadata[key];
            }
            return null;
        }
        public static int? GetIntFromMetadata(this Dictionary<string, object> metadata, string key)
        {
            if (metadata.ContainsKey(key))
            {
                if (metadata[key] != null)
                {
                    if (metadata[key] is int)
                        return (int) metadata[key];
                    if (metadata[key] is long)
                        return (int) ((long) metadata[key]);
                }
            }
            return null;
        }
        public static int IndexOf<T,S>(this Dictionary<T,S> dictionary, S value)
        {
            int cnt = 0;
            foreach (T k in dictionary.Keys)
            {
                if (dictionary[k].Equals(value))
                    return cnt;
                cnt++;
            }
            return -1;
        }
        public static IWebProxy ProxyFromGlobalRequirements(this DownloadPluginInfo pinfo, Dictionary<string, object> globalmetadata)
        {
            if (globalmetadata == null)
                return null;
            NetworkCredential nt = null;
            bool? enabled = globalmetadata.GetBoolFromMetadata(DownloadPluginInfo.ProxyEnabled);
            if (enabled.HasValue && enabled.Value)
            {
                string proxyaddress = globalmetadata.GetStringFromMetadata(DownloadPluginInfo.ProxyAddress);
                int? proxyport = globalmetadata.GetIntFromMetadata(DownloadPluginInfo.ProxyPort);
                string proxyusername = globalmetadata.GetStringFromMetadata(DownloadPluginInfo.ProxyUsername);
                string proxypassword = globalmetadata.GetStringFromMetadata(DownloadPluginInfo.ProxyPassword);
                if (!string.IsNullOrEmpty(proxyusername) || !string.IsNullOrEmpty(proxypassword))
                    nt = new NetworkCredential(proxyusername, proxypassword);
                UriBuilder bld = new UriBuilder(proxyaddress);
                bld.Port = proxyport.Value;
                return new WebProxy(bld.Uri, false, null, nt);

            }
            return null;
        }
        public static string PostFromDictionary(this Dictionary<string, string> dict)
        {
            return String.Join("&", dict.Select(a => HttpUtility.UrlEncode(a.Key) + "=" + HttpUtility.UrlEncode(a.Value)));
        }

        public static Dictionary<string, string> ToDictionary(this CookieCollection coll)
        {
            Dictionary<string,string> data=new Dictionary<string, string>();
            foreach (Cookie c in coll)
            {
                data.Add(c.Name, c.Value);
            }
            return data;
        }

        public static CookieCollection ToCookieCollection(this Dictionary<string, string> dict)
        {
            CookieCollection cont=new CookieCollection();
            foreach(string n in dict.Keys)
                cont.Add(new Cookie(n,dict[n]));
            return cont;
        } 
        public static void PropagateError(this IResponse from, IResponse to)
        {
            if (from.Status != ResponseStatus.Ok)
            {
                to.Status = from.Status;
                to.ErrorMessage = from.ErrorMessage;
            }
        }
        public static void CopyTo(this object s, object t)
        {
            foreach (var pS in s.GetType().GetProperties())
            {
                foreach (var pT in t.GetType().GetProperties())
                {
                    if (pT.Name != pS.Name) continue;
                    (pT.GetSetMethod()).Invoke(t, new object[] { pS.GetGetMethod().Invoke(s, null) });
                }
            };
        }
    }
}
