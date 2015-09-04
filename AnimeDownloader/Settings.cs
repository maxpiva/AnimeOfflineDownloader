using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;


namespace AnimeDownloader
{
    public partial class Settings : Form
    {
        public const string VarList = "Possible variables are {show}, {title}, {episodealpha}, {episodenumeric}, {seasonalphaorshow}, {seasonnumeric}, {index}, {plugin} and {resolution}";



        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }
        static void SetDefaultDownloadPath()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.DownloadPath))
            {
                IntPtr outPath;
                int result = SHGetKnownFolderPath(new Guid("{374DE290-123F-4565-9164-39C4925E467B}"), 0x00004000, IntPtr.Zero, out outPath);
                if (result >= 0)
                {
                    Properties.Settings.Default.DownloadPath = Marshal.PtrToStringUni(outPath);
                    Properties.Settings.Default.Save();
                }
            }
        }
        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);


        private void LoadSettings()
        {
            labStatus.Text = VarList;
            if (string.IsNullOrEmpty(Properties.Settings.Default.DownloadPath))
                SetDefaultDownloadPath();
            txtDownloadPath.Text = Properties.Settings.Default.DownloadPath;
            txtTemplate.Text = Properties.Settings.Default.DownloadTemplate;
            ADBaseLibrary.Settings.Instance.OpenSessions = Properties.Settings.Default.PluginSessions;
            ADBaseLibrary.Settings.Instance.AuthorizationsMetadata = Properties.Settings.Default.AuthenticationSettings;
            ADBaseLibrary.Settings.Instance.GlobalMetadata = Properties.Settings.Default.GlobalSettings;
            auth.Init();
            global.Init();

        }

        private void SaveSettings()
        {
            Properties.Settings.Default.DownloadTemplate = txtTemplate.Text;
            Properties.Settings.Default.DownloadPath = txtDownloadPath.Text;
            Properties.Settings.Default.GlobalSettings = ADBaseLibrary.Settings.Instance.GlobalMetadata;
            Properties.Settings.Default.AuthenticationSettings = ADBaseLibrary.Settings.Instance.AuthorizationsMetadata;
            Properties.Settings.Default.PluginSessions = ADBaseLibrary.Settings.Instance.OpenSessions;
            Properties.Settings.Default.Save();
        }


        private bool VerifyDownloadPath()
        {
            bool ex = Directory.Exists(txtDownloadPath.Text);
            if (ex)
                return true;
            labStatus.Text = "Invalid Directory Path";
            labStatus.ForeColor = Color.Red;
            return false;
        }
     
        private async void butOk_Click(object sender, EventArgs e)
        {
            if (await global.VerifyGlobalRequirements() && await auth.VerifyAuthentications() && VerifyDownloadPath())
            {
                SaveSettings();
                await PluginHandler.Instance.Init();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void butDownloads_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (Directory.Exists(txtDownloadPath.Text))
                dialog.SelectedPath = txtDownloadPath.Text;
            DialogResult dl = dialog.ShowDialog();
            if (dl == DialogResult.OK)
            {
                if (Directory.Exists(dialog.SelectedPath))
                {
                    labStatus.Text = VarList;
                    labStatus.ForeColor = Color.Black;
                    txtDownloadPath.Text = dialog.SelectedPath;
                }
            }
        }

        private async void butCancel_Click(object sender, EventArgs e)
        {
            ADBaseLibrary.Settings.Instance.OpenSessions = Properties.Settings.Default.PluginSessions;
            ADBaseLibrary.Settings.Instance.AuthorizationsMetadata = Properties.Settings.Default.AuthenticationSettings;
            ADBaseLibrary.Settings.Instance.GlobalMetadata = Properties.Settings.Default.GlobalSettings;
            await PluginHandler.Instance.Init();
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
