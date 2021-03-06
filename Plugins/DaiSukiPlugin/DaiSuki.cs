﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using ADBaseLibrary.AdobeHDS;
using ADBaseLibrary.AdobeHDS.FlashWrapper;
using ADBaseLibrary.Helpers;
using ADBaseLibrary.Subtitles;
using DaiSukiPlugin.JsonXmlClasses;
using Newtonsoft.Json;
using Response = ADBaseLibrary.Response;

namespace DaiSukiPlugin
{
    public class DaiSuki : BaseDownloadPlugin, IDownloadPlugin
    {
        DaiSukiPluginInfo _info = new DaiSukiPluginInfo();
        private Dictionary<string, object> _global = null;
        private Dictionary<string, object> _authmeta = null;

        public const string ShowRegexS = "ShowRegex";
        public const string Show2RegexS = "Show2Regex";
        public const string Show3RegexS = "Show3Regex";
        public const string ImageServerS = "ImageServer";
        public const string BgnWrapperS = "BgnWrapper";
        public const string FlashVarsS = "FlashVars";
        public const string Flash2VarsS = "Flash2Vars";
        public const string PublicKeyS = "PublicKey";
        public const string BaseHostS = "BaseHost";

        public override LibSettings LibSet { get; set; } = new LibSettings
        {
            { ShowRegexS,"<div.*?class=\"latestMovieThumbnail\".*?<img.*?delay=\"(?<image>.*?)\".*?<p.*?class=\"episodeNumber\">(?<episode>.*?)</p>"},
            { Show2RegexS,"<div.*?class=\"thumbnail\">.*?<img.*?delay=\"(?<image>.*?)\".*?<a.*?>(?<episode>.*?)</a>" },
            { Show3RegexS,"daisuki/.*?/.*?/(?<id>.*?)/movie.jpg" },
            { ImageServerS,"http://img.daisuki.net/"},
            { BgnWrapperS, "<script.*?src=\"(?<wrapper>.*?clientlibs_anime_watch.min\\.js.*?)\""},
            { FlashVarsS,"flashvars.*?=*.?{(?<vars>.*?)}" },
            { Flash2VarsS,"(['\"])(?<name>.*?)(['\"])\\s*:\\s*(['\"])(?<value>.*?)(['\"])"},
            { PublicKeyS,"-----BEGIN PUBLIC KEY-----(?<key>.*?)-----END PUBLIC KEY-----" },
            { BaseHostS,"https://www.daisuki.net" }
        };
        private static Regex showregex;
        private static Regex show2regex;
        private static Regex bgnWrapper;
        private static Regex flashVars;
        private static Regex flash2Vars;
        private static Regex publicKey;

        public DaiSuki()
        {
            LoadSettings("daisuki_settings.json");
            showregex = new Regex(LibSet[ShowRegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            show2regex = new Regex(LibSet[Show2RegexS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            bgnWrapper = new Regex(LibSet[BgnWrapperS], RegexOptions.Compiled);
            flashVars = new Regex(LibSet[FlashVarsS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            flash2Vars = new Regex(LibSet[Flash2VarsS], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            publicKey = new Regex(LibSet[PublicKeyS], RegexOptions.Compiled);

        }




        public DownloadPluginInfo Information()
        {
            DecryptForm.Init();
            return _info;
        }

        public async Task<Response> SetRequirements(Dictionary<string, object> globalmetadata)
        {
            try
            {
                Response r = await _info.VerifyGlobalRequirements(globalmetadata);
                if (r.Status == ResponseStatus.Ok)
                {
                    string FFMPEGPath = globalmetadata.GetStringFromMetadata(DaiSukiPluginInfo.FFMPEGPath);
                    string RTMPDumpPath = globalmetadata.GetStringFromMetadata(DaiSukiPluginInfo.RTMPDumpPath);
                    if (string.IsNullOrEmpty(FFMPEGPath))
                    {
                        r.Status = ResponseStatus.MissingRequirement;
                        r.ErrorMessage = "'" + DaiSukiPluginInfo.FFMPEGPath + "' Path is missing.";
                        return r;
                    }
                    if (string.IsNullOrEmpty(RTMPDumpPath))
                    {
                        r.Status = ResponseStatus.MissingRequirement;
                        r.ErrorMessage = "'" + DaiSukiPluginInfo.RTMPDumpPath + "' Path is missing.";
                        return r;
                    }
                    if (!File.Exists(FFMPEGPath))
                    {
                        r.Status = ResponseStatus.InvalidArgument;
                        r.ErrorMessage = "'" + DaiSukiPluginInfo.FFMPEGPath + "' Path is not valid.";
                    }
                    if (!File.Exists(RTMPDumpPath))
                    {
                        r.Status = ResponseStatus.InvalidArgument;
                        r.ErrorMessage = "'" + DaiSukiPluginInfo.RTMPDumpPath + "' Path is not valid.";
                    }
                }
                _global = r.Status == ResponseStatus.Ok ? globalmetadata : null;
                return r;
            }
            catch (Exception e)
            {
                return new Response { ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError };
            }

        }

        public async Task<ISession> Authenticate(Dictionary<string, object> authenticationmetadata)
        {
            _authmeta = authenticationmetadata;
            DaiSukiSession session = new DaiSukiSession();
            try
            {
                Response r = await _info.VerifyBaseAuthentication(authenticationmetadata);
                if (r.Status != ResponseStatus.Ok)
                {
                    r.PropagateError(session);
                    return session;
                }
                Dictionary<string, string> form = new Dictionary<string, string>();
                form.Add("emailAddress", authenticationmetadata.GetStringFromMetadata(DownloadPluginInfo.Username));
                form.Add("password", authenticationmetadata.GetStringFromMetadata(DownloadPluginInfo.Password));
                form.Add("keepLogin", "on");


                
                WebStream ws = await WebStream.Post("https://www.daisuki.net/bin/SignInServlet.html/input", form, null, LibSet[UserAgentS], null, null, SocketTimeout, true, "http://www.daisuki.net/", _info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {

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
                session.Status = ResponseStatus.SystemError;
                session.ErrorMessage = e.ToString();

            }
            return session;

        }

        public ISession Deserialize(Dictionary<string, string> data)
        {
            DaiSukiSession ses = new DaiSukiSession();
            ses.cookies = data;
            return ses;
        }

        public async Task<Shows> Shows(ISession session)
        {
            return await Shows(session, true);
        }

        private async Task<Shows> Shows(ISession session, bool order)
        {
            try
            {
                DaiSukiSession s = session as DaiSukiSession;
                if (s == null)
                    return new Shows { ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument };
                Shows ret = new Shows();
                ret.Items = new List<Show>();
                WebStream ws = await WebStream.Get("http://www.daisuki.net/bin/wcm/searchAnimeAPI?api=anime_list&searchOptions=&currentPath=%2Fcontent%2Fdaisuki%2Fus%2Fen", null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout, true, null, _info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {
                    if (!VerifyLogin(ws.Cookies))
                        SetLoginError(ret);
                    else
                    {
                        
                        StreamReader rd = new StreamReader(ws);
                        string dta = rd.ReadToEnd();
                        BaseResponse<Anime> animes = JsonConvert.DeserializeObject<BaseResponse<Anime>>(dta);
                        rd.Dispose();
                        if (animes.Response == null || animes.Response.Count == 0)
                        {
                            if (animes.Error != null)
                            {
                                ret.ErrorMessage = animes.Error;
                                ret.Status=ResponseStatus.WebError;
                                return ret;
                            }
                            SetWebError(ret);
                            ws?.Dispose();
                            return ret;
                        }
                        foreach (Anime a in animes.Response)
                        {
                            Show cs=new Show();
                            cs.PluginName = DaiSukiPluginInfo.PluginName;
                            cs.Id = a.Id.ToString();
                            cs.Type=ShowType.Anime;
                            cs.Description = a.Synopsis;
                            cs.Name = a.Title;
                            cs.PluginMetadata.Add("Url", new Uri("http://www.daisuki.net/anime/detail/"+a.AdId).ToString());
                            ret.Items.Add(cs);
                        }
                        if (order)
                            ret.Items = ret.Items.OrderBy(a => a.Name).ToList();
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
                return new Shows { ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError };
            }
        }

        public async Task<Shows> Shows(ISession session, ShowType type)
        {
            if(type==ShowType.Anime)
                return await Shows(session);
            Shows s = new Shows();
            s.Status = ResponseStatus.Ok;
            return s;
        }

        private Episode GetEpisode(Show s, Match m)
        {
            
            var ep=new Episode();
            string imgurl = "http://www.daisuki.net/"+m.Groups["image"].Value;
            ep.ImageUri=new Uri(imgurl);
            int r = imgurl.LastIndexOf("/");
            if (r >= 0)
            {
                int n = imgurl.LastIndexOf("/", r - 1);
                if (n >= 0)
                {
                    ep.Id = imgurl.Substring(n + 1, r - n - 1);
                }
            }
            ep.EpisodeAlpha = m.Groups["episode"].Value;
            ep.SeasonNumeric = 0;
            ep.SeasonAlpha = string.Empty;
            ep.EpisodeNumeric = 0;
            int val = 0;
            if (int.TryParse(ep.EpisodeAlpha,out val))
                ep.EpisodeNumeric = val;
            ep.Name = "Episode " + ep.EpisodeAlpha;
            ep.PluginName = DaiSukiPluginInfo.PluginName;
            string nam = s.PluginMetadata["Url"];
            r = nam.LastIndexOf("/");
            nam = nam.Substring(r + 1);
            string url = string.Format("http://www.daisuki.net/us/en/anime/watch.{0}.{1}.html",nam,ep.Id);
            ep.PluginMetadata.Add("Url", url);
            ep.ShowId = s.Id;
            ep.ShowName = s.Name;
            ep.Type = s.Type;
            return ep;
        }
        public async Task<Episodes> Episodes(ISession session, Show show)
        {
            try
            {
                DaiSukiSession s = session as DaiSukiSession;
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
                        ret.ImageUri = new Uri(LibSet[ImageServerS] + "/img/series/" + show.Id + "/340_506.jpg");
                        Match sm = showregex.Match(dta);
                        if (sm.Success)
                        {
                            ret.Items.Add(GetEpisode(show, sm));
                        }
                        MatchCollection scol = show2regex.Matches(dta);
                        foreach (Match sma in scol)
                        {
                            if (sma.Success)
                            {
                                ret.Items.Add(GetEpisode(show, sma));
                            }
                        }
                        ret.Items = ret.Items.OrderBy(a => a.EpisodeNumeric).ToList();
                        for (int x = 0; x < ret.Items.Count; x++)
                        {
                            ret.Items[x].Index = x + 1;
                        }
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
                return new Episodes { ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError };
            }
        }

        public async Task<Response> Download(ISession session, Episode episode, string template, string downloadpath, Quality quality, Format formats,
            CancellationToken token, IProgress<DownloadInfo> progress)
        {
            try
            {
                string deflangcode = "jpn";
                string deflang = "日本語";
                Response ret = new Response();
                DaiSukiSession s = session as DaiSukiSession;
                if (s == null)
                    return new Response { ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument };
                if (!episode.PluginMetadata.ContainsKey("Url"))
                    return new Response { ErrorMessage = "Invalid Episode", Status = ResponseStatus.InvalidArgument };
                DownloadInfo dp = new DownloadInfo { FileName = TemplateParser.FilenameFromEpisode(episode, quality, template), Format = formats, Percent = 0, Quality = quality };
                token.ThrowIfCancellationRequested();
                dp.Languages = new List<string>();
                dp.Percent = 1;
                dp.Status = "Getting Metadata";
                progress.Report(dp);
                List<string> todeleteFiles = new List<string>();

                WebStream ws = await WebStream.Get(episode.PluginMetadata["Url"], null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout, true, null, _info.ProxyFromGlobalRequirements(_global));
                if (ws != null && ws.StatusCode == HttpStatusCode.OK)
                {
                    if (!VerifyLogin(ws.Cookies))
                        SetLoginError(ret);
                    else
                    {

                        StreamReader rd = new StreamReader(ws);
                        string dta = rd.ReadToEnd();
                        rd.Dispose();
                        ws.Dispose();
                        Match bgn = bgnWrapper.Match(dta);
                        if (!bgn.Success)
                        {
                            ret.ErrorMessage = "Unable to find Daisuki public key";
                            ret.Status=ResponseStatus.WebError;
                            return ret;
                        }
                        Match flash = flashVars.Match(dta);
                        if (!flash.Success)
                        {
                            ret.ErrorMessage = "Seems this Episode is a YouTube video, unable to download";
                            ret.Status = ResponseStatus.WebError;
                            return ret;
                        }
                        MatchCollection col = flash2Vars.Matches(flash.Groups["vars"].Value);
                        Dictionary<string,string> vars=new Dictionary<string, string>();
                        foreach (Match m in col)
                        {
                            if (m.Success)
                            {
                                vars.Add(m.Groups["name"].Value, m.Groups["value"].Value);
                            }
                        }
                        if (!vars.ContainsKey("s") || !vars.ContainsKey("country") || !vars.ContainsKey("init"))
                        {
                            ret.ErrorMessage = "Some of Daisuki startup variables are missing";
                            ret.Status = ResponseStatus.WebError;
                            return ret;
                        }
                        token.ThrowIfCancellationRequested();
                        ws = await WebStream.Get(LibSet[BaseHostS]+bgn.Groups["wrapper"].Value, null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout, true,null, _info.ProxyFromGlobalRequirements(_global));
                        if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                        {
                            ret.ErrorMessage = "Unable to find Daisuki public key";
                            ret.Status = ResponseStatus.WebError;
                            ws?.Dispose();
                            return ret;
                        }
                        rd = new StreamReader(ws);
                        dta = rd.ReadToEnd();
                        rd.Dispose();
                        ws.Dispose();
                        Match mm = publicKey.Match(dta);
                        if (!mm.Success)
                        {
                            ret.ErrorMessage = "Unable to find Daisuki public key";
                            ret.Status = ResponseStatus.WebError;
                            return ret;
                        }
                        string bld = mm.Groups["key"].Value.Replace("\\n", "");

                        token.ThrowIfCancellationRequested();
                        dp.Percent = 2;
                        progress.Report(dp);

                        ws = await WebStream.Get(LibSet[BaseHostS] + vars["country"]+"?cashPath="+ (long)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds), null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout, true, episode.PluginMetadata["Url"], _info.ProxyFromGlobalRequirements(_global));
                        Country c;
                        if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                        {
                            ret.ErrorMessage = "Unable to find Daisuki Country Code";
                            ret.Status = ResponseStatus.WebError;
                            ws?.Dispose();
                            return ret;
                        }
                        try
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(Country));
                            c = (Country)ser.Deserialize(ws);
                            ws.Dispose();

                        }
                        catch (Exception)
                        {
                            ret.ErrorMessage = "Unable to find Daisuki Country Code";
                            ret.Status = ResponseStatus.WebError;
                            ws.Dispose();
                            return ret;
                        }
                        Dictionary<string, string> form = new Dictionary<string, string>();
                        Api api=new Api();
                        if (vars.ContainsKey("ss_id"))
                            api.SS_Id = vars["ss_id"];
                        if (vars.ContainsKey("mv_id"))
                            api.MV_Id = vars["mv_id"];
                        if (vars.ContainsKey("device_cd"))
                            api.Device_CD = vars["device_cd"];
                        if (vars.ContainsKey("ss1_prm"))
                            api.SS1_PRM = vars["ss1_prm"];
                        if (vars.ContainsKey("ss2_prm"))
                            api.SS2_PRM = vars["ss2_prm"];
                        if (vars.ContainsKey("ss3_prm"))
                            api.SS3_PRM = vars["ss3_prm"];
                        RSACryptoServiceProvider prov = ProviderFromPEM(bld);
                        AesManaged aes = new AesManaged();
                        aes.GenerateKey();
                        aes.Mode=CipherMode.CBC;
                        int blocksize = aes.BlockSize/8;
                        aes.IV=new byte[blocksize];
                        aes.KeySize=256;
                        aes.Padding=PaddingMode.Zeros;
                        aes.GenerateKey();
                        byte[] apidata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(api));
                        int nsize = ((apidata.Length + (blocksize - 1))/blocksize)*blocksize;
                        if (nsize!=apidata.Length)
                            Array.Resize(ref apidata,nsize);
                        ICryptoTransform t=aes.CreateEncryptor();
                        byte[] enc=t.TransformFinalBlock(apidata, 0, nsize);
                        byte[] key = prov.Encrypt(aes.Key,false);
                        form.Add("s", vars["s"]);
                        form.Add("c", c.CountryCode);
                        form.Add("e", episode.PluginMetadata["Url"]);
                        form.Add("d", Convert.ToBase64String(enc));
                        form.Add("a", Convert.ToBase64String(key));
                        token.ThrowIfCancellationRequested();
                        string n = form.PostFromDictionary();

                        ws = await WebStream.Get(LibSet[BaseHostS]+vars["init"]+"?"+n, null, LibSet[UserAgentS], null, s.cookies.ToCookieCollection(), SocketTimeout, true, episode.PluginMetadata["Url"], _info.ProxyFromGlobalRequirements(_global));
                        if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                        {
                            ret.ErrorMessage = "Unable to retrieve metadata";
                            ret.Status = ResponseStatus.WebError;
                            ws?.Dispose();
                            return ret;
                        }
                        rd = new StreamReader(ws);
                        dta = rd.ReadToEnd();
                        rd.Dispose();
                        ws.Dispose();
                        MetaEncrypt menc = JsonConvert.DeserializeObject<MetaEncrypt>(dta);
                        if (menc == null || menc.Status != "00")
                        {
                            ret.ErrorMessage = "Unable to retrieve metadata";
                            ret.Status = ResponseStatus.WebError;
                            return ret;
                        }
                        t = aes.CreateDecryptor();
                        byte[] indata = Convert.FromBase64String(menc.EncryptedData);
                        nsize = ((indata.Length + (blocksize - 1)) / blocksize) * blocksize;
                        if (nsize != indata.Length)
                            Array.Resize(ref indata, nsize);

                        byte[] outdata=t.TransformFinalBlock(indata, 0, indata.Length);
                        int start = outdata.Length;
                        while (outdata[start - 1] == 0)
                            start--;
                       if (start!=outdata.Length)
                            Array.Resize(ref outdata,start);
                        string final = Encoding.UTF8.GetString(outdata);
                        Data ldta = JsonConvert.DeserializeObject<Data>(final);
                        NameValueCollection headers = new NameValueCollection();
                        headers.Add("Accept", "*/*");
                        headers.Add("Accept-Language", "en-US");
                        headers.Add("x-flash-version", "18,0,0,232");
                        string guid = GenGUID(12);
                        string playurl = ldta.play_url + "&g=" + guid + "&hdcore=3.2.0";
                        token.ThrowIfCancellationRequested();
                        dp.Percent = 3;
                        dp.Status = "Gettings subtitles";
                        progress.Report(dp);
                        dp.Languages = new List<string>();
                        Dictionary<string, string> subtitles = new Dictionary<string, string>();

                        if (string.IsNullOrEmpty(ldta.caption_url))
                            dp.Languages.Add("Hardcoded");
                        else
                        {
                            ws = await WebStream.Get(ldta.caption_url + "?cashPath=" + (long)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds), null, LibSet[UserAgentS], headers, null, SocketTimeout, true, "http://img.daisuki.net/common2/pages/anime/swf/bngn_player_001.swf", _info.ProxyFromGlobalRequirements(_global));
                            if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                            {
                                ret.ErrorMessage = "Unable to retrieve subtitles";
                                ret.Status = ResponseStatus.WebError;
                                ws?.Dispose();
                                return ret;
                            }
                            TTML subs = new TTML(ws);
                            subtitles = subs.ToAss();
                            ws.Dispose();
                        }
                        dp.Percent = 4;
                        dp.Status = "Downloading video";
                        progress.Report(dp);
                        token.ThrowIfCancellationRequested();
                        ws = await WebStream.Get(playurl, null, LibSet[UserAgentS], headers, null, SocketTimeout, true, "http://img.daisuki.net/common2/pages/anime/swf/bngn_player_001.swf", _info.ProxyFromGlobalRequirements(_global));
                        int idx = playurl.LastIndexOf(".smil/", StringComparison.InvariantCulture);
                        string baseurl = string.Empty;
                        if (idx > 0)
                            baseurl = playurl.Substring(0, idx + 6);
                        if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                        {
                            ret.ErrorMessage = "Unable to retrieve metadata";
                            ret.Status = ResponseStatus.WebError;
                            ws?.Dispose();
                            return ret;
                        }
                        XmlSerializer serm = new XmlSerializer(typeof(Manifest));
                        //Stream ms = File.OpenRead(@"C:\users\mpiva\Downloads\s.manifest");
                        //Manifest manifest = (Manifest)serm.Deserialize(ms);
                        Manifest manifest = (Manifest)serm.Deserialize(ws);


                        rd.Dispose();
                        ws.Dispose();
                        manifest.Init();
                        KeyValuePair<Media, Quality>? kv = BestMediaFromManifest(manifest, quality);
                        if (kv == null)
                        {
                            ret.ErrorMessage = "Unable to find the best media";
                            ret.Status = ResponseStatus.WebError;
                            return ret;
                        }
                        dp.Quality = kv.Value.Value;
                        Media media = kv.Value.Key;
                        string inputs = string.Empty;
                        string maps = String.Empty;
                        int pp = 0;
                        foreach (string k in subtitles.Keys)
                        {
                            string pth = Path.GetTempFileName() + ".ass";
                            todeleteFiles.Add(pth);
                            File.WriteAllText(pth, subtitles[k]);
                            inputs += "-i \"" + pth + "\" ";
                            dp.Languages.Add(Languages.TranslateToOriginalLanguage(k));
                            maps += GetFFMPEGSubtitleArguments(pp + 1, pp, Languages.CodeFromLanguage(k), Languages.TranslateToOriginalLanguage(k));
                            pp++;
                        }
                        dp.Percent = 4;
                        dp.FileName = TemplateParser.FilenameFromEpisode(episode, dp.Quality, template);
                        dp.FullPath = Path.Combine(downloadpath, dp.FileName);
                        token.ThrowIfCancellationRequested();
                        progress.Report(dp);
                        string intermediatefile = dp.FullPath + ".tm1";

                        /* http://www.daisuki.net/etc/designs/daisuki/swf/bngn_player_002.swf*/
                        headers["X-Requested-With"] = "ShockwaveFlash/20.0.0.267";
                        FragmentProcessor frag =new FragmentProcessor(ws.Cookies,headers, LibSet[UserAgentS], SocketTimeout, episode.PluginMetadata["Url"], _info.ProxyFromGlobalRequirements(_global), 2,5,intermediatefile);
                        double dbl = 91;
                        IProgress<double> d = new Progress<double>((val) =>
                        {
                            dp.Percent = (val * dbl / 100) + 4;
                            progress.Report(dp);
                        });

                        todeleteFiles.Add(intermediatefile);
                        await frag.Start(baseurl, guid, manifest, media, token, d);
                        dp.Size = await ReMux(intermediatefile, inputs, maps, formats, deflangcode, deflang, 96, 4, dp, progress, token);
                        dp.Percent = 100;
                        dp.Status = "Finished";
                        progress.Report(dp);
                        foreach (string del in todeleteFiles)
                        {
                            try
                            {
                                File.Delete(del);
                            }
                            catch (Exception)
                            {

                            }
                        }
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
                if (e is OperationCanceledException)
                    return new Response { ErrorMessage = "Canceled", Status = ResponseStatus.Canceled };
                return new Shows { ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError };
            }
        }

        public async Task<Updates> Updates(ISession session)
        {
            try
            {

                Updates upds=new Updates();
                upds.Items=new List<Episode>();
                if (!UpdateHistory.IsLoaded)
                    UpdateHistory.Load();
                DaiSukiSession s = session as DaiSukiSession;
                if (s == null)
                    return new Updates { ErrorMessage = "Invalid Session", Status = ResponseStatus.InvalidArgument };
                Shows sws = await Shows(s, false);
                if (sws.Status != ResponseStatus.Ok)
                {
                    Updates k = new Updates();
                    sws.PropagateError(k);
                    return k;
                }
                int cnt = Convert.ToInt32(GetAuthSetting(DaiSukiPluginInfo.MaxFollowItems));
                List<Show> avs = sws.Items.Take(cnt).ToList();
                List<Task<Episodes>> ms = new List<Task<Episodes>>();
                foreach (Show sh in avs)
                {
                    ms.Add(Episodes(s, sh));
                }
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                if (ms.Count > 0)
                {
                    while (ms.Count > 0)
                    {
                        int max = 5;
                        if (ms.Count < 5)
                            max = ms.Count;
                        Task<Episodes>[] tsks = new Task<Episodes>[max];
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
                                if (tsks[x].Result.Items.Count > 0)
                                {
                                    Episode ep = tsks[x].Result.Items.OrderByDescending(a => a.Index).First();
                                    ep.UniqueTag = DaiSukiPluginInfo.PluginName + "|" + ep.ShowName + "|" +
                                                   ep.EpisodeAlpha;
                                    if (UpdateHistory.Exists(ep.UniqueTag))
                                    {
                                        Episode c =
                                            JsonConvert.DeserializeObject<Episode>(UpdateHistory.Get(ep.UniqueTag));
                                        upds.Items.Add(c);
                                    }
                                    else
                                    {
                                        ep.DateTime = datetime;
                                        UpdateHistory.Add(ep.UniqueTag, JsonConvert.SerializeObject(ep));
                                        upds.Items.Add(ep);
                                    }
                                }
                            }
                        }
                    }
                    UpdateHistory.Save();
                }
                return upds;
            }
            catch (Exception e)
            {
                return new Updates { ErrorMessage = e.ToString(), Status = ResponseStatus.SystemError };
            }
        }

        public void Exit()
        {
            DecryptForm.Kill();
        }


        private KeyValuePair<Media,Quality>? BestMediaFromManifest(Manifest m, Quality q)
        {
            List<Quality> qs = Enum.GetValues(typeof (Quality)).Cast<Quality>().ToList();
            int idx = qs.IndexOf(q);
            do
            {
                foreach (Media a in m.Medias)
                {
                    if ((int) a.MetadataInfo.height == q.ToHeight())
                    {
                        return new KeyValuePair<Media, Quality>(a,q);
                    }
                }
                idx--;
                if (idx>=0)
                    q = qs[idx];
            } while (idx==-1);
            return null;
        }
        private bool VerifyLogin(CookieCollection cookies)
        {
            Dictionary<string, string> cooks = cookies.ToDictionary();
            if (cooks.ContainsKey("key") && cooks.ContainsKey("userID"))
                return true;
            return false;
        }


        private string GenGUID(int length = 5)
        {
            StringBuilder bld=new StringBuilder();
            Random rnd=new Random();
            for (int x = 0; x < length; x++)
            {
                char c = (char)(65 + rnd.Next(25));
                bld.Append(c);
            }
            return bld.ToString();
        }

     
        public static RSACryptoServiceProvider ProviderFromPEM(string base64key)
        {
            byte[] key = Convert.FromBase64String(base64key);
            BinaryReader reader = new BinaryReader(new MemoryStream(key));
            try
            {
                var twobytes = reader.ReadUInt16();
                if (twobytes == 0x8130)
                    reader.ReadByte();
                else if (twobytes == 0x8230)
                    reader.ReadInt16();
                else
                    return null;
                reader.ReadBytes(15);
                twobytes = reader.ReadUInt16();
                if (twobytes == 0x8103)
                    reader.ReadByte();
                else if (twobytes == 0x8203)
                    reader.ReadInt16();
                else
                    return null;
                var bt = reader.ReadByte();
                if (bt != 0x00)
                    return null;
                twobytes = reader.ReadUInt16();
                if (twobytes == 0x8130)
                    reader.ReadByte();
                else if (twobytes == 0x8230)
                    reader.ReadInt16();
                else
                    return null;
                twobytes = reader.ReadUInt16();
                byte lowbyte;
                byte highbyte = 0x00;
                if (twobytes == 0x8102)
                    lowbyte = reader.ReadByte();
                else if (twobytes == 0x8202)
                {
                    highbyte = reader.ReadByte();
                    lowbyte = reader.ReadByte();
                }
                else
                    return null;
                byte[] modint = {lowbyte, highbyte, 0x00, 0x00};
                int modsize = BitConverter.ToInt32(modint, 0);
                byte firstbyte = reader.ReadByte();
                reader.BaseStream.Seek(-1, SeekOrigin.Current);
                if (firstbyte == 0x00)
                {
                    reader.ReadByte();
                    modsize -= 1;
                }
                byte[] modulus = reader.ReadBytes(modsize);
                if (reader.ReadByte() != 0x02)
                    return null;
                int expbytes = (int) reader.ReadByte();
                byte[] exponent = reader.ReadBytes(expbytes);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }

            finally
            {
                reader.Close();
            }
        }

    }
}
