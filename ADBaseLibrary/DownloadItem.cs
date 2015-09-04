using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADBaseLibrary
{

    public class DownloadItem 
    {
        public DownloadInfo DownloadInfo { get; private set; }
        public DownloadStatus Status { get; set; } = DownloadStatus.Queue;        
        public string DownloadError { get; set; }
        public EpisodeWithDownloadSettings Episode { get { return _ep; }}
        public string Id { get; set; }
        public int Index { get; set; }
        private DownloadManager _manager;
        private CancellationTokenSource _token;
        private EpisodeWithDownloadSettings _ep;
        private TaskScheduler _scheduler;
        private string _template;
        private string _path;
        public delegate void FinishHandler(string id);
        public event DownloadManager.ProgressHandler OnProgress;
        public event FinishHandler OnFinish;

        public DownloadItem(EpisodeWithDownloadSettings ep, string template, string path, DownloadManager manager, DownloadStatus status)
        {
            _ep = ep;
            Status = status;
            DownloadInfo=new DownloadInfo();
            DownloadInfo.FileName = TemplateParser.FilenameFromEpisode(ep, ep.Quality, template);
            DownloadInfo.FullPath = Path.Combine(path, DownloadInfo.FileName);
            DownloadInfo.Quality = ep.Quality;
            DownloadInfo.Format = ep.Format;
            DownloadInfo.Languages=new List<string>();
            DownloadInfo.Status = string.Empty;
            DownloadInfo.Percent = 0;
            _template = template;
            _path = path;
            Id = Guid.NewGuid().ToString();
            _manager = manager;
            _scheduler= TaskScheduler.FromCurrentSynchronizationContext();
        }

        public void Cancel()
        {
            if ((Status == DownloadStatus.Downloading) && (_token!=null))
            {
                _token.Cancel();
            }
            Status = DownloadStatus.Cancelled;
        }



        public void DoProgress()
        {
            if (OnProgress != null)
            {
                Task.Factory.StartNew(() =>
                {
                    OnProgress(this);
                }, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            }
        }
        public void DoProgress(DownloadInfo d)
        {
            this.DownloadInfo = d;
            if (OnProgress != null)
            {
                Task.Factory.StartNew(() =>
                {
                    OnProgress(this);
                }, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            }
        }
        public void DoFinish()
        {
            if (OnFinish != null)
            {
                Task.Factory.StartNew(() =>
                {
                    OnFinish(this.Id);
                }, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            }
        }
        public void Start()
        {
            if (Status != DownloadStatus.Downloading)
            {
                DownloadError = string.Empty;
                _token = new CancellationTokenSource();
                IProgress<DownloadInfo> ip = new Progress<DownloadInfo>(DoProgress);
                Status=DownloadStatus.Downloading;
                DoProgress();
                Task.Run(async () =>
                {
                    try
                    {
                        Response r = await DownloadPluginHandler.Instance.Download(_ep.PluginName, _ep, _template, _path, DownloadInfo.Quality, DownloadInfo.Format, _token.Token, ip);
                        if (r.Status != ResponseStatus.Ok)
                        {
                            DownloadError = r.ErrorMessage;
                            Status = DownloadStatus.Error;
                        }
                        else
                        { 
                            Status = DownloadStatus.Complete;
                        }
                    }
                    catch (TaskCanceledException ce)
                    {
                        Status=DownloadStatus.Cancelled;
                    }
                    DoProgress();
                    DoFinish();
                });
                
            }
        }

    }
}
