using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxShockwaveFlashObjects;

namespace ADBaseLibrary.AdobeHDS.FlashWrapper
{
    public partial class DecryptForm : Form
    {
        //This form will load an swf, that will download the akamai swf library in charge of decrypting the HDS Stream.
        public static DecryptForm form;
        internal  AxShockwaveFlash axShock;
        private static Thread thread;
        private static bool _isinit = false;
        private static TaskScheduler thscheduler;

        public static void Init()
        {
            if (_isinit)
                return;
            _isinit = true;
            thread = new Thread(CreateComponent);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(null);
            AppDomain.CurrentDomain.FirstChanceException += (a,b) => { };
        }



        public static void Kill()
        {
            if (thread != null)
            {
                Task.Factory.StartNew(Application.Exit, CancellationToken.None, TaskCreationOptions.None, thscheduler);
                thread = null;
            }
        }

        private static void CreateComponent(object obj)
        {
            //Flash Control created on other task, otherwise, when decrypting it'll block the main UI Thread


            ComponentResourceManager resources = new ComponentResourceManager(typeof(DecryptForm));
            form = new DecryptForm();
            form.axShock = new AxShockwaveFlash();
            form.axShock.Enabled = true;
            form.axShock.Location = new Point(1, 1);
            form.axShock.Name = "axShock";
            form.axShock.OcxState = ((AxHost.State)(resources.GetObject("axShock.OcxState")));
            form.axShock.Size = new Size(1,1);
            form.axShock.TabIndex = 0;
            form.Controls.Add(form.axShock);
            form.Show();
            form.Hide();
            string swf = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "loader.swf");
            if (File.Exists(swf))
                form.axShock.LoadMovie(0, swf);
            thscheduler= TaskScheduler.FromCurrentSynchronizationContext();
            Application.Run();
            form.axShock.Stop();
            form.axShock.Dispose();
        }
        public DecryptForm()
        {
            InitializeComponent();


        }

        private static object decryptLock = new object();

        public static string Decrypt(string datas, string keys)
        {
            try
            {
                lock (decryptLock)
                {
                    string result =
                        form.axShock.CallFunction("<invoke name=\"decrypt\" returntype=\"xml\"><arguments><string>" +
                                                  datas + "</string><string>" + keys + "</string></arguments></invoke>");

                    return result.Substring(8, result.Length - 17);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }
    }
}
