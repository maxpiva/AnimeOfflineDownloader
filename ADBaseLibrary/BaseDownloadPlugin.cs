using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ADBaseLibrary.Helpers;

namespace ADBaseLibrary
{
    public class BaseDownloadPlugin
    {
        public static string UserAgent = Properties.Settings.Default.UserAgent ?? "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.132 Safari/537.36";
        public static string FFMPEGArgs = Properties.Settings.Default.FFMPEGArgs ?? "-i \"{0}\" {1} -vcodec copy -acodec copy -scodec copy {4}-map 0:0 -map 0:1 -metadata:s:a:0 language={5} -metadata:s:a:0 title=\"{6}\" {2} {7} -y \"{3}\"";
        public static string FFMPEGSubtitleArgs = Properties.Settings.Default.FFMPEGSubtitleArgs ?? "-map {0}:0 -metadata:s:s:{1} language={2} -metadata:s:s:{1} title=\"{3}\" ";
        public static string FFMPEGEXE = Properties.Settings.Default.FFMPEGEXE ?? "ffmpeg.exe";
        public static int SocketTimeout = Properties.Settings.Default.SocketTimeout==0 ? 50000 : Properties.Settings.Default.SocketTimeout;

        public string GetFFMPEGSubtitleArguments(int subtitleFileCount, int subtitileCount, string languageCode, string language)
        {
            return string.Format(FFMPEGSubtitleArgs, subtitleFileCount, subtitileCount, languageCode, language);
        }
        public async Task<string> ReMux(string inputfile, string inputs, string subtileargs, Format formats,
            string audiolanguagecode, string audiolanguage, double initialPercent, double percentIncrement,
            DownloadInfo dinfo, IProgress<DownloadInfo> progress, CancellationToken token)
        {
            int formatcnt = 0;
            foreach (Format fol in Enum.GetValues(typeof(Format)))
            {
                if ((fol & formats) == fol)
                {
                    formatcnt++;
                }
            }

            percentIncrement /=(2*formatcnt);
            string size = string.Empty;
            string intermediatefile = dinfo.FullPath + ".tm2";
            foreach (Format fol in Enum.GetValues(typeof(Format)))
            {
                if ((fol & formats) == fol)
                {
                    string ffmpegargs = string.Format(FFMPEGArgs, inputfile, inputs, subtileargs, intermediatefile, fol == Format.Mkv ? string.Empty : "-c:s mov_text ", audiolanguagecode, audiolanguage, fol == Format.Mkv ? "-f matroska" : "-f mp4");
                    token.ThrowIfCancellationRequested();
                    dinfo.Percent = initialPercent;
                    initialPercent += percentIncrement;
                    dinfo.Status = "Muxing Video";
                    progress.Report(dinfo);
                    ShellParser ffm = new ShellParser();
                    await ffm.Start(FFMPEGEXE, ffmpegargs, token);
                    dinfo.Percent = initialPercent;
                    initialPercent += percentIncrement;
                    dinfo.Status = "Unique Hashing";
                    progress.Report(dinfo);

                    if (fol == Format.Mkv)
                        Matroska.Matroska.MatroskaHash(intermediatefile, dinfo.FullPath + "." + fol.ToExtension());
                    else
                        Mp4.Mp4.Mp4Hash(intermediatefile, dinfo.FullPath + "." + fol.ToExtension());
                    try
                    {
                        File.Delete(intermediatefile);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    FileInfo f = new FileInfo(dinfo.FullPath + "." + fol.ToExtension());
                    if (formatcnt == 1)
                    {
                        size = f.Length.ToString();
                    }
                    else
                    {
                        size += f.Length.ToString() + " (" + fol.ToExtension() + "), ";
                    }
                }
            }
            try
            {
                File.Delete(inputfile);
            }
            catch (Exception)
            {
                // ignored
            }
            if (formatcnt > 1)
                size = size.Substring(0, size.Length - 2);
            return size;
        }

        public void SetLoginError(Response resp)
        {
            resp.Status = ResponseStatus.LoginRequired;
            resp.ErrorMessage = "Login is required";
        }
        public void SetWebError(Response resp)
        {
            resp.Status = ResponseStatus.WebError;
            resp.ErrorMessage = "Unable to connect to server";
        }
        public object GetAuthSetting(string name)
        {
            IDownloadPlugin d = (IDownloadPlugin) this;
            Dictionary<string, object> meta = Settings.Instance.AuthorizationsMetadataDictionary[d.Information().Name];
            if (meta.ContainsKey(name))
                return meta[name];
            return null;
        }
    }
}
