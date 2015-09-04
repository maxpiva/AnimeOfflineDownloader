using System.Collections.Generic;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class Boolean : CheckBox, IInit
    {
        public virtual void Init(Requirement r, Dictionary<string, object> meta)
        {
            Tag = new TagInfo { Req = r, Meta = meta };
            Checked =  meta.GetBoolFromMetadata(r.Name) ?? false;
            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = false;
            CheckedChanged += (a, b) =>
            {
                TagInfo m = (TagInfo)Tag;
                m.Meta[m.Req.Name] = Checked;
            };
        }
    }
}
