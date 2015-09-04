using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADBaseLibrary.Helpers;
using Newtonsoft.Json;

namespace ADBaseLibrary
{
    public class Follows 
    {
        [JsonIgnore]
        public static Follows Instance { get; } = new Follows();

        private const string follows = "follows.json";

        public Follows()
        {
            Load();
        }
        public List<Follow> ShowFollows=new List<Follow>(); 
        public List<EpisodeWithDownloadSettings> Downloads=new List<EpisodeWithDownloadSettings>();

        public bool IsFollow(string showId, string pluginName)
        {
            if (ShowFollows.Any(a => a.ShowId == showId && a.PluginName == pluginName))
                return true;
            return false;
        }
        public bool IsFollow(string showId, string pluginName, Quality quality, Format format)
        {
            foreach (Follow f in ShowFollows.Where(a => a.ShowId == showId && a.PluginName == pluginName && a.Quality == quality))
            {
                if ((f.Format & format) != 0)
                    return true;

            }
            return false;
        }

        public void Load()
        {
            string cpath = Path.Combine(UserDataPath.Get(), follows);
            if (File.Exists(cpath))
            {
                string str = File.ReadAllText(cpath);
                JsonConvert.PopulateObject(str,this);
            }
        }

        public void Save()
        {
            string cpath = Path.Combine(UserDataPath.Get(), follows);
            File.WriteAllText(cpath, JsonConvert.SerializeObject(this));
        }
        

        public void AddFollow(string showId, string pluginName, Quality quality, Format format)
        {
            ShowFollows.Add(new Follow {ShowId = showId, PluginName = pluginName, Quality = quality, Format = format});
            Save();
        }

        public void RemoveFollow(string showId, string pluginName)
        {
            foreach (Follow f in ShowFollows.Where(a => a.ShowId == showId && a.PluginName == pluginName).ToList())
            {
                ShowFollows.Remove(f);
            }
            Save();
        }

        public void AddDownload(EpisodeWithDownloadSettings d)
        {
            Downloads.Add(d);
            Save();
        }
        public List<EpisodeWithDownloadSettings> CheckFollows(List<Episode> upds)
        {
            List <EpisodeWithDownloadSettings> downs=new List<EpisodeWithDownloadSettings>();
            foreach (Episode upd in upds)
            {
                List<Follow> fls = ShowFollows.Where(a => a.ShowId == upd.ShowId && a.PluginName == upd.PluginName).ToList();
                foreach(Follow f in fls.ToList())
                {
                    if (!Downloads.Any(a => a.Id == upd.Id && a.PluginName == upd.PluginName && a.Quality == f.Quality && a.Format == f.Format))
                    {
                        EpisodeWithDownloadSettings his=new EpisodeWithDownloadSettings();
                        upd.CopyTo(his);
                        his.Format = f.Format;
                        his.Quality = f.Quality;
                        downs.Add(his);
                    }
                }
            }
            return downs;
        } 

    }




}
