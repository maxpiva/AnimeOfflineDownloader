using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;

namespace AnimeDownloader
{
    public partial class MultiSelect : Form
    {
        private bool _follow = false;
        private Episodes _episodes;
        public Episodes Episodes
        {
            get { return _episodes; }
            set
            {
                _episodes = value;
                List<Episode> eps =  _follow ? _episodes.Items.OrderByDescending(a => a.SeasonAlpha).ThenByDescending(a => a.EpisodeNumeric).ToList() : _episodes.Items.OrderBy(a => a.SeasonAlpha).ThenBy(a => a.EpisodeNumeric).ToList();
                objEpisodes.SetObjects(eps);
                if ((_follow) && (eps.Count>0))
                    objEpisodes.CheckObject(eps[0]);
                else
                    objEpisodes.CheckAll();
            }
        }

        public List<Episode> Active
        {
            get
            {
                List<Episode> eps=new List<Episode>();
                foreach (Episode ep in objEpisodes.CheckedObjects)
                {
                    eps.Add(ep);
                }
                return eps;
            }
        } 
        private ADBaseLibrary.Format _formats;

        public ADBaseLibrary.Format FileFormats
        {
            get { return _formats; }
            set
            {
                _formats = value;
                chkListBox.Items.Clear();
                foreach (ADBaseLibrary.Format f in Enum.GetValues(typeof(ADBaseLibrary.Format)))
                {
                    bool active = (f & value) == f;
                    chkListBox.Items.Add(f.ToText(), active);
                }
            }
        }

        private Quality _quality;

        public Quality FileQuality
        {
            get { return _quality; }
            set
            {
                _quality = value;
                cmbQuality.SelectedIndex = Enum.GetValues(typeof(Quality)).Cast<Quality>().ToList().IndexOf(value);
                objEpisodes.BuildList(true);
            }
        }
        public MultiSelect(bool follow, string showname)
        {
            InitializeComponent();
            _follow = follow;
            this.Text = (follow ? " Follow " : string.Empty) + showname+" - "+(follow ? "Select additional Episodes to download..." : "Select Episodes to download...");
            butOk.Text = follow ? "Follow" : "Download";
            olvFile.AspectGetter = (d) =>
            {
                Episode e = (Episode) d;
                return TemplateParser.FilenameFromEpisode(e,_quality,Settings.Instance.DownloadTemplate);
            };
            foreach (Quality q in Enum.GetValues(typeof(Quality)))
                cmbQuality.Items.Add(q.ToText());
            cmbQuality.SelectedIndexChanged += (a, b) =>
            {
                if (cmbQuality.SelectedIndex != -1)
                    FileQuality = (Quality)cmbQuality.SelectedIndex;
            };
            int tl = 0;
            foreach (ADBaseLibrary.Format f in Enum.GetValues(typeof(ADBaseLibrary.Format)))
            {
                tl += 18;
                chkListBox.Items.Add(f.ToText(), false);
            }
            chkListBox.Height = tl;
            chkListBox.ItemCheck += (a, b) =>
            {
                ADBaseLibrary.Format[] fmts = Enum.GetValues(typeof(ADBaseLibrary.Format)).Cast<ADBaseLibrary.Format>().ToArray();
                if (b.NewValue == CheckState.Checked)
                {
                    _formats |= fmts[b.Index];
                }
                else if (b.NewValue == CheckState.Unchecked)
                {
                    _formats &= ~fmts[b.Index];
                }
            };
        }
    }
}
