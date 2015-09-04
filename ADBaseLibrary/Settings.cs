using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace ADBaseLibrary
{

    public class Settings
    {
        [JsonIgnore]
        public static Settings Instance { get; } = new Settings();
        [JsonProperty(Required = Required.Always), JsonConverter(typeof(SessionDictionarySerializer))]
        internal Dictionary<string, ISession> OpenSessionsDictionary = new Dictionary<string, ISession>();
        [JsonProperty(Required = Required.Always)]
        internal Dictionary<string, object> GlobalMetadataDictionary = new Dictionary<string, object>();
        [JsonProperty(Required = Required.Always)]
        internal Dictionary<string, Dictionary<string, object>> AuthorizationsMetadataDictionary = new Dictionary<string, Dictionary<string, object>>();
        public string DownloadPath { get; set; } = GetDefaultDownloadPath();
        public string DownloadTemplate { get; set; } = "{seasonalphaorshow} {episodenumeric:000} [{resolution}]";
        public int NumberOfRetries { get; set; } = 3;
        public int SimultaneousDownloads { get; set; } = 5;
        public Format DefaultFormat { get; set; } = Format.Mkv;
        public Quality DefaultQuality { get; set; } = Quality.Quality1080P;
        public int UpdateTime { get; set; } = 3;
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Deserialize(string str)
        {
            JsonConvert.PopulateObject(str, this);
        }

        /*
        public string AuthorizationsMetadata
        {
            get { return JsonConvert.SerializeObject(AuthorizationsMetadataDictionary); }
            set
            {
                AuthorizationsMetadataDictionary = !string.IsNullOrEmpty(value)
                    ? JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(value)
                    : new Dictionary<string, Dictionary<string, object>>();
            }
        }

        public string GlobalMetadata
        {
            get { return JsonConvert.SerializeObject(GlobalMetadataDictionary); }
            set
            {
                GlobalMetadataDictionary = !string.IsNullOrEmpty(value)
                    ? JsonConvert.DeserializeObject<Dictionary<string, object>>(value)
                    : new Dictionary<string, object>();
            }
        }

        public string OpenSessions
        {
            get
            {
                Dictionary<string, Dictionary<string, string>> serializedsessions =
                    new Dictionary<string, Dictionary<string, string>>();
                foreach (string k in OpenSessionsDictionary.Keys)
                    serializedsessions.Add(k, OpenSessionsDictionary[k].Serialize());
                return JsonConvert.SerializeObject(serializedsessions);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Dictionary<string, Dictionary<string, string>> data =
                        JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(value);
                    foreach (string k in data.Keys)
                    {
                        if (PluginHandler.Instance.Plugins.ContainsKey(k))
                            OpenSessionsDictionary.Add(k, PluginHandler.Instance.Plugins[k].Deserialize(data[k]));
                    }
                }
                else
                    OpenSessionsDictionary = new Dictionary<string, ISession>();
            }

        }
        */
        private static string GetDefaultDownloadPath()
        {
            IntPtr outPath;
            int result = SHGetKnownFolderPath(new Guid("{374DE290-123F-4565-9164-39C4925E467B}"), 0x00004000,
                IntPtr.Zero, out outPath);
            if (result >= 0)
                return Marshal.PtrToStringUni(outPath);
            return string.Empty;
        }

        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
            IntPtr hToken, out IntPtr ppszPath);

    }
}
