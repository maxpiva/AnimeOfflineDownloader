using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ADBaseLibrary.Helpers;
using Newtonsoft.Json;

namespace ADBaseLibrary
{
    public class DownloadManager
    {

        private const string downloads = "downloads.json";

        public delegate void ProgressHandler(DownloadItem p);


        public event ProgressHandler OnProgress;
     
        private int cnt;

        internal List<DownloadItem> _downloads = new List<DownloadItem>();
        public object _downloadslock = new object();

        private int _maxdownloads;

        public int MaxDownloads
        {
            get { return _maxdownloads; }
            set
            {
                _maxdownloads = value;
                CheckForDownload();
            }
        }

        public DownloadManager(int maxdownloads)
        {
            MaxDownloads = maxdownloads;
        }

        public List<DownloadItem> Load(string template, string downloadpath)
        {
            List<DownloadItem> downs=new List<DownloadItem>();
            string cpath = Path.Combine(UserDataPath.Get(), downloads);
            if (File.Exists(cpath))
            {
                string str = File.ReadAllText(cpath);
                List<EpisodeWithDownloadSettingAndStatus> dm=JsonConvert.DeserializeObject<List<EpisodeWithDownloadSettingAndStatus>>(str);
                foreach (EpisodeWithDownloadSettingAndStatus ep in dm)
                {
                    if (ep.DownloadStatus==DownloadStatus.Queue || ep.DownloadStatus==DownloadStatus.Downloading)
                        ep.DownloadStatus=DownloadStatus.Queue;
                    downs.Add(Add(ep,template,downloadpath,ep.DownloadStatus));
                }
            }
            return downs;
        }
        public DownloadItem Add(EpisodeWithDownloadSettings ep, string template, string downloadpath,DownloadStatus status=DownloadStatus.Queue)
        {
            DownloadItem dinfo = null;
            lock (_downloadslock)
            {
                dinfo = new DownloadItem(ep, template, downloadpath, this, status);
                dinfo.Index = cnt++;
                dinfo.OnProgress += ((a) =>
                {
                    if (OnProgress != null)
                        OnProgress(a);
                });

                dinfo.OnFinish += ((a) =>
                {
                    Save();
                    CheckForDownload();
                });
                lock (_downloadslock)
                {
                    _downloads.Add(dinfo);
                }
            }
            CheckForDownload();
            return dinfo;
        }

        public void Delete(string id)
        {
            DownloadItem dinfo = _downloads.FirstOrDefault(a => a.Id == id);
            lock (_downloadslock)
            {
                if (dinfo != null && (dinfo.Status == DownloadStatus.Downloading))
                    dinfo.Cancel();
            }
            Save();
        }

        public void Save()
        {
            lock (_downloadslock)
            {
                List<EpisodeWithDownloadSettings> sv=new List<EpisodeWithDownloadSettings>();
                foreach (DownloadItem d in _downloads)
                {
                    EpisodeWithDownloadSettingAndStatus dk =new EpisodeWithDownloadSettingAndStatus();
                    d.Episode.CopyTo(dk);
                    dk.DownloadStatus = d.Status;
                    sv.Add(dk);
                }
                string cpath = Path.Combine(UserDataPath.Get(), downloads);
                File.WriteAllText(cpath, JsonConvert.SerializeObject(sv));
            }
        }

        public void Cancel(string id)
        {
            DownloadItem dinfo = _downloads.FirstOrDefault(a => a.Id == id);
            if (dinfo != null && ((dinfo.Status == DownloadStatus.Downloading) || (dinfo.Status==DownloadStatus.Queue)))
                dinfo.Cancel();
            Save();
        }

        public void PrepateToQuit()
        {
            lock (_downloadslock)
            {
                foreach (DownloadItem d in _downloads)
                {
                    Cancel(d.Id);
                }
            }
            Save();
        }
        public void Reset(string id)
        {
            DownloadItem dinfo = _downloads.FirstOrDefault(a => a.Id == id);
            if (dinfo != null && (dinfo.Status != DownloadStatus.Complete) &&
                (dinfo.Status != DownloadStatus.Downloading) && (dinfo.Status != DownloadStatus.Queue))
            {
                dinfo.Status = DownloadStatus.Queue;
                CheckForDownload();
            }
            
        }

        public void ReOrder(string id, string downid)
        {
            lock (_downloadslock)
            {
                DownloadItem dinfo = _downloads.FirstOrDefault(a => a.Id == id);
                DownloadItem rel = null;
                if (dinfo != null)
                {
                    _downloads.Remove(dinfo);
                    if (downid != null)
                        rel = _downloads.FirstOrDefault(a => a.Id == downid);
                    if (rel == null)
                        _downloads.Add(dinfo);
                    if (rel != null)
                        _downloads.Insert(_downloads.IndexOf(rel), dinfo);
                }
            }
            Save();
        }

        private void CheckForDownload()
        {
            bool change = false;
            lock (_downloadslock)
            {
                int cnt = _downloads.Count(a => a.Status == DownloadStatus.Downloading);
                List<DownloadItem> infos =_downloads.Where(a => a.Status == DownloadStatus.Queue).OrderBy(a => a.Index).ToList();
                if (cnt < MaxDownloads && (infos.Count > 0))
                {
                    int max = MaxDownloads - cnt;
                    if (max > infos.Count)
                        max = infos.Count;
                    for (int x = 0; x < max; x++)
                    {
                        infos[x].Start();
                        change = true;
                    }
                }
            }
            if (change)
               Save();

        }
    }
}
