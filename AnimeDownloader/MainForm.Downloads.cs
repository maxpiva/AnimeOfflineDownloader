using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;
using BrightIdeasSoftware;

namespace AnimeDownloader
{
    public partial class MainForm
    {
        private DownloadManager _manager;





        public void InitDownloadManager()
        {
            _manager = new DownloadManager(Settings.Instance.SimultaneousDownloads);
            _manager.OnProgress += (a) =>
            {
                objDownloads.RefreshObject(a);
                if (a==objDownloads.SelectedObject)
                    RefreshInfo();
            };
            olvFile.AspectGetter = (x) =>
            {
                DownloadItem d = (DownloadItem) x;
                return d.DownloadInfo.FileName;
            };
            olvStatus.AspectGetter = (x) =>
            {
                DownloadItem d = (DownloadItem)x;
                return d.Status;
            };
            olvMessage.AspectGetter = (x) =>
            {
                DownloadItem d = (DownloadItem) x;
                if (!string.IsNullOrEmpty(d.DownloadError))
                {
                    return d.DownloadError;
                }
                return d.DownloadInfo.Status;
            };
            objDownloads.FormatCell += (a, b) =>
            {
                if (b.ColumnIndex == olvMessage.Index)
                {
                    DownloadItem d = (DownloadItem) b.Model;
                    b.Item.ForeColor = !string.IsNullOrEmpty(d.DownloadError) ? Color.Red : SystemColors.ControlText;
                }
            };
            olvProgress.Renderer = new BarRenderer(0, 1000);
            olvProgress.AspectGetter = (x) =>
            {
                DownloadItem d = (DownloadItem) x;
                return (int)(d.DownloadInfo.Percent*10);
            };
           objDownloads.DragSource = new SimpleDragSource();
           objDownloads.DropSink = new DropSink(false,_manager);


            
        }

        public void LoadDownloadItems()
        {
            List<DownloadItem> downs = _manager.Load(Settings.Instance.DownloadTemplate, Settings.Instance.DownloadPath);
            if (downs != null && downs.Count > 0)
            {
                objDownloads.SetObjects(downs);
                if (downs.Count > 0)
                    objDownloads.SelectedObject = downs[0];
            }
        }
        public class DropSink : RearrangingDropSink
        {
            private DownloadManager _manager;
            public DropSink(bool t, DownloadManager mng) : base(t)
            {
                _manager = mng;
            }
            
            public override void Drop(DragEventArgs args)
            {
                base.Drop(args);
                DownloadItem dinfo = (DownloadItem)DropTargetItem.RowObject;
                if (dinfo != null)
                {
                    int idx = this.ListView.IndexOf(dinfo);
                    if (idx == this.ListView.Items.Count - 1)
                    {
                        _manager.ReOrder(dinfo.Id, null);
                    }
                    else
                    {
                        DownloadItem d2 = (DownloadItem) this.ListView.GetModelObject(idx + 1);
                        _manager.ReOrder(dinfo.Id,d2.Id);
                    }
                }
            
            }
        }
        public void RefreshInfo()
        {
            if (objDownloads.SelectedIndex == -1)
            {
                labStatus.Text = string.Empty;
                labWorkStatus.Text = string.Empty;
                labSize.Text = string.Empty;
                labFile.Text = string.Empty;
                labFormat.Text = string.Empty;
                labQuality.Text = string.Empty;
                labProgress.Text = string.Empty;
                labSubtitles.Text = string.Empty;
                labPlugin.Text = string.Empty;
                butChange.Visible = false;
                butDownload.Visible = false;
                butRemoveSelected.Visible = false;
            }
            else
            {
                DownloadItem d = (DownloadItem)objDownloads.SelectedObject;
                labFile.Text = d.DownloadInfo.FileName;
                labProgress.Text = d.DownloadInfo.Percent.ToString("N2") + " %";
                labStatus.Text = d.Status.ToString();
                if (!string.IsNullOrEmpty(d.DownloadError))
                {
                    labWorkStatus.Text=d.DownloadError;
                    labWorkStatus.ForeColor = Color.Red;
                }
                else
                {
                    labWorkStatus.Text = d.DownloadInfo.Status;
                    labWorkStatus.ForeColor = SystemColors.ControlText;
                }
                if (d.DownloadInfo.Languages != null && d.DownloadInfo.Languages.Count > 0)
                {
                    labSubtitles.Text = string.Join(", ", d.DownloadInfo.Languages);
                }
                else
                {
                    labSubtitles.Text = "Not yet know";
                }
                labFormat.Text = d.DownloadInfo.Format.ToText();
                labQuality.Text = d.DownloadInfo.Quality.ToText();
                labPlugin.Text = d.Episode.PluginName;
                labSize.Text = string.IsNullOrEmpty(d.DownloadInfo.Size) ? "Not yet know" : d.DownloadInfo.Size;
                if ((d.Status == DownloadStatus.Cancelled) || (d.Status == DownloadStatus.Error) ||
                    (d.Status == DownloadStatus.Queue))
                {
                    butChange.Visible = true;
                }
                else
                {
                    butChange.Visible = false;
                }
                if ((d.Status == DownloadStatus.Cancelled) || (d.Status == DownloadStatus.Error))

                {
                    butDownload.Text = "Download Selected";
                    butDownload.Visible = true;                    
                    butDownload.Tag = true;
                }
                else if ((d.Status == DownloadStatus.Queue) || (d.Status == DownloadStatus.Downloading))
                {
                    butDownload.Text = "Cancel Selected";
                    butDownload.Visible = true;
                    butDownload.Tag = false;
                }
                else
                {
                    butDownload.Visible = false;
                }
                butRemoveSelected.Visible = true;
            }
            bool remd = false;
            bool remc = false;
            bool rema = objDownloads.Items.Count>0;
            foreach (DownloadItem d in objDownloads.Objects)
            {
                if (d.Status == DownloadStatus.Cancelled)
                    remc = true;
                if (d.Status == DownloadStatus.Complete)
                    remd = true;
            }
            butRemoveCanceled.Visible = remc;
            butRemoveDownloaded.Visible = remd;
            butRemoveAll.Visible = rema;
        }
        public void AddDownloadEpisode(Episode ep,Quality quality, ADBaseLibrary.Format format, bool logerror=true)
        {
            if (IsInDownloadList(ep.Id, quality))
            {
                if (logerror)
                {
                    Log(LogType.Info,
                    TemplateParser.FilenameFromEpisode(ep, quality, Settings.Instance.DownloadTemplate) +
                    " was already added");
                }

            }
            else
            {
                DownloadItem dinfo = _manager.Add(EpisodeWithDownloadSettings.FromEpisode(ep,quality,format), Settings.Instance.DownloadTemplate, Settings.Instance.DownloadPath);
                Log(LogType.Info, "Adding " + dinfo.DownloadInfo.FileName + " to downloads");
                objDownloads.AddObject(dinfo);
                if (objDownloads.Items.Count == 1)
                    objDownloads.SelectedObject = dinfo;
                RefreshInfo();
            }
        }

        public void AddDownloadEpisode(Episode ep)
        {
            AddDownloadEpisode(ep,Settings.Instance.DefaultQuality,Settings.Instance.DefaultFormat);
        }


        private void RemoveFlagged(DownloadStatus status)
        {
            List<DownloadItem> dels = new List<DownloadItem>();
            foreach (DownloadItem s in objDownloads.Objects)
            {
                if (s.Status == status)
                {
                    dels.Add(s);
                }
            }
            if (dels.Count > 0)
            {
                objDownloads.RemoveObjects(dels);
                foreach (DownloadItem d in dels)
                    _manager.Delete(d.Id);
            }
        }

        private void butRemoveCanceled_Click(object sender, EventArgs e)
        {
            RemoveFlagged(DownloadStatus.Cancelled);
        }

        private void butRemoveDownloaded_Click(object sender, EventArgs e)
        {
            RemoveFlagged(DownloadStatus.Downloading);

        }

        private void butRemoveAll_Click(object sender, EventArgs e)
        {
            List<DownloadItem> dels = new List<DownloadItem>();
            foreach (DownloadItem s in objDownloads.Objects)
            {
                dels.Add(s);
            }
            if (dels.Count > 0)
            {
                objDownloads.RemoveObjects(dels);
                foreach (DownloadItem d in dels)
                    _manager.Delete(d.Id);
            }
        }

        private void butRemoveSelected_Click(object sender, EventArgs e)
        {
            if (objDownloads.SelectedIndex != -1)
            {
                DownloadItem d = (DownloadItem)objDownloads.SelectedObject;
                objDownloads.RemoveObject(d);
                _manager.Delete(d.Id);
            }
        }

        private void butDownload_Click(object sender, EventArgs e)
        {
            if (objDownloads.SelectedIndex != -1)
            {
                DownloadItem d = (DownloadItem)objDownloads.SelectedObject;
                if ((bool)butDownload.Tag)
                    _manager.Reset(d.Id);
                else
                    _manager.Cancel(d.Id);
            }
        }

        private void butChange_Click(object sender, EventArgs e)
        {
            if (objDownloads.SelectedIndex != -1)
            {
                DownloadItem d = (DownloadItem)objDownloads.SelectedObject;
                Format f = new Format(d.DownloadInfo.FileName, false);
                f.Filename = d.DownloadInfo.FileName;
                f.FileFormats = d.DownloadInfo.Format;
                f.FileQuality = d.DownloadInfo.Quality;
                DialogResult r = f.ShowDialog();
                if (r == DialogResult.OK)
                {
                    d.DownloadInfo.Format = f.FileFormats;
                    d.DownloadInfo.Quality = f.FileQuality;
                    d.DownloadInfo.FileName=TemplateParser.FilenameFromEpisode(d.Episode, f.FileQuality, Settings.Instance.DownloadTemplate);
                    d.DownloadInfo.FullPath=Path.Combine(Settings.Instance.DownloadPath, d.DownloadInfo.FileName);
                    RefreshInfo();
                }
            }

        }
        private void objDownloads_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        public bool IsInDownloadList(string id,Quality q)
        {
            if (objDownloads.Objects != null)
            {
                foreach (DownloadItem d in objDownloads.Objects)
                {
                    if ((d.Episode.Id == id) && (d.DownloadInfo.Quality==q))
                        return true;
                }
            }
            return false;
        }

    }
}
