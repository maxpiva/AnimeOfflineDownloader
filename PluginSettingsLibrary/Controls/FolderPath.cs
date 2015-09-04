using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class FolderPath : Panel,IInit
    {
        public virtual void Init(Requirement r, Dictionary<string, object> meta)
        {
            Tag = new TagInfo { Req = r, Meta = meta };
            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = false;
            Button but = new Button {Text = "...", Size = new Size(24, 23), Dock=DockStyle.Right};
            TextBox txt = new TextBox
            {
                Text = meta.GetStringFromMetadata(r.Name) ?? string.Empty,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(10, 20),
                Location = new Point(0, 2)
            };
            txt.TextChanged += (a, b) =>
            {
                TagInfo m = (TagInfo)Tag;
                m.Meta[m.Req.Name] = Text;
            };
            but.Click+=(a,b)=>
            {
                FolderBrowserDialog dialog=new FolderBrowserDialog();
                if (Directory.Exists(txt.Text))
                    dialog.SelectedPath = txt.Text;
                DialogResult dl = dialog.ShowDialog();
                if (dl == DialogResult.OK)
                {
                    if (Directory.Exists(dialog.SelectedPath))
                        txt.Text = dialog.SelectedPath;
                }
            };
            Panel fill = new Panel { Dock = DockStyle.Fill, Anchor = AnchorStyles.Left | AnchorStyles.Top, Size = new Size(10, 24) };
            fill.Controls.Add(txt);
            Panel spacer = new Panel { Size = new Size(10, 24), Dock = DockStyle.Right };
            Controls.Add(fill);
            Controls.Add(spacer);
            Controls.Add(but);
        }
    }
}
