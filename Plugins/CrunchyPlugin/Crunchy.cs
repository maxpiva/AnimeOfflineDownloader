using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ADBaseLibrary;
using ADBaseLibrary.Helpers;
using ADBaseLibrary.Matroska;
using ADBaseLibrary.Mp4;
using CrunchyPlugin.XmlClasses;
using Ionic.Zlib;
using Newtonsoft.Json;

namespace CrunchyPlugin
{
    public class Crunchy : BaseDownloadPlugin, IDownloadPlugin
    {



        public const string ShowUrlS = "ShowUrl";
        public const string UpdateUrlS = "UpdateUrl";
        public const string ShowRegexS = "ShowRegex";
        public const string Show2RegexS = "Show2Regex";
        public const string EpsRegexS = "EpsRegex";
        public const string UpdRegexS = "UpdRegex";
        public const string SeasonRegexS = "SeasonRegex";
        public const string EpsShowImageRegexS = "EpsShowImage";
        public const string RTMPDumpArgsS = "RTMPDumpArgs";
        public const string RTMPDumpEXES = "RTMPDumpEXE";
        public const string DubbedAnimeS = "DubbedAnime";



        public override LibSettings LibSet { get; set;} = new LibSettings
        {
            { ShowUrlS,"http://www.crunchyroll.com/en/videos/{0}/alpha?group=all"},
            { UpdateUrlS,"http://www.crunchyroll.com/videos/{0}/updated/ajax_page?pg={1}" },
            { ShowRegexS,"<li\\sid.*?group_id=\"(?<id>.*?)\".*?<a.*?title=\"(?<title>.*?)\".*?href=\"(?<url>.*?)\".*?</a>.*?</li>"},
            { Show2RegexS, "\\#media_group_(?<id>.*?)\".*?\"description\":\"(?<desc>.*?)\""},
            { EpsRegexS,"<li\\sid=\"showview_videos_media_(?<id>.*?)\".*?<a.*?href=\"(?<url>.*?)\".*?<img.*?(src|data-thumbnailUrl)=\"(?<image>.*?)\".*?class=\"series-title\\sblock\\sellipsis\"\\sdir=\"auto\">(?<episode>.*?)</span>.*?class=\"short-desc\"\\sdir=\"auto\">(?<title>.*?)</p>.*?<script>.*?\"description\":(?<description>.*?),\"offsetLeft\":" },
            { UpdRegexS,"<li\\sid=\"media_group.*?group_id=\"(?<show>.*?)\".*?href=\"(?<url>.*?)\".*?<img.*?src=\"(?<image>.*?)\".*?<span.*?>(?<title>.*?)</span>.*?<span.*?>(?<ep>.*?)</span>"},
            { SeasonRegexS,"class=\"season-dropdown.*?title=\"(?<season>(.*?))\".*?</ul>" },
            { EpsShowImageRegexS,"<div\\sid=\"sidebar\".*?<img\\sitemprop=\"image\".*?src=\"(?<image>.*?)\".*?/>"},
            { RTMPDumpArgsS,"-r \"{0}\" -a \"{1}\" -y \"{2}\" -o \"{3}\"" },
            { RTMPDumpEXES, "rtmpdump.exe"},
            { DubbedAnimeS," dubbed| dub|anisava|colormail|digimon frontier|digimon tamers|digimon adventure|future card buddyfight|holy knight|karasuma kyoko no jikenbo|lady death movie|mizu no kotoba|rwby|sherlock hound|the mythical detective loki|valerian and laureline|zatch bell|crunchyroll collection|crunchycast|crunchyroll x tokyo" }
        };

        private static Regex showregex;
        private static Regex show2regex;
        private static Regex epsregex;
        private static Regex updregex;
        private static Regex seasonregex;
        private static Regex epsshowimage;

        public Crunchy()
        {
            LoadSettings("crunchy_settings.json");
            showregex = new Regex(LibSet[ShowRegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            show2regex = new Regex(LibSet[Show2RegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            epsregex = new Regex(LibSet[EpsRegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            updregex = new Regex(LibSet[UpdRegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            seasonregex = new Regex(LibSet[SeasonRegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            epsshowimage = new Regex(LibSet[EpsShowImageRegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        CrunchyPluginInfo _info = new CrunchyPluginInfo();
        private Dictionary<string, object> _global = null;


        #region Implementation

        public DownloadPluginInfo Information()
        {
            return _info;
        }

        public async Task<Response> SetRequirements(Dictionary<string,object> globalmetadata)
        {
            try
            {
                Response r = await _info.VerifyGlobalRequirements(globalmetadata);
                if (r.Status == ResponseStatus.Ok)
                {
                    string FFMPEGPath = globalmetadata.GetStringFromMetadata(CrunchyPluginInfo.FFMPEGPath);
                    string RTMPDumpPath = globalmetadata.GetStringFromMetadata(CrunchyPluginInfo.RTMPDumpPath);
                    if (string.IsNullOrEmpty(FFMPEGPath))
                    {
                        r.Status = ResponseStatus.MissingRequirement;
                        r.ErrorMessage = "'" + CrunchyPluginInfo.FFMPEGPath + "' Path is missing.";
                        return r;
                    }
                    if (string.IsNullOrEmpty(RTMPDumpPath))
                    {
                        r.Status = ResponseStatus.MissingRequirement;
                        r.ErrorMessage = "'" + CrunchyPluginInfo.RTMPDumpPath + "' Path is missing.";
                        return r;
                    }
                    if (!File.Exists(FFMPEGPath))
                    {
                        r.Status = ResponseStatus.InvalidArgument;
                        r.ErrorMessage = "'" + CrunchyPluginInfo.FFMPEGPath + "' Path is not valid.";
                    }
                    if (!File.Exists(RTMPDumpPath))
                    {
                        r.Status = ResponseStatus.InvalidArgument;
                        r.ErrorMessage = "'" + CrunchyPluginInfo.RTMPDumpPath + "' Path is not valid.";
                    }
                }
                _global = r.Status == ResponseStatus.Ok ? globalmetadata : null;
                return r;
            }
            catch (Exception e)
            {
                return new Response {ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError};
            }

        }

        public async Task<ISession> Authenticate(Dictionary<string,object> authenticationmetadata)
        {
            CrunchySession session = new CrunchySession();
            try
            {
                Response r = await _info.VerifyBaseAuthentication(authenticationmetadata);
                if (r.Status != ResponseStatus.Ok)
                {
                    r.PropagateError(session);
                    return session;
                }
                Dictionary<string, string> form = new Dictionary<string, string>();
                form.Add("formname", "RpcApiUser_Login");
                form.Add("fail_url", "http://www.crunchyroll.com/login");
                form.Add("name", authenticationmetadata.GetStringFromMetadata(DownloadPluginInfo.Username));
                form.Add("password", authenticationmetadata.GetStringFromMetadata(DownloadPluginInfo.Password));
                string postdata = form.PostFromDictionary();
                WebStream ws = await WebStream.Post("https://www.crunchyroll.com/?a=formhandler",form,null,LibSet[UserAgentS],null,null,SocketTimeout,false,null, _info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.Found)
                {
                    ws.Cookies = await SetLocale(LocaleFromString(authenticationmetadata.GetStringFromMetadata(CrunchyPluginInfo.Language)), ws.Cookies);
                    if (!VerifyLogin(ws.Cookies))
                    {
                        session.Status = ResponseStatus.InvalidLogin;
                        session.ErrorMessage = "Invalid Account Information";
                        session.cookies = new Dictionary<string, string>();
                    }
                    else
                    {
                        session.cookies = ws.Cookies.ToDictionary();
                        session.Status = ResponseStatus.Ok;
                    }
                }
                else
                {
                    SetWebError(session);
                }
                ws?.Dispose();
            }
            catch (Exception e)
            {
                session.Status=ResponseStatus.SystemError;
                session.ErrorMessage = e.ToString();
            }
            return session;
        }

        public ISession Deserialize(Dictionary<string,string> data)
        {
            CrunchySession ses=new CrunchySession();
            ses.cookies = data;
            return ses;
        }

        public async Task<Shows> Shows(ISession session)
        {
            try
            {
                Task<Shows> c1 = Shows(session, ShowType.Anime);
                Task<Shows> c2 = Shows(session, ShowType.Drama);
                Task<Shows> c3 = Shows(session, ShowType.Pop);
                await Task.WhenAll(c1, c2, c3);
                c3.Result.PropagateError(c2.Result);
                c2.Result.PropagateError(c1.Result);
                c1.Result.Items = c1.Result.Items.Concat(c2.Result.Items).Concat(c3.Result.Items).ToList();
                return c1.Result;
            }
            catch (Exception e)
            {
                return new Shows {ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError};
            }

        }

        public async Task<Shows> Shows(ISession session, ShowType type)
        {
            try
            {
                CrunchySession s = session as CrunchySession;
                if (s == null)
                    return new Shows { ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument };
                Shows ret = new Shows();
                ret.Items = new List<Show>();
                string url = string.Format(LibSet[ShowUrlS], ShowFromType(type));
                WebStream ws = await WebStream.Get(url,null,LibSet[UserAgentS],null,s.cookies.ToCookieCollection(),SocketTimeout,true,null,_info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {
                    if (!VerifyLogin(ws.Cookies))
                        SetLoginError(ret);
                    else
                    {
                        StreamReader rd = new StreamReader(ws);
                        string dta = rd.ReadToEnd();
                        rd.Dispose();
                        MatchCollection col = showregex.Matches(dta);
                        Dictionary<string, Show> ls = new Dictionary<string, Show>();
                        foreach (Match m in col)
                        {
                            if (m.Success)
                            {
                                Show cs = new Show();
                                cs.Id = int.Parse(m.Groups["id"].Value).ToString();
                                cs.Name = m.Groups["title"].Value;
                                cs.Type = type;
                                cs.PluginName = CrunchyPluginInfo.PluginName;
                                cs.PluginMetadata.Add("Url",new Uri("http://www.crunchyroll.com" + m.Groups["url"].Value).ToString());
                                ls.Add(cs.Id, cs);
                            }
                        }
                        col = show2regex.Matches(dta);
                        foreach (Match m in col)
                        {
                            if (m.Success)
                            {
                                string id = int.Parse(m.Groups["id"].Value).ToString();
                                Show ccd = ls[id];
                                ccd.Description = Regex.Unescape(m.Groups["desc"].Value);
                            }
                        }
                        ret.Items = ls.Values.OrderBy(a => a.Name).Cast<Show>().ToList();
                        ret.Status = ResponseStatus.Ok;
                    }
                }
                else
                {
                    SetWebError(ret);
                }
                ws?.Dispose();
                return ret;
            }
            catch (Exception e)
            {
                return new Shows {ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError};
            }
          
        }     

        public async Task<Episodes> Episodes(ISession session, Show show)
        {
            try
            {
                CrunchySession s = session as CrunchySession;
                if (s == null)
                    return new Episodes { ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument };
                if (!show.PluginMetadata.ContainsKey("Url"))
                    return new Episodes { ErrorMessage = "Invalid Show", Status = ResponseStatus.InvalidArgument };
                Episodes ret = new Episodes();
                ret.Items = new List<Episode>();
                WebStream ws = await WebStream.Get(show.PluginMetadata["Url"], null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout, true, null, _info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {
                    if (!VerifyLogin(ws.Cookies))
                        SetLoginError(ret);
                    else
                    {
                        StreamReader rd = new StreamReader(ws);
                        string dta = rd.ReadToEnd();
                        rd.Dispose();
                        ret.Status = ResponseStatus.Ok;
                        ret.Items = new List<Episode>();
                        Match sm = epsshowimage.Match(dta);
                        if (sm.Success)
                            ret.ImageUri = new Uri(sm.Groups["image"].Value);
                        MatchCollection scol = seasonregex.Matches(dta);
                        int seasonnum = scol.Count;
                        if (scol.Count == 0)
                        {
                            AddEpisodes(ret, show, dta, String.Empty, 1);
                        }
                        else
                        {
                            foreach (Match sma in scol)
                            {
                                if (sma.Success)
                                {
                                    string data = sma.Value;
                                    string seasoname = sma.Groups["season"].Value;
                                    AddEpisodes(ret, show, data, seasoname, seasonnum);
                                }
                                seasonnum--;
                            }
                        }
                        int index = ret.Items.Count;
                        for (int x = 0; x < ret.Items.Count; x++)
                        {
                            ret.Items[x].Index = index;
                            index--;
                        }
                        ret.Items.Reverse();
                    }
                }
                else
                {
                    SetWebError(ret);
                }
                ws?.Dispose();
                return ret;
            }
            catch (Exception e)
            {
                return new Episodes {ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError};
            }
            
        }

        public async Task<Response> Download(ISession session, Episode episode, string template, string downloadpath, Quality quality, Format formats, CancellationToken token, IProgress<DownloadInfo> progress)
        {
            try
            {
                KeyValuePair<string, string> defl = AudioLanguageFromEpisode(episode);
                CrunchySession sess = session as CrunchySession;

                if (sess == null)
                    return new Response { ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument };
                if (!episode.PluginMetadata.ContainsKey("Url"))
                    return new Response { ErrorMessage = "Invalid Episode", Status = ResponseStatus.InvalidArgument };
                DownloadInfo dp = new DownloadInfo { FileName = TemplateParser.FilenameFromEpisode(episode, quality, template), Format = formats, Percent = 0, Quality = quality };

                token.ThrowIfCancellationRequested();
                dp.Languages = new List<string>();
                dp.Percent = 1;
                dp.Status = "Getting Metadata";
                progress.Report(dp);
                Config c = await GetStandardConfig(sess, episode.Id, quality);
                if (c == null)
                    return new Response { ErrorMessage = "Unable to retrieve metadata", Status = ResponseStatus.WebError };
                if (!string.IsNullOrEmpty(c.Stream_info.Error))
                    return new Response { ErrorMessage = "Login Required", Status = ResponseStatus.LoginRequired };
                List<Task<CrunchySubtitleInfo>> subTasks = new List<Task<CrunchySubtitleInfo>>();
                token.ThrowIfCancellationRequested();
                if ((c.Subtitles != null && c.Subtitles.Subtitle != null && c.Subtitles.Subtitle.Count > 0))
                {
                    foreach (Subtitle s in c.Subtitles.Subtitle)
                    {
                        subTasks.Add(GetSubtitle(sess, int.Parse(s.Id)));
                    }
                    dp.Percent = 2;
                    dp.Status = "Gettings subtitles";
                    progress.Report(dp);
                    await Task.WhenAll(subTasks);
                    foreach (CrunchySubtitleInfo s in subTasks.Select(a => a.Result))
                        dp.Languages.Add(s.Title);
                }
                else
                {
                    dp.Languages.Add("Hardcoded");
                }
                dp.Quality = c.Stream_info.Metadata.Height.ToQuality();
                dp.FileName = TemplateParser.FilenameFromEpisode(episode, dp.Quality, template);
                dp.FullPath = Path.Combine(downloadpath, dp.FileName);
                string intermediatefile = Path.Combine(downloadpath, dp.FileName + ".tm1");
                KeyValuePair<string, string> hh = ParseHost(c.Stream_info.Host);
                string args = string.Format(LibSet[RTMPDumpArgsS], hh.Key, hh.Value, c.Stream_info.File, intermediatefile);
                List<string> todeleteFiles = new List<string>();
                todeleteFiles.Add(intermediatefile);
                dp.Percent = 3;
                dp.Status = "Downloading video";
                progress.Report(dp);
                RTMPDumpParser rtmp = new RTMPDumpParser();
                double dbl = 92;
                double final = 0;
                rtmp.OnProgress += (val) =>
                {
                    final = val;
                    dp.Percent = (val * dbl / 100) + 3;
                    progress.Report(dp);
                };
                await rtmp.Start(LibSet[RTMPDumpEXES], args, token);
                if (final < 100)
                    return new Response { ErrorMessage = "Error downloading video", Status = ResponseStatus.TransferError };
                List<CrunchySubtitleInfo> sis = subTasks.Select(a => a.Result).ToList();
                string inputs = string.Empty;
                string maps = String.Empty;
                int pp = 0;
                foreach (CrunchySubtitleInfo k in sis)
                {
                    string pth = Path.GetTempFileName() + ".ass";
                    todeleteFiles.Add(pth);
                    File.WriteAllText(pth, k.Ass);
                    inputs += "-i \"" + pth + "\" ";
                    maps += GetFFMPEGSubtitleArguments(pp + 1, pp, k.Language, k.Title);
                    pp++;
                }
                dp.Size = await ReMux(intermediatefile, inputs, maps, formats, defl.Key, defl.Value, 96, 4, dp, progress, token);
                dp.Percent = 100;
                dp.Status = "Finished";
                progress.Report(dp);
                foreach (string s in todeleteFiles)
                {
                    try
                    {
                        File.Delete(s);

                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                return new Response { Status = ResponseStatus.Ok, ErrorMessage = "OK" };
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                    return new Response {ErrorMessage = "Canceled", Status = ResponseStatus.Canceled};
                return new Response {ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError};
            }
            
        }
      
        public async Task<Updates> Updates(ISession session)
        {
            try
            {
                if (!UpdateHistory.IsLoaded)
                    UpdateHistory.Load();
                CrunchySession s = session as CrunchySession;
                if (s == null)
                    return new Updates {ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument};

                Task<UpdateResponse> c1 = Updates(s, ShowType.Anime);
                Task<UpdateResponse> c2 = Updates(s, ShowType.Drama);
                Task<UpdateResponse> c3 = Updates(s, ShowType.Pop);
                await Task.WhenAll(c1, c2, c3);
                c3.Result.PropagateError(c2.Result);
                c2.Result.PropagateError(c1.Result);
                if (c1.Result.Status != ResponseStatus.Ok)
                {
                    Updates k = new Updates();
                    c1.Result.PropagateError(k);
                    return k;
                }
                c1.Result.Found = c1.Result.Found.Concat(c2.Result.Found).Concat(c3.Result.Found).ToList();
                Episode[] nfnd =
                    c1.Result.NotFound.Concat(c2.Result.NotFound).Concat(c3.Result.NotFound).ToArray();
                Array.Reverse(nfnd);
                c1.Result.NotFound = nfnd.ToList();
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                List<Task<Episode>> ms = new List<Task<Episode>>();
                foreach (Episode m in c1.Result.NotFound)
                {
                    ms.Add(GetEpisodeUpdate(s, m, datetime));
                }
                if (ms.Count > 0)
                {
                    while (ms.Count > 0)
                    {
                        int max = 5;
                        if (ms.Count < 5)
                            max = ms.Count;
                        Task<Episode>[] tsks = new Task<Episode>[max];
                        for (int x = 0; x < max; x++)
                        {
                            tsks[x] = ms[0];
                            ms.Remove(ms[0]);
                        }
                        await Task.WhenAll(tsks);
                        for (int x = max - 1; x >= 0; x--)
                        {
                            if (tsks[x].Result != null)
                            {
                                c1.Result.Found.Add(tsks[x].Result);
                            }
                        }
                    }
                    UpdateHistory.Save();
                }
                Updates n = new Updates();
                n.Items = c1.Result.Found.ToList();
                int cnt = 1;
                foreach (Episode epu in n.Items)
                    epu.Index = cnt++;
                n.Status = ResponseStatus.Ok;
                n.ErrorMessage = "OK";
                return n;
            }
            catch (Exception e)
            {
                return new Updates {ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError};
            }
           
        }

        public void Exit()
        {

        }

        #endregion

        #region Helpers


        private class UpdateResponse : Response
        {
            public List<Episode> Found { get; set; } = new List<Episode>();

            public List<Episode> NotFound { get; set; } = new List<Episode>();
        }

        private async Task<UpdateResponse> Updates(CrunchySession s, ShowType t)
        {
            int startpage = 0;
            UpdateResponse ret = new UpdateResponse();
            bool end = false;
            do
            {
                string url = string.Format(LibSet[UpdateUrlS], ShowFromType(t), startpage);
                WebStream ws = await WebStream.Get(url, null,LibSet[UserAgentS], null, s.cookies.ToCookieCollection(),SocketTimeout,true,null,_info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {
                    if (!VerifyLogin(ws.Cookies))
                    {
                        ws.Dispose();
                        SetLoginError(ret);
                        return ret;
                    }
                    StreamReader rd = new StreamReader(ws);
                    string dta = rd.ReadToEnd();
                    rd.Dispose();
                    MatchCollection scol = updregex.Matches(dta);
                    if (scol.Count == 0)
                        end = true;
                    foreach (Match m in scol)
                    {
                        string show = m.Groups["show"].Value;
                        string image = m.Groups["image"].Value;
                        string title = WebUtility.HtmlDecode(m.Groups["title"].Value);
                        string ep = m.Groups["ep"].Value;
                        Uri ur = new Uri("http://www.crunchyroll.com" + m.Groups["url"].Value);
                        int a = ep.IndexOf("&ndash;", StringComparison.InvariantCulture);
                        if (a >= 0)
                        {
                            ep = ep.Substring(0, a).Trim();
                            string tag = CrunchyPluginInfo.PluginName + "|" + show + "|" + ep;
                            if (UpdateHistory.Exists(tag))
                            {
                                Episode c = JsonConvert.DeserializeObject<Episode>(UpdateHistory.Get(tag));
                                ret.Found.Add(c);
                            }
                            else
                            {
                                Episode p = new Episode();
                                p.PluginMetadata.Add("Url",ur.ToString());
                                p.ShowName = title;
                                p.ShowId = show;
                                p.PluginName = CrunchyPluginInfo.PluginName;
                                p.UniqueTag = tag;
                                p.ImageUri = new Uri(image);
                                p.Type = t;
                                ret.NotFound.Add(p);

                            }
                        }

                    }


                }
                else
                {
                    ws?.Dispose();
                    SetWebError(ret);
                    return ret;
                }
                ws?.Dispose();
                startpage++;
            } while (!end);
            return ret;
        }

        private async Task<Episode> GetEpisodeUpdate(CrunchySession s, Episode placeholder, string datetime)
        {

            try
            {
                WebStream ws = await WebStream.Get(placeholder.PluginMetadata["Url"], null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout,true,null,_info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {
                    if (!VerifyLogin(ws.Cookies))
                    {
                        ws?.Dispose();
                        return null;
                    }
                    StreamReader rd = new StreamReader(ws);
                    string dta = rd.ReadToEnd();
                    rd.Dispose();
                    Episodes eps = new Episodes();
                    eps.Items = new List<Episode>();
                    Show show = new Show();
                    show.PluginName = placeholder.PluginName;
                    show.Id = placeholder.ShowId;

                    MatchCollection scol = seasonregex.Matches(dta);
                    int seasonnum = scol.Count;
                    if (scol.Count == 0)
                    {
                        AddEpisodes(eps, show, dta, String.Empty, 1, true);
                    }
                    else
                    {
                        Match sma = scol[0];
                        if (sma.Success)
                        {
                            string data = sma.Value;
                            string seasoname = sma.Groups["season"].Value;
                            AddEpisodes(eps, show, data, seasoname, seasonnum, true);
                        }
                    }
                    if (eps.Items.Count == 0)
                    {
                        ws?.Dispose();
                        return null;
                    }
                    Episode ep = eps.Items[0];
                    placeholder.PluginMetadata["Url"] = ep.PluginMetadata["Url"];
                    placeholder.ImageUri = ep.ImageUri;
                    placeholder.Description = ep.Description;
                    placeholder.EpisodeAlpha = ep.EpisodeAlpha;
                    placeholder.EpisodeNumeric = ep.EpisodeNumeric;
                    placeholder.Id = ep.Id;
                    placeholder.SeasonAlpha = ep.SeasonAlpha;
                    placeholder.SeasonNumeric = ep.SeasonNumeric;
                    placeholder.Name = ep.Name;
                    placeholder.DateTime = datetime;
                    UpdateHistory.Add(placeholder.UniqueTag, JsonConvert.SerializeObject(placeholder));
                    ws?.Dispose();
                    return placeholder;
                }
            }
            catch (Exception )
            {
                return null;
            }
            return null;
        }




        private string LocaleFromString(string str)
        {
            switch (str)
            {
                case "English (US)":
                    return "enUS";
                case "English (UK)":
                    return "enGB";
                case "Español":
                    return "esLA";
                case "Español (España)":
                    return "esES";
                case "Português (Brasil)":
                    return "ptBR";
                case "Português (Portugal)":
                    return "ptPT";
                case "Français (France)":
                    return "frFR";
                case "Deutsch":
                    return "deDE";
                case "العربية":
                    return "arME";
                case "Italiano":
                    return "itIT";
            }
            return "enUS";
        }

        private bool VerifyLogin(CookieCollection cookies)
        {
            Dictionary<string, string> cook = cookies.ToDictionary();
            if (cook.ContainsKey("c_userid") && cook.ContainsKey("c_userkey"))
                return true;
            return false;
        }


        private string CreateKeyString(int length, int module, int start1, int start2)
        {
            int[] arr = new int[length + 2];
            arr[0] = start1;
            arr[1] = start2;
            string ret = string.Empty;
            for (int x = 2; x < length + 2; x++)
            {
                arr[x] = arr[x - 1] + arr[x - 2];
                char c = (char)((arr[x] % module) + 33);
                ret += c;
            }
            return ret;
        }

        private byte[] GenerateKey(int mediaid)
        {
            long eq1 = (int)((int)(Math.Floor(Math.Sqrt(6.9) * Math.Pow(2, 25))) ^ mediaid);
            long eq2 = (int)(Math.Floor(Math.Sqrt(6.9) * Math.Pow(2, 25)));
            long eq3 = (mediaid ^ eq2) ^ ((mediaid ^ eq2) >> 3) ^ eq1 * 32;
            SHA1Managed mng = new SHA1Managed();
            string init = CreateKeyString(20, 97, 1, 2) + eq3.ToString();
            byte[] data = mng.ComputeHash(Encoding.UTF8.GetBytes(init), 0, init.Length);
            return data;
        }

        private void DecodeSubtitle(int subtitleId, string ivBase64, string dataBase64, ref CrunchySubtitleInfo si)
        {
            byte[] key = GenerateKey(subtitleId).Concat(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }).ToArray();
            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] data = Convert.FromBase64String(dataBase64);
            AesManaged mng = new AesManaged();
            mng.Mode = CipherMode.CBC;
            mng.Padding = PaddingMode.None;
            ICryptoTransform tr = mng.CreateDecryptor(key, iv);
            byte[] kk = tr.TransformFinalBlock(data, 0, data.Length);
            MemoryStream ms = new MemoryStream();
            ZlibStream stream = new ZlibStream(new MemoryStream(kk), Ionic.Zlib.CompressionMode.Decompress);
            stream.CopyTo(ms);
            ms.Position = 0;
            XmlSerializer serializer = new XmlSerializer(typeof(SubtitleScript));
            SubtitleScript script = (SubtitleScript)serializer.Deserialize(ms);
            si.Title = script.Title;
            si.Ass = script.ToAss();
        }



        KeyValuePair<string, string> ParseHost(string host)
        {
            int x = 0;
            int idx = 0;
            do
            {
                idx = host.IndexOf("/", idx + 1, StringComparison.InvariantCulture);
                x++;
            } while (x != 3);
            string str2 = host.Substring(idx + 1);
            idx = host.IndexOf("/", idx + 1, StringComparison.InvariantCulture);
            if (idx==-1)
                idx = host.IndexOf("?", idx + 1, StringComparison.InvariantCulture);
            string str1 = host.Substring(0, idx);
            return new KeyValuePair<string, string>(str1, str2);
        }

        KeyValuePair<string, string> QualityConvert(Quality q)
        {
            string vf = "106";
            string res = "";
            switch (q)
            {
                case Quality.Quality360P:
                    res = "60";
                    break;
                case Quality.Quality480P:
                    res = "61";
                    break;
                case Quality.Quality720P:
                    res = "62";
                    break;
                case Quality.Quality1080P:
                    res = "80";
                    vf = "108";
                    break;
            }
            return new KeyValuePair<string, string>(res, vf);
        }



        private async Task<Config> GetStandardConfig(CrunchySession token, string epid, Quality qlty)
        {
            KeyValuePair<string, string> vf = QualityConvert(qlty);

            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("req", "RpcApiVideoPlayer_GetStandardConfig");
            form.Add("media_id", epid);
            form.Add("video_format", vf.Value);
            form.Add("video_quality", vf.Key);
            form.Add("auto_play", "1");
            form.Add("show_pop_out_controls", "1");
            form.Add("current_page", "http://www.crunchyroll.com/");
            WebStream ws = await WebStream.Post("http://www.crunchyroll.com/xml/",form,null,LibSet[UserAgentS], null, token.cookies.ToCookieCollection(), SocketTimeout,true,"http://www.crunchyroll.com/swf/StandardVideoPlayer.swf", _info.ProxyFromGlobalRequirements(_global));
            string dta = null;
            if (ws!=null && ws.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(ws);
                string str = reader.ReadToEnd();
                int be = str.IndexOf("<stream_info>",StringComparison.InvariantCulture);
                int en = str.IndexOf("</default:preload>", StringComparison.InvariantCulture);
                str = "<config>" + str.Substring(be, en - be) + "</config>";
                XmlSerializer ser = new XmlSerializer(typeof(Config));
                reader.Dispose();
                ws.Dispose();
                return (Config)ser.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(str)));
            }
            ws?.Dispose();
            return null;
        }

        private async Task<CrunchySubtitleInfo> GetSubtitle(CrunchySession token, int subid)
        {

            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("req", "RpcApiSubtitle_GetXml");
            form.Add("subtitle_script_id", subid.ToString());
            WebStream ws = await WebStream.Post("http://www.crunchyroll.com/xml/", form, null, LibSet[UserAgentS], null, token.cookies.ToCookieCollection(), SocketTimeout,true,"http://www.crunchyroll.com/swf/StandardVideoPlayer.swf", _info.ProxyFromGlobalRequirements(_global));
            string dta = null;
            if (ws.StatusCode == HttpStatusCode.OK)
            {
                XmlSerializer ser = new XmlSerializer(typeof(Subtitle));
                Subtitle s = (Subtitle)ser.Deserialize(ws);
                CrunchySubtitleInfo si = new CrunchySubtitleInfo();
                DecodeSubtitle(int.Parse(s.Id), s.Iv, s.Data, ref si);
                si.Language = ADBaseLibrary.Subtitles.Languages.CodeFromOriginalLanguage(si.Title);
                ws.Dispose();
                return si;
            }
            ws?.Dispose();
            return null;
        }

        private async Task<CookieCollection> SetLocale(string locale, CookieCollection cookies)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("req", "RpcApiTranslation_SetLang");
            form.Add("locale", "enUS");

            WebStream ws = await WebStream.Post("http://www.crunchyroll.com/ajax/", form, null, LibSet[UserAgentS], null, cookies, SocketTimeout, false, null, _info.ProxyFromGlobalRequirements(_global));
            if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                return ws.Cookies;
            return new CookieCollection();
        }

        private string ShowFromType(ShowType type)
        {
            string tp = "anime";
            if (type == ShowType.Drama)
                tp = "drama";
            else if (type == ShowType.Pop)
                tp = "pop";
            return tp;
        }

        private KeyValuePair<string, string> AudioLanguageFromEpisode(Episode ep)
        {
            string s1 = ep.ShowName.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToLower(CultureInfo.InvariantCulture);
            string s2 = string.IsNullOrEmpty(ep.SeasonAlpha) ? string.Empty : ep.SeasonAlpha.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToLower(CultureInfo.InvariantCulture);
            string s3 = string.IsNullOrEmpty(ep.Name) ? string.Empty : ep.Name.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToLower(CultureInfo.InvariantCulture);
            foreach (string s in LibSet[DubbedAnimeS].Split('|'))
            {
                if (s1.Contains(s) || s2.Contains(s) || s3.Contains(s))
                {
                    return new KeyValuePair<string, string>("eng","English");
                }
            }
            return new KeyValuePair<string, string>("jpn", "日本語");

        }

        private void AddEpisodes(Episodes ret, Show show, string data, string seasoname, int seasonnum, bool firstone = false)
        {
            MatchCollection col = epsregex.Matches(data);
            foreach (Match m in col)
            {
                if (m.Success)
                {
                    Episode e = new Episode();
                    e.Id = int.Parse(m.Groups["id"].Value).ToString();
                    e.PluginMetadata["Url"]=new Uri("http://www.crunchyroll.com" + m.Groups["url"].Value).ToString();
                    e.Name = WebUtility.HtmlDecode(m.Groups["title"].Value).Trim();
                    e.EpisodeAlpha = WebUtility.HtmlDecode(m.Groups["episode"].Value).Trim();
                    e.SeasonNumeric = seasonnum;
                    e.SeasonAlpha = seasoname;
                    e.ShowName = show.Name;
                    e.ShowId = show.Id;
                    e.Type = show.Type;
                    e.PluginName = show.PluginName;
                    string number = Regex.Replace(e.EpisodeAlpha, "[^0-9]", string.Empty).Trim();
                    int n = 0;
                    if (int.TryParse(number, out n))
                        e.EpisodeNumeric = n;
                    else
                        e.EpisodeNumeric = 0;
                    string desc1 = m.Groups["description"].Value;
                    e.ImageUri = new Uri(m.Groups["image"].Value);
                    e.Description = Regex.Unescape(desc1.Substring(1, desc1.Length - 2));
                    if (!m.Groups["image"].Value.Contains("coming_soon"))
                        ret.Items.Add(e);
                    if (ret.Items.Count > 0 && firstone)
                        return;
                }
            }
        }
        #endregion
    }
}
