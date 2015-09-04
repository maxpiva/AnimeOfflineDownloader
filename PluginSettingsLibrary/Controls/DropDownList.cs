using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class DropDownList : ComboBox, IInit
    {
        public virtual void Init(Requirement r, Dictionary<string, object> meta)
        {
            Tag = new TagInfo { Req = r, Meta = meta };
            foreach (string op in r.Options)
                Items.Add(op);
            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = false;
            DropDownStyle=ComboBoxStyle.DropDownList;
            SelectedIndexChanged += (a, b) =>
            {
                if (SelectedIndex != -1)
                {
                    TagInfo m = (TagInfo)Tag;
                    m.Meta[r.Name] = (string) SelectedItem;
                }
            };
            string def = meta.GetStringFromMetadata(r.Name);
            if (r.Options.Contains(def))
            {
                SelectedIndex = r.Options.IndexOf(def);
            }
        }
    }
}
