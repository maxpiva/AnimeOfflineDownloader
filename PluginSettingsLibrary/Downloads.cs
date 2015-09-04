using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary
{
    public partial class Downloads : UserControl , ISettings
    {
  
        public Downloads()
        {
            InitializeComponent();
        }

        public void Init()
        {
            txtDownloadPath.Text = Settings.Instance.DownloadPath;
            txtTemplate.Text = Settings.Instance.DownloadTemplate;
            numRetries.Value = Settings.Instance.NumberOfRetries;
            numDownloads.Value = Settings.Instance.SimultaneousDownloads;
            string jn = string.Join(", ", TemplateParser.Variables.Select(a=>"{"+a+"}"));
            int idx = jn.LastIndexOf(',');
            labTemplate.Text= "Possible variables are: " + jn.Substring(0, idx) + " and " + jn.Substring(idx + 2);
            List<Quality> qtys = Enum.GetValues(typeof (Quality)).Cast<Quality>().ToList();
            List<Format> fmts = Enum.GetValues(typeof (Format)).Cast<Format>().ToList();
            foreach (Quality q in qtys)
                cmbQuality.Items.Add(q.ToText());
            foreach (Format f in fmts)
                cmbFormat.Items.Add(f.ToText());
            cmbQuality.SelectedIndex = qtys.IndexOf(Settings.Instance.DefaultQuality);
            cmbFormat.SelectedIndex = fmts.IndexOf(Settings.Instance.DefaultFormat);
            if (cmbQuality.SelectedIndex == -1)
                cmbQuality.SelectedIndex = 0;
            if (cmbFormat.SelectedIndex == -1)
                cmbFormat.SelectedIndex = 0;
        }

        public Task<bool> Verify(bool force)
        {
            string errorText = null;
            string res = null;
            if (string.IsNullOrEmpty(txtDownloadPath.Text))
                errorText = "Download Path is empty";
            else if (!Directory.Exists(txtDownloadPath.Text))
                errorText = "Download Path '" + txtDownloadPath.Text + " ' do not exist";
            else if (string.IsNullOrEmpty(txtTemplate.Text))
                errorText = "File Template is empty";
            else if (!TemplateParser.ValidateTemplate(txtTemplate.Text, out res))
                errorText = "File Template has invalid variable '" + res + "'";
            if (errorText != null)
            {
                labStatus.Text = errorText;
                labStatus.ForeColor = Color.Red;
                return Task.FromResult(false);
            }
            labStatus.Text = "OK";
            labStatus.ForeColor = Color.Green;
            Settings.Instance.DownloadPath = txtDownloadPath.Text;
            Settings.Instance.DownloadTemplate = txtTemplate.Text;
            Settings.Instance.NumberOfRetries = (int)numRetries.Value;
            Settings.Instance.SimultaneousDownloads = (int)numDownloads.Value;
            Settings.Instance.DefaultFormat = Enum.GetValues(typeof(Format)).Cast<Format>().ToList()[cmbFormat.SelectedIndex];
            Settings.Instance.DefaultQuality = (Quality) cmbQuality.SelectedIndex;
            return Task.FromResult(true);
        }
    }
}
