using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ADBaseLibrary;

namespace DaiSukiPlugin
{
    public class DaiSukiPluginInfo : DownloadPluginInfo
    {
        internal const string PluginName = "DaiSuki";
        public override string Name
        {
            get { return PluginName; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string RegisterUrl
        {
            get { return "http://www.daisuki.net/"; }
        }

        public const string FFMPEGPath="FFMPEG Path";
        public const string RTMPDumpPath = "RTMPDump Path";
        public const string FFMPEGDownloadLink = "Download FFMPEG";
        public const string RTMPDDownloadLink = "Download RTMPDump";
        public const string MaxFollowItems = "Max Follow Items";

        public override List<Requirement> GlobalRequirements
        {
            get
            {
                List<Requirement> ls = base.GlobalRequirements.ToList();
                ls.Add(new Requirement { Name = FFMPEGPath, RequirementType = RequirementType.Required | RequirementType.FilePath });
                ls.Add(new Requirement { Name = FFMPEGDownloadLink, RequirementType = RequirementType.Link, Options = new List<string> { "http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-20150903-git-d1bdaf3-win64-static.7z" } });
                ls.Add(new Requirement { Name = RTMPDumpPath, RequirementType = RequirementType.Required | RequirementType.FilePath });
                ls.Add(new Requirement { Name = RTMPDDownloadLink, RequirementType = RequirementType.Link, Options = new List<string> { "https://github.com/K-S-V/Scripts/releases/download/v2.4/rtmpdump-2.4.zip" } });
                return ls;
            }

        }


        public override List<Requirement> AuthenticationRequirements
        {
            get
            {
                List<Requirement> ls = base.AuthenticationRequirements.ToList();
                ls.Add(new Requirement { Name = MaxFollowItems,RequirementType = RequirementType.Integer|RequirementType.Required});
                return ls;
            }
        }

        public override Dictionary<string, object> DefaultAuthenticationData
        {

            get
            {
                Dictionary<string, object> m = base.DefaultAuthenticationData.ToDictionary(a => a.Key, a => a.Value);
                m.Add(MaxFollowItems, 20);
                return m;
            }
        }

        public override Dictionary<string, object> DefaultGlobalData
        {
            
            get
            {
                Dictionary<string, object> m = base.DefaultGlobalData.ToDictionary(a => a.Key, a => a.Value);
                string basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string ffmpegpath = Path.Combine(basepath, "FFMPEG.exe");
                string rtmpdump = Path.Combine(basepath, "RTMPDump.exe");
                if (!File.Exists(ffmpegpath))
                    ffmpegpath = string.Empty;
                if (!File.Exists(rtmpdump))
                    rtmpdump = string.Empty;

                m.Add(FFMPEGPath, ffmpegpath);
                m.Add(RTMPDumpPath, rtmpdump);
                return m;
            }
        }


    }
}
