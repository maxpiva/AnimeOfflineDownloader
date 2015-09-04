using System.Collections.Generic;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class Integer : NumericUpDown, IInit
    {
        public virtual void Init(Requirement r, Dictionary<string, object> meta)
        {
            Minimum = 0;
            Maximum = int.MaxValue;
            Tag = new TagInfo { Req = r, Meta = meta };
            Value = (meta.GetIntFromMetadata(r.Name) ?? 0);
            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = false;
            ValueChanged += (a, b) =>
            {
                TagInfo m = (TagInfo)Tag;
                m.Meta[r.Name] = (int)Value;
            };
        }
    }
}
