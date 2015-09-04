using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ADBaseLibrary.Helpers
{
    public class Web
    {
        private static void PopulateHeaders(HttpRequestMessage msg, Dictionary<string, string> headers)
        {
            foreach (string s in headers.Keys)
            {
                string k = s.ToLower();
                if (k.StartsWith("content") || k == "expires" || k == "last-modified")
                {
                    msg.Content.Headers.Add(s, headers[s]);
                }
                else
                {
                    msg.Headers.Add(s, headers[s]);
                }
            }
        }

      


        public static async Task<WebStream> GetStream(string Url, string PostData, bool GZip, string encoding, string uagent = null, Dictionary<string, string> headers = null, Dictionary<string, string> cookies = null, int timeout = 10000, bool SkipFound=false, string referer=null, IWebProxy proxy=null, bool specialcookies=false)
        {
            WebStream wb = new WebStream();
            try
            {
                do
                {
                    Uri u = new Uri(Url);
                    if (string.IsNullOrEmpty(encoding))
                        encoding = "UTF-8";
                    int t = ServicePointManager.DefaultConnectionLimit;
                    Uri bas = new Uri(u.Scheme + "://" + u.Host);
                    HttpRequestMessage msg = new HttpRequestMessage(string.IsNullOrEmpty(PostData) ? HttpMethod.Get : HttpMethod.Post, u);
                    wb.DisposableObjects.Add(msg);
                    if (!string.IsNullOrEmpty(uagent))
                        msg.Headers.UserAgent.ParseAdd(uagent);
                    if (!string.IsNullOrEmpty(PostData))
                    {
                        byte[] dta = Encoding.GetEncoding(encoding).GetBytes(PostData);
                        msg.Content = new ByteArrayContent(dta, 0, dta.Length);
                        msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    }
                    if (headers != null)
                        PopulateHeaders(msg, headers);
                    if (referer != null)
                        msg.Headers.Referrer = new Uri(referer);
                    HttpClientHandler handler = new HttpClientHandler();
                    wb.DisposableObjects.Add(handler);
                    handler.AllowAutoRedirect = false;
                    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    handler.CookieContainer = new CookieContainer();
                    if (proxy != null)
                        handler.Proxy = proxy;
                    if (cookies != null)
                    {
                        foreach (string k in cookies.Keys)
                            handler.CookieContainer.Add(new Cookie(k, cookies[k], "", bas.Host));
                    }
                    HttpClient http = new HttpClient(handler);
                    wb.DisposableObjects.Add(http);
                    http.Timeout = TimeSpan.FromMilliseconds(timeout);
                    HttpResponseMessage response = await http.SendAsync(msg);
                    wb.DisposableObjects.Add(response);
                    wb.DisposableObjects.Add(response.Content);
                    wb.Stream = await response.Content.ReadAsStreamAsync();
                    wb.DisposableObjects.Add(wb.Stream);
                    wb.ContentType = response.Content.Headers.ContentType.MediaType;
                    wb.ContentEncoding = response.Content.Headers.ContentEncoding.ToString();
                    wb.ContentLength = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : 0;
                    wb.Headers = new Dictionary<string, string>();
                    if (response.Headers.Contains("Set-Cookie"))
                    {
                        try
                        {
                            CookieContainer cc = new CookieContainer();
                            IEnumerable<string> sss;
                            response.Headers.TryGetValues("Set-Cookie", out sss);
                            if (!specialcookies)
                            {
                                foreach (string k in sss)
                                {

                                    cc.SetCookies(bas, k);

                                }
                                if (wb.Cookies == null)
                                {
                                    wb.Cookies = cookies != null
                                        ? cookies.ToDictionary(a => a.Key, a => a.Value)
                                        : new Dictionary<string, string>();
                                }
                                foreach (Cookie c in cc.GetCookies(bas))
                                {
                                    string cname = c.Name;
                                    //HttpUtility.UrlDecode(c.Name);
                                    string cvalue = c.Value.Replace("***EQUAL***", "=");
                                    if (c.Expired && wb.Cookies.ContainsKey(cname))
                                        wb.Cookies.Remove(cname);
                                    else
                                    {
                                        wb.Cookies[cname] = cvalue;
                                    }
                                }
                                if (wb.Cookies != null)
                                    cookies = wb.Cookies.ToDictionary(a => a.Key, a => a.Value);
                            }
                            else
                            {
                                wb.Cookies = cookies = GetCookies(sss);
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }
                    else
                    {
                        wb.Cookies = cookies;
                    }
                    if ((response.Headers != null) && (response.Headers.Any()))
                    {
                        foreach (KeyValuePair<string, IEnumerable<string>> h in response.Headers)
                        {
                            foreach (string r in h.Value)
                            {
                                string val = r;
                                if (val.StartsWith("\"") && val.EndsWith("\""))
                                    val = val.Substring(1, val.Length - 2);
                                wb.Headers[h.Key] = val;
                            }
                        }
                    }
                    wb.StatusCode = response.StatusCode;
                    if ((wb.StatusCode == HttpStatusCode.Found) || (wb.StatusCode == HttpStatusCode.Moved) ||
                        (wb.StatusCode == HttpStatusCode.Redirect))
                    {
                        referer = Url;
                        if (SkipFound)
                            return wb;
                        wb.Dispose();
                        if (response.Headers.Location.ToString().StartsWith("/"))
                            Url = u.Scheme + "://" + u.Host + response.Headers.Location;
                        else
                            Url = response.Headers.Location.AbsoluteUri;
                        UriBuilder bld = new UriBuilder(Url);
                        bld.Scheme = bas.Scheme;
                        Url = bld.Uri.ToString();
                        cookies = wb.Cookies;
                        PostData = null;
                    }
                } while ((wb.StatusCode == HttpStatusCode.Found) ||
                    (wb.StatusCode == HttpStatusCode.Moved) ||
                    (wb.StatusCode == HttpStatusCode.Redirect));
                return wb;
            }
            catch (Exception e)
            {
                wb?.Dispose();
                return null;
            }
           
        }

        private static Dictionary<string,string> GetCookies(IEnumerable<string> headers)
        {
            Dictionary<string, string> cc = new Dictionary<string, string>();
            foreach(string cookie in headers)
            {
                string[] splits = cookie.Split(';');
                if (splits[0] != string.Empty)
                {
                    int idx = splits[0].IndexOf("=");
                    cc.Add(splits[0].Substring(0, idx), splits[0].Substring(idx + 1, splits[0].Length - (idx + 1)));
                }
            }
            return cc;
        }
    }
}
