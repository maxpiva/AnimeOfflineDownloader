using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;
using BrightIdeasSoftware;

namespace AnimeDownloader
{
    public partial class MainForm
    {
        private string search_text = null;
        private Episodes _selected_episodes;

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            search_text = txtSearch.Text.ToUpper();
            objShows.UseFiltering = !string.IsNullOrEmpty(search_text);
            objShows.BuildList(true);
        }
        private async void butRefresh_Click(object sender, EventArgs e)
        {
            await PopulateSeries();
        }

        public void InitShowLists()
        {
            colEpisodeSeason.FreeSpaceProportion = 1;
            colEpisodeName.FreeSpaceProportion = 3;
            objShows.ModelFilter = new ModelFilter(a =>
            {
                if (string.IsNullOrEmpty(search_text))
                    return true;
                if (((Show)a).Name.ToUpper().Contains(search_text))
                    return true;
                return false;
            });
            olvShowStatus.AspectGetter = (x) =>
            {
                Show d = (Show)x;
                if (Follows.Instance.IsFollow(d.Id, d.PluginName))
                    return "Following";
                return string.Empty;
            };
            olvDownloadBut.ImageGetter = (x) =>
            {
                return 1;
            };
            olvDownAll.ImageGetter = (x) =>
            {
                return 1;
            };
            olvFollow.ImageGetter = (x) =>
            {
                return 0;
            };
        }
        private async Task PopulateSeries()
        {
            List<Show> shows = new List<Show>();
            List<Task<Shows>> tasks = new List<Task<Shows>>();
            foreach (string s in DownloadPluginHandler.Instance.AvailableAuthorizations)
            {
                tasks.Add(DownloadPluginHandler.Instance.Shows(s));
            }
            if (tasks.Count > 0)
            {
                SetBusy(objShows, true);
                await Task.WhenAll(tasks);
                foreach (Task<Shows> t in tasks)
                {
                    if (t.Result.Status == ResponseStatus.Ok)
                        shows.AddRange(t.Result.Items);
                }
                objShows.BeginUpdate();
                objShows.SetObjects(shows);
                objShows.BuildGroups(colShowType, SortOrder.Ascending);
                objShows.BuildList(true);
                objShows.EndUpdate();
                SetBusy(objShows, false);
            }
        }
        private int _lastshowindex = -1;
        private int _lastepisodeindex = -1;

        private async void objShows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((objShows.SelectedIndex != -1) && (_lastshowindex != objShows.SelectedIndex))
            {
                SetBusy(objEpisode, true);
                Show s = (Show)objShows.SelectedObject;
                labShowDescription.Text = s.Description;
                _selected_episodes = await DownloadPluginHandler.Instance.Episodes(s.PluginName, s);
                if (_selected_episodes.Status == ResponseStatus.Ok)
                {
                    objEpisode.Items.Clear();
                    _lastepisodeindex = -1;
                    labEpisodeDescription.Text = string.Empty;
                    pictEpisode.Image = null;
                    if ((_selected_episodes.Items.Any(a => a.SeasonAlpha != string.Empty)))
                    {
                        objEpisode.ShowGroups = true;
                        colEpisodeSeason.IsVisible = true;
                        objEpisode.RebuildColumns();
                        objEpisode.SetObjects(_selected_episodes.Items);
                        objEpisode.BuildGroups(colEpisodeSeason, SortOrder.Ascending);
                        objEpisode.BuildList(true);
                    }
                    else
                    {
                        colEpisodeSeason.IsVisible = false;
                        objEpisode.Groups.Clear();
                        objEpisode.ShowGroups = false;
                        objEpisode.RebuildColumns();
                        objEpisode.SetObjects(_selected_episodes.Items);
                        objEpisode.BuildList(true);
                    }
                    if (_selected_episodes.ImageUri != null)
                        picShow.LoadAsync(_selected_episodes.ImageUri.ToString());
                    if (_selected_episodes.Items.Count > 0)
                        objEpisode.SelectedIndex = 0;

                }
                SetBusy(objEpisode, false);

            }
        }

        private void objEpisode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((objEpisode.SelectedIndex != -1) && (_lastepisodeindex != objEpisode.SelectedIndex))
            {
                Episode ep = (Episode)objEpisode.SelectedObject;
                if (ep.ImageUri != null)
                    pictEpisode.LoadAsync(ep.ImageUri.ToString());
                labEpisodeDescription.Text = ep.Description;
            }

        }


        private async void objEpisode_DoubleClick(object sender, EventArgs e)
        {
            if (objEpisode.SelectedIndex != -1)
            {
                DownloadEpisodeWithRequest((Episode)objEpisode.SelectedObject);
            }
        }
        private void objEpisode_CellClick(object sender, CellClickEventArgs e)
        {
            if ((e.ColumnIndex == olvDownloadBut.Index) && (objEpisode.SelectedIndex == e.RowIndex) )
            {
                Episode ep = (Episode)objEpisode.SelectedObject;
                AddDownloadEpisode(ep);
                objEpisode.RefreshObject(objEpisode.SelectedObject);

            }
        }

        private void objShows_CellClick(object sender, CellClickEventArgs e)
        {
            if ((e.ColumnIndex == olvDownAll.Index) && (objShows.SelectedIndex == e.RowIndex))
            {
                ADBaseLibrary.Show s = (Show)objShows.SelectedObject;
                if (_selected_episodes != null && _selected_episodes.Items.Count > 0 && _selected_episodes.Items[0].ShowId == s.Id)
                {
                    MultiSelect m = new MultiSelect(false, s.Name);
                    m.FileFormats = Settings.Instance.DefaultFormat;
                    m.FileQuality = Settings.Instance.DefaultQuality;
                    m.Episodes = _selected_episodes;
                    DialogResult f = m.ShowDialog();
                    if (f == DialogResult.OK)
                    {
                        foreach (Episode ep in m.Active)
                        {
                            AddDownloadEpisode(ep, m.FileQuality, m.FileFormats);
                        }
                    }
                }
            }
            else if ((e.ColumnIndex == olvFollow.Index) && (objShows.SelectedIndex == e.RowIndex))
            {
                ADBaseLibrary.Show s = (Show) objShows.SelectedObject;
                if (Follows.Instance.IsFollow(s.Id, s.PluginName))
                {
                    DeleteFollow(s);
                }
                else
                {
                    if (_selected_episodes != null && _selected_episodes.Items.Count > 0 &&
                        _selected_episodes.Items[0].ShowId == s.Id)
                    {
                        FollowRequester(s, _selected_episodes);
                    }
                }
                RefreshStatus(s.Id,s.PluginName);
            }
        }

        private void DeleteFollow(Show s)
        {
            DialogResult r = MessageBox.Show("You want to stop following this show?", s.Name, MessageBoxButtons.YesNo);
            if (r == DialogResult.Yes)
            {
                Follows.Instance.RemoveFollow(s.Id, s.PluginName);
            }
        }
        private void FollowRequester(Show s, Episodes eps)
        {
            MultiSelect m = new MultiSelect(true, s.Name);
            m.FileFormats = Settings.Instance.DefaultFormat;
            m.FileQuality = Settings.Instance.DefaultQuality;
            m.Episodes = eps;
            DialogResult f = m.ShowDialog();
            if (f == DialogResult.OK)
            {
                if (Follows.Instance.IsFollow(s.Id, s.PluginName, m.FileQuality, m.FileFormats))
                {
                    Log(LogType.Warn, "You are already following '" + s.Name + "' with this settings");
                }
                else
                {
                    Follows.Instance.AddFollow(s.Id, s.PluginName, m.FileQuality, m.FileFormats);
                    foreach (Episode ep in eps.Items)
                    {
                        Follows.Instance.AddDownload(EpisodeWithDownloadSettings.FromEpisode(ep, m.FileQuality, m.FileFormats));
                    }
                    foreach (Episode ep in m.Active)
                    {
                        AddDownloadEpisode(ep, m.FileQuality, m.FileFormats);
                    }
                }
            }
        }
        private void DownloadEpisodeWithRequest(Episode ep)
        {
 
            Format f = new Format(TemplateParser.FilenameFromEpisode(ep, Settings.Instance.DefaultQuality, Settings.Instance.DownloadTemplate), true);
            f.FileFormats = Settings.Instance.DefaultFormat;
            f.FileQuality = Settings.Instance.DefaultQuality;
            f.Filename = TemplateParser.FilenameFromEpisode(ep, Settings.Instance.DefaultQuality, Settings.Instance.DownloadTemplate);
            DialogResult d = f.ShowDialog();
            if (d == DialogResult.OK)
            {
                AddDownloadEpisode(ep, f.FileQuality, f.FileFormats);

            }
        }

        private void objEpisode_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if ((e.ColumnIndex == olvDownAll.Index) && (objShows.SelectedIndex == e.RowIndex))
            {
                DownloadEpisodeWithRequest((Episode)objEpisode.SelectedObject);
            }
        }
    }
}
