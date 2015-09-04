using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;
using PluginSettingsLibrary.Controls;

namespace PluginSettingsLibrary
{
    public partial class Plugin : UserControl, ISettings
    {


        private int _lastselection=-1;
        public Plugin()
        {
            InitializeComponent();
            cmbPlugin.SelectedIndexChanged += (a, b) =>
            {
                if ((cmbPlugin.SelectedIndex != -1) && (_lastselection != cmbPlugin.SelectedIndex))
                {
                    labStatus.Text = string.Empty;
                    _lastselection = cmbPlugin.SelectedIndex;
                    AddControls();
                    linkRegister.Tag = ((DownloadPluginInfo)cmbPlugin.SelectedItem).RegisterUrl;
                    linkRegister.Text = "Sign Up for " + ((DownloadPluginInfo) cmbPlugin.SelectedItem).Name;
                }
            };
            butTest.Click += async (a, b) =>
            {
                if (cmbPlugin.SelectedIndex != -1)
                {
                    DownloadPluginInfo pinfo = (DownloadPluginInfo)cmbPlugin.SelectedItem;
                    await VerifyAuthentication(pinfo.Name);
                }
            };
            linkRegister.LinkClicked += (a, b) =>
            {
                System.Diagnostics.Process.Start((string) linkRegister.Tag);
            };
        }

        public void Init()
        {
                Dictionary<string, Dictionary<string, object>> auth=new Dictionary<string, Dictionary<string, object>>();
                foreach (string s in DownloadPluginHandler.Instance.Plugins.Keys)
                {
                    DownloadPluginInfo pi = DownloadPluginHandler.Instance.PluginInfos[s];
                    Dictionary<string, object> pmeta;
                    pmeta = Settings.Instance.AuthorizationsMetadataDictionary.ContainsKey(s) ? Settings.Instance.AuthorizationsMetadataDictionary[s] : new Dictionary<string, object>();
                    auth.Add(s, pmeta.JoinMetadata(pi.DefaultAuthenticationData));
                    cmbPlugin.Items.Add(pi);
                }
                Settings.Instance.AuthorizationsMetadataDictionary = auth;
                if (DownloadPluginHandler.Instance.Plugins.Count > 0)
                    cmbPlugin.SelectedIndex = 0;
        }


        private async Task<bool> VerifyAuthentication(string plugin, bool nostatus=false)
        {
            IDownloadPlugin pp = DownloadPluginHandler.Instance.Plugins[plugin];
            Dictionary<string, object> meta = Settings.Instance.AuthorizationsMetadataDictionary[plugin];
            ISession session = await pp.Authenticate(meta);
            if (session.Status == ResponseStatus.Ok)
            {
                if (!nostatus)
                {
                    labStatus.ForeColor = Color.Green;
                    labStatus.Text = "OK";
                }
                if (!Settings.Instance.OpenSessionsDictionary.ContainsKey(plugin))
                    Settings.Instance.OpenSessionsDictionary.Add(plugin, session);
                else
                    Settings.Instance.OpenSessionsDictionary[plugin] = session;
                return true;
            }
            if (!nostatus)
            {
                labStatus.ForeColor = Color.Red;
                labStatus.Text = session.ErrorMessage;
            }
            return false;
        }
        
        public async Task<bool> Verify(bool force)
        {
            if (force)
            {
                foreach (string s in DownloadPluginHandler.Instance.Plugins.Keys)
                {
                    await VerifyAuthentication(s, true);
                }
            }
            return Settings.Instance.OpenSessionsDictionary.Count > 0;
        }
        private void AddControls()
        {
            DownloadPluginInfo pinfo = (DownloadPluginInfo) cmbPlugin.SelectedItem;

            panel.Controls.Clear();
            TableLayoutPanel pn = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = pinfo.AuthenticationRequirements.Count,
                ColumnCount = 2,
                AutoSize = true,
                AutoScroll = true,
                ColumnStyles =
                {
                    new ColumnStyle{Width=30,SizeType=SizeType.Percent},
                    new ColumnStyle{Width=70,SizeType=SizeType.Percent},
                    
                }
            };

            List<Control> labels = pinfo.AuthenticationRequirements.GenerateLabels();
            List<Control> ctrls=pinfo.AuthenticationRequirements.GenerateControls(Settings.Instance.AuthorizationsMetadataDictionary[pinfo.Name]);
            for (int x = 0; x < pinfo.AuthenticationRequirements.Count; x++)
            {
                pn.RowStyles.Add(new RowStyle {Height = 30, SizeType = SizeType.Absolute});
                pn.Controls.Add(labels[x]);
                pn.Controls.Add(ctrls[x]);
                pn.SetColumn(labels[x],0);
                pn.SetRow(labels[x], x);
                pn.SetColumn(ctrls[x],1);
                pn.SetRow(ctrls[x],x);
            }
            pn.RowStyles.Add(new RowStyle { Height = 1, SizeType = SizeType.Percent });
            Panel pp = new Panel
            {
                Dock = DockStyle.Fill,
            };
            pn.Controls.Add(pp);
            pn.SetColumn(pp, 0);
            pn.SetRow(pp, pinfo.AuthenticationRequirements.Count);
            pn.SetColumn(pp, 2);
            panel.Controls.Add(pn);
            panel.Controls.Add(pn);
        }


    }
}
