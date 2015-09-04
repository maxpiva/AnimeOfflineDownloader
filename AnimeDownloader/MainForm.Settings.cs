using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADBaseLibrary;

namespace AnimeDownloader
{
    public partial class MainForm
    {

        public void LoadSettings()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ADSettings))
                ADBaseLibrary.Settings.Instance.Deserialize(Properties.Settings.Default.ADSettings);
            settingsAuth.Init();
            settingsGlobal.Init();
            settingsDownload.Init();
        }

        public async Task<bool> VerifySettings(bool firsttime=false)
        {
            bool res =  await settingsGlobal.Verify(!firsttime) && await settingsDownload.Verify(!firsttime) && await settingsAuth.Verify(!firsttime);
            TabDisable = !res;
            if (!res)
                tabs.SelectedIndex = 3;

            return res;
        }
        public void SaveSettings()
        {
            Properties.Settings.Default.ADSettings = ADBaseLibrary.Settings.Instance.Serialize();
            Properties.Settings.Default.Save();
            _manager.MaxDownloads=Settings.Instance.SimultaneousDownloads;

        }
        private async void butCheckSettings_Click(object sender, EventArgs e)
        {
            bool res=await VerifySettings();
            if (res)
            {
                SaveSettings();
                if (objDownloads.Items.Count==0)
                    LoadDownloadItems();
                Task.Run(async () => await RefreshUpdates());
                await PopulateSeries();

            }
        }

    }
}
