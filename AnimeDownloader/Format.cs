using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ADBaseLibrary;

namespace AnimeDownloader
{
    public partial class Format : Form
    {
        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                labFile.Text = value;
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
                foreach(ADBaseLibrary.Format f in Enum.GetValues(typeof(ADBaseLibrary.Format)))
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
                _quality= value;
                cmbQuality.SelectedIndex = Enum.GetValues(typeof(Quality)).Cast<Quality>().ToList().IndexOf(value);
            }
        }


        public Format(string name, bool download)
        {
            
            InitializeComponent();
            Text = download ? name + " - Select Download Options" : name + " - Change Download Options";
            butOk.Text = download ? "Download" : "Change";
            foreach (Quality q in Enum.GetValues(typeof(Quality)))
                cmbQuality.Items.Add(q.ToText());           
            cmbQuality.SelectedIndexChanged += (a, b) =>
            {
                if (cmbQuality.SelectedIndex != -1)
                    FileQuality = (Quality) cmbQuality.SelectedIndex;
            };
            foreach (ADBaseLibrary.Format f in Enum.GetValues(typeof(ADBaseLibrary.Format)))
            {
                chkListBox.Items.Add(f.ToText(), false);
            }
            int tl = 0;
            chkListBox.ItemCheck += (a, b) =>
            {
                tl += 18;
                ADBaseLibrary.Format[] fmts=Enum.GetValues(typeof (ADBaseLibrary.Format)).Cast<ADBaseLibrary.Format>().ToArray();
                if (b.NewValue==CheckState.Checked)
                {
                    _formats |= fmts[b.Index];
                }
                else if (b.NewValue == CheckState.Unchecked)
                {
                    _formats &= ~fmts[b.Index];
                }
            };
            chkListBox.Height = tl;
        }
    }
}
