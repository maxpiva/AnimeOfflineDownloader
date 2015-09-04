using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary.Helpers
{
    public class WebStream : Stream, IDisposable
    {
        private Stream baseStream=null;
        public CookieCollection Cookies { get; set; }
        public NameValueCollection Headers { get; private set; }
        public string ContentType { get; private set; }
        public string ContentEncoding { get; private set; }
        public long ContentLength { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string Url { get; private set; }


        internal List<IDisposable> DisposableObjects=new List<IDisposable>();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                for (int x = DisposableObjects.Count - 1; x >= 0; x--)
                {
                    DisposableObjects[x].Dispose();
                }
                baseStream?.Dispose();
            }
        }

        public override void Flush()
        {
            baseStream?.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if(baseStream!=null)
                return baseStream.Seek(offset, origin);
            return 0;
        }

        public override void SetLength(long value)
        {
            baseStream?.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (baseStream == null)
                return 0;
            return baseStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream?.Write(buffer,offset,count);
        }

        public override bool CanRead { get { return baseStream?.CanRead ?? false; } }
        public override bool CanSeek { get { return baseStream?.CanSeek ?? false; } }
        public override bool CanWrite { get { return baseStream?.CanWrite ?? false; } }

        public override long Length
        {
            get
            {
                if (baseStream == null)
                    return 0;
                if (baseStream.Length == 0)
                    return ContentLength;
                return baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                if (baseStream == null)
                    return 0;
                return baseStream.Position;
            }
            set
            {
                if (baseStream!=null)
                    baseStream.Position = value;
            }
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~WebStream()
        {
            Dispose(false);
        }

        public static async Task<WebStream> Get(string url, string encoding, string uagent = null,
            NameValueCollection headers = null, CookieCollection cookies = null, int timeout = 10000,
            bool redirect = true, string referer = null, IWebProxy proxy = null)
        {
            return await CreateStream(url, null, encoding, null, uagent, headers, cookies, timeout, redirect, referer, proxy);

        }
        public static async Task<WebStream> Post(string url, Dictionary<string, string> postdata, string encoding, string uagent = null,
    NameValueCollection headers = null, CookieCollection cookies = null, int timeout = 10000,
    bool redirect = true, string referer = null, IWebProxy proxy = null)
        {
            string pdata = postdata.PostFromDictionary();
            return await CreateStream(url,pdata, encoding, "application/x-www-form-urlencoded", uagent, headers, cookies, timeout, redirect, referer, proxy);

        }
        public static async Task<WebStream> CreateStream(string url, string postData, string encoding,
            string postencoding = "application/x-www-form-urlencoded", string uagent = null,
            NameValueCollection headers = null, CookieCollection cookies = null, int timeout = 10000,
            bool redirect = true, string referer = null, IWebProxy proxy = null)
        {
            WebStream wb = new WebStream();
            try
            {
                do
                {
                    Uri u = new Uri(url);
                    if (string.IsNullOrEmpty(encoding))
                        encoding = "UTF-8";
                    int t = ServicePointManager.DefaultConnectionLimit;
                    Uri bas = new Uri(u.Scheme + "://" + u.Host);
                    HttpRequestMessage msg =
                        new HttpRequestMessage(string.IsNullOrEmpty(postData) ? HttpMethod.Get : HttpMethod.Post, u);
                    wb.DisposableObjects.Add(msg);
                    if (!string.IsNullOrEmpty(uagent))
                        msg.Headers.UserAgent.ParseAdd(uagent);
                    if (!string.IsNullOrEmpty(postData))
                    {
                        byte[] dta = Encoding.GetEncoding(encoding).GetBytes(postData);
                        msg.Content = new ByteArrayContent(dta, 0, dta.Length);
                        msg.Content.Headers.ContentType = new MediaTypeHeaderValue(postencoding);
                    }
                    if (headers != null)
                        PopulateHeaders(msg, headers);
                    if (referer != null)
                        msg.Headers.Referrer = new Uri(referer);
                    HttpClientHandler handler = new HttpClientHandler();
                    wb.DisposableObjects.Add(handler);
                    handler.AllowAutoRedirect = false;
                    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    if (cookies != null && cookies.Count > 0)
                    {
                        handler.CookieContainer = new CookieContainer();
                        foreach (Cookie c in cookies)
                        {
                            if (string.IsNullOrEmpty(c.Domain))
                                c.Domain = bas.Host;

                            handler.CookieContainer.Add(c);
                        }
                    }
                    if (proxy != null)
                        handler.Proxy = proxy;
                    HttpClient http = new HttpClient(handler);
                    wb.DisposableObjects.Add(http);
                    http.Timeout = TimeSpan.FromMilliseconds(timeout);
                    HttpResponseMessage response = await http.SendAsync(msg);
                    wb.DisposableObjects.Add(response);
                    wb.DisposableObjects.Add(response.Content);
                    wb.baseStream = await response.Content.ReadAsStreamAsync();
                    wb.ContentType = response.Content.Headers.ContentType.MediaType;
                    wb.ContentEncoding = response.Content.Headers.ContentEncoding.ToString();
                    wb.ContentLength = response.Content.Headers.ContentLength ?? 0;
                    wb.Headers = new NameValueCollection();
                    if (response.Headers.Contains("Set-Cookie"))
                    {
                        try
                        {
                            IEnumerable<string> sss=null;
                            response.Headers.TryGetValues("Set-Cookie", out sss);
                            wb.Cookies=new CookieCollection();
                            CookieCollection coll = null;
                            if (sss != null)
                            {
                                wb.Cookies = GetAllCookiesFromHeader(sss, bas.ToString());
                            }
                            if (cookies != null && cookies.Count > 0)
                            {
                                foreach (Cookie c in cookies)
                                {
                                    bool found = false;
                                    foreach (Cookie d in wb.Cookies)
                                    {
                                        if (d.Name == c.Name)

                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                        wb.Cookies.Add(c);
                                }
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
                        if (wb.Headers==null)
                            wb.Headers=new NameValueCollection();
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
                        referer = url;
                        if (!redirect)
                            return wb;
                        wb.Dispose();
                        if (response.Headers.Location.ToString().StartsWith("/"))
                            url = u.Scheme + "://" + u.Host + response.Headers.Location;
                        else
                            url = response.Headers.Location.AbsoluteUri;
                        UriBuilder bld = new UriBuilder(url);
                        bld.Scheme = bas.Scheme;
                        url = bld.Uri.ToString();
                        cookies = wb.Cookies;
                        postData = null;
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

        private static void PopulateHeaders(HttpRequestMessage msg, NameValueCollection headers)
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

        //DrunkenProgrammer CookieCollection code Snippet

        #region CookieCollection

        public static CookieCollection GetAllCookiesFromHeader(IEnumerable<string> headers, string strHost)
        {
            CookieCollection cc = new CookieCollection();
            if (headers!=null && headers.Count()>0)
            {
                ArrayList al = ConvertCookieHeaderToArrayList(headers);
                cc = ConvertCookieArraysToCookieCollection(al, strHost);
            }
            return cc;
        }


        private static ArrayList ConvertCookieHeaderToArrayList(IEnumerable<string> headers)
        {
            ArrayList al = new ArrayList();
            foreach (string h in headers)
            {
                string strCookHeader = h.Replace("\r", "");
                strCookHeader = strCookHeader.Replace("\n", "");
                string[] strCookTemp = strCookHeader.Split(',');
                int i = 0;
                int n = strCookTemp.Length;
                while (i < n)
                {
                    if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                        i = i + 1;
                    }
                    else
                    {
                        al.Add(strCookTemp[i]);
                    }
                    i = i + 1;
                }
            }
            return al;
        }


        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
        {
            CookieCollection cc = new CookieCollection();

            int alcount = al.Count;
            string strEachCook;
            string[] strEachCookParts;
            for (int i = 0; i < alcount; i++)
            {
                strEachCook = al[i].ToString();
                strEachCookParts = strEachCook.Split(';');
                int intEachCookPartsCount = strEachCookParts.Length;
                string strCNameAndCValue = string.Empty;
                string strPNameAndPValue = string.Empty;
                string strDNameAndDValue = string.Empty;
                string[] NameValuePairTemp;
                Cookie cookTemp = new Cookie();

                for (int j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            int firstEqual = strCNameAndCValue.IndexOf("=");
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            NameValuePairTemp = strPNameAndPValue.Split('=');
                            if (NameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Path = NameValuePairTemp[1];
                            }
                            else
                            {
                                cookTemp.Path = "/";
                            }
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            NameValuePairTemp = strPNameAndPValue.Split('=');

                            if (NameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Domain = NameValuePairTemp[1];
                            }
                            else
                            {
                                cookTemp.Domain = strHost;
                            }
                        }
                        continue;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }
                cc.Add(cookTemp);
            }
            return cc;
        }

        #endregion
    }
}
