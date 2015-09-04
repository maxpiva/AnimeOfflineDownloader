using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;
using ADBaseLibrary.Helpers;
using BrightIdeasSoftware;


namespace AnimeDownloader
{
    public partial class MainForm : Form
    {
        private bool TabDisable = false;

        private TaskScheduler mainScheduler;

        public MainForm()
        {
            UserDataPath.AppPath = "AnimeDownloader";
            InitializeComponent();
        }

        public async Task WrapGuiAction(Action a)
        {
            await Task.Factory.StartNew(a, CancellationToken.None, TaskCreationOptions.None, mainScheduler);
        }


        private async Task InitPlugins()
        {

            DownloadPluginHandler.Instance.OnLogging += async (ty, str) =>
            {
                await WrapGuiAction(() => Log(ty, str));
            };
            DownloadPluginHandler.Instance.OnSettingsChanged+=()=>
            {
                Properties.Settings.Default.ADSettings = Settings.Instance.Serialize();
                Properties.Settings.Default.Save();
            };
            await DownloadPluginHandler.Instance.Init();
        }

        private void Log(LogType tp, string str)
        {
            Log l = new Log
            {
                Type = tp,
                Text = str,
                DateTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
            };
            objLog.AddObject(l);
            objLog.EnsureModelVisible(l);
        }
        public void SetBusy(ObjectListView lv,bool busy)
        {
            if (busy)
            {

                lv.OverlayText = GenerateOverlay();
            }
            else
            {
                lv.OverlayText = null;
            }
        }

        TextOverlay GenerateOverlay()
        {
            TextOverlay ov = new TextOverlay();
            ov.Alignment = ContentAlignment.MiddleCenter;
            ov.TextColor = Color.White;
            ov.BackColor = Color.GreenYellow;
            ov.BorderColor = Color.Black;
            ov.BorderWidth = 4.0f;
            ov.Font = new Font("Segoe UI", 36);
            ov.Text = "Loading...";
            return ov;
        }
        


        private async void Form1_Load(object sender, EventArgs e)
        {
            mainScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            InitShowLists();
            InitUpdateList();
            InitDownloadManager();
            await InitPlugins();
            LoadSettings();
            if (await VerifySettings(true))
            {
                LoadDownloadItems();
                Task.Run(async () => await RefreshUpdates());
                await PopulateSeries();
            }

        }

        private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if ((e.TabPageIndex!=3) && (TabDisable))
                    e.Cancel=true;
        }

        private void objLog_FormatCell(object sender, FormatCellEventArgs e)
        {
            Log l = (Log) e.Model;
            switch (l.Type)
            {
                case LogType.Info:
                    e.SubItem.ForeColor = SystemColors.ControlText;
                    break;
                case LogType.Warn:
                    e.SubItem.ForeColor = Color.FromArgb(255, 180, 180, 0);
                    break;
                case LogType.Error:
                    e.SubItem.ForeColor = Color.Red;
                    break;
            }
        }

    }
}
