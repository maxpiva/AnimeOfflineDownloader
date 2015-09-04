using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;
using PluginSettingsLibrary.Controls;

namespace PluginSettingsLibrary
{
    public partial class Global : UserControl, ISettings
    {


        public Global()
        {
            InitializeComponent();
        }

        public void Init()
        {
            Dictionary<string, object>  meta = new Dictionary<string, object>();
            foreach (string s in DownloadPluginHandler.Instance.Plugins.Keys)
            {
                DownloadPluginInfo pi = DownloadPluginHandler.Instance.PluginInfos[s];
                Dictionary<string, object> ndata = Settings.Instance.GlobalMetadataDictionary.JoinMetadata(pi.DefaultGlobalData);
                foreach (string n in ndata.Keys)
                {
                    if (!meta.ContainsKey(n))
                        meta.Add(n, ndata[n]);
                }
            }
            Settings.Instance.GlobalMetadataDictionary = meta;
            AddControls();
        }




        public async Task<bool> Verify(bool force)
        {
            bool error = false;
            foreach (IDownloadPlugin p in DownloadPluginHandler.Instance.Plugins.Values)
            {
                Response r = await p.SetRequirements(Settings.Instance.GlobalMetadataDictionary);
                if (r.Status != ResponseStatus.Ok)
                {
                    error = true;
                    labStatus.Text = r.ErrorMessage;
                    labStatus.ForeColor = Color.Red;
                    break;
                }
            }
            if (!error)
            {
                labStatus.Text = "OK";
                labStatus.ForeColor = Color.Green;
            }
            return !error;
        }
        private List<Requirement> GetAllRequirements()
        {
            List<Requirement> reqs=new List<Requirement>();
            foreach (string s in DownloadPluginHandler.Instance.PluginInfos.Keys)
            {
                foreach (Requirement r in DownloadPluginHandler.Instance.PluginInfos[s].GlobalRequirements)
                {
                    if (!reqs.Any(a=>a.Name==r.Name))
                        reqs.Add(r);
                }
            }
            return reqs;
        }

        private void AddControls()
        {
            List<Requirement> reqs = GetAllRequirements();
            panel.Controls.Clear();
            TableLayoutPanel pn = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = reqs.Count,
                ColumnCount = 2,
                AutoSize = true,
                AutoScroll = true,                ColumnStyles =
                {
                    new ColumnStyle{Width=30,SizeType=SizeType.Percent},
                    new ColumnStyle{Width=70,SizeType=SizeType.Percent},
                    
                }
            };
            List<Control> labels = reqs.GenerateLabels();
            List<Control> ctrls = reqs.GenerateControls(Settings.Instance.GlobalMetadataDictionary);
            for (int x = 0; x < reqs.Count; x++)
            {
                pn.RowStyles.Add(new RowStyle { Height = 30, SizeType = SizeType.Absolute });
                pn.Controls.Add(labels[x]);
                pn.Controls.Add(ctrls[x]);
                pn.SetColumn(labels[x], 0);
                pn.SetRow(labels[x], x);
                pn.SetColumn(ctrls[x], 1);
                pn.SetRow(ctrls[x], x);
            }
            pn.RowStyles.Add(new RowStyle { Height = 1, SizeType = SizeType.Percent });
            Panel pp = new Panel
            {
                Dock = DockStyle.Fill,
            };
            pn.Controls.Add(pp);
            pn.SetColumn(pp,0);
            pn.SetRow(pp,reqs.Count);
            pn.SetColumn(pp,2);
            panel.Controls.Add(pn);
        }
    }
}
