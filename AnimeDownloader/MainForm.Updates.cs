using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;
using BrightIdeasSoftware;
using Timer = System.Threading.Timer;

namespace AnimeDownloader
{
    public partial class MainForm
    {
        public bool IsUpdated = false;
        public bool IsUpdateRefreshing = false;
        private int SecondCount=0;
        System.Threading.Timer secondtimer=null;

        public void InitUpdateList()
        {
            olvUpdateStatus.AspectGetter = (x) =>
            {
                Episode d = (Episode) x;
                if (Follows.Instance.IsFollow(d.ShowId, d.PluginName))
                    return "Following";
                return string.Empty;
            };
            olvUpdateDown.ImageGetter = (x) =>
            {
                return 1;
            };
            olvUpdateFollow.ImageGetter = (x) =>
            {
                return 0;
            };
        }

        public async Task RefreshUpdates()
        {
            if (!IsUpdateRefreshing)
            {
                IsUpdateRefreshing = true;
                await WrapGuiAction(() => labUpdateTime.Text = "Refreshing");
                List<Task<Updates>> tasks = new List<Task<Updates>>();
                foreach (string s in DownloadPluginHandler.Instance.AvailableAuthorizations)
                {
                    tasks.Add(DownloadPluginHandler.Instance.Updates(s));
                }
                await Task.WhenAll(tasks);
                List<Episode> updates=new List<Episode>();
                foreach (Task<Updates> up in tasks)
                {
                    if (up.Result.Status != ResponseStatus.Ok)
                        await WrapGuiAction(() => Log(LogType.Error, up.Result.ErrorMessage));
                    else
                        updates.AddRange(up.Result.Items);
                }
                List<EpisodeWithDownloadSettings> downloads = Follows.Instance.CheckFollows(updates);
                await WrapGuiAction(() =>
                {
                    foreach (EpisodeWithDownloadSettings d in downloads)
                    {
                        AddDownloadEpisode(d, d.Quality, d.Format,false);
                    }
                    Episode epu = null;
                    if (objUpdates.SelectedObject != null)
                        epu = (Episode)objUpdates.SelectedObject;

                    objUpdates.BeginUpdate();
                    objUpdates.SetObjects(updates);
                    objUpdates.BuildGroups(olvUpdateDate, SortOrder.Descending);
                    objUpdates.BuildList(true);
                    objUpdates.Sort(olvUpdateDate,SortOrder.Descending);

                    objUpdates.EndUpdate();
                    if (epu != null)
                    {
                        Episode n = updates.FirstOrDefault(a => a.UniqueTag == epu.UniqueTag);
                        if (n != null)
                            objUpdates.SelectedObject = n;
                    }
                });
                Episode last = updates.OrderByDescending(a => a.DateTime).FirstOrDefault();
                if (last!=null)
                    await WrapGuiAction(() => labUpdateLast.Text=last.ShowName+" "+last.EpisodeAlpha);
                IsUpdateRefreshing = false;
                IsUpdated = true;

            }
            if (secondtimer == null)
            {
                SecondCount = Settings.Instance.UpdateTime*60;
                secondtimer = new Timer(SecondTimer, null, 1000, 1000);

            }
        }

        private async void SecondTimer(object obj)
        {
            SecondCount--;
            if ((SecondCount < 60) && (SecondCount>0))
            {
                string text = SecondCount+" "+(SecondCount == 1 ? "second" : "seconds");
                await WrapGuiAction(() => labUpdateTime.Text = text);
            }
            else if (SecondCount>=60)
            {
                int val = (SecondCount+30)/60;
                string text = val + " " + (val == 1 ? "minute" : "minutes");
                await WrapGuiAction(() => labUpdateTime.Text = text);
            }
            else
            {
                secondtimer.Dispose();
                secondtimer = null;
                await RefreshUpdates();
            }
        }
        private void numMinutes_ValueChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.UpdateTime != numMinutes.Value)
            {
                if (numMinutes.Value > Settings.Instance.UpdateTime)
                {
                    int delta = ((int)numMinutes.Value - Settings.Instance.UpdateTime)*60;
                    SecondCount += delta;
                }
                else
                {
                    int delta = (Settings.Instance.UpdateTime - (int) numMinutes.Value)*60;
                    if (SecondCount - delta < 0)
                    {
                        SecondCount = SecondCount%60;
                    }
                    else
                    {
                        SecondCount -= delta;
                    }
                }
                SaveSettings();
                
            }
        }
        private void objUpdates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objUpdates.SelectedObject != null)
            {
                Episode epu = (Episode) objUpdates.SelectedObject;
                if (epu.ImageUri != null)
                    picUpdate.LoadAsync(epu.ImageUri.ToString());
                labUpdateDescription.Text = epu.Description ?? string.Empty;
            }
        }
        private async void butUpdateRefresh_Click(object sender, EventArgs e)
        {
            secondtimer.Dispose();
            secondtimer = null;
            Task.Factory.StartNew(async ()=>await RefreshUpdates());
        }

        private bool _follow_start=false;
        private async void objUpdates_CellClick(object sender, CellClickEventArgs e)
        {
            if ((e.ColumnIndex == olvUpdateDown.Index) && (objUpdates.SelectedIndex == e.RowIndex))
            {
                Episode ep=(Episode)objUpdates.SelectedObject;
                AddDownloadEpisode(ep);
            }
            else if ((e.ColumnIndex == olvUpdateFollow.Index) && (objUpdates.SelectedIndex == e.RowIndex) &&
                     (!_follow_start))
            {
                Episode ep = (Episode) objUpdates.SelectedObject;
                Show sel = null;
                foreach (Show s in objShows.Objects)
                {
                    if (s.Id == ep.ShowId)
                    {
                        sel = s;
                        break;
                    }
                }
                if (sel != null)
                {
                    if (Follows.Instance.IsFollow(sel.Id, sel.PluginName))
                    {
                        DeleteFollow(sel);
                    }
                    else
                    {
                        _follow_start = true;
                        Episodes result = await DownloadPluginHandler.Instance.Episodes(sel.PluginName, sel);
                        if (result.Status == ResponseStatus.Ok)
                        {
                            if (result.Items.Count > 0 &&
                                result.Items[0].ShowId == sel.Id)
                            {
                                FollowRequester(sel, result);
                            }
                        }
                        _follow_start = false;
                    }
                    RefreshStatus(ep.ShowId,ep.PluginName);
                }
            }
        }

        void RefreshStatus(string showid, string pluginname)
        {
            foreach (Episode ep in objUpdates.Objects)
            {
                if (ep.ShowId==showid || ep.PluginName==pluginname)
                    objUpdates.RefreshObject(ep);
            }
            foreach (Show s in objShows.Objects)
            {
                if (s.Id == showid || s.PluginName == pluginname)
                {
                    objShows.RefreshObject(s);
                }
            }
        }
    }
}
