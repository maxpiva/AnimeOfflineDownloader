using System.Collections.Generic;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class String : TextBox,IInit
    {
        public virtual void Init(Requirement r, Dictionary<string, object> meta)
        {
            Tag = new TagInfo {Req = r, Meta = meta};
            Text = meta.GetStringFromMetadata(r.Name) ?? string.Empty;
            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = false;
            TextChanged += (a, b) =>
            {
                TagInfo m = (TagInfo) Tag;
                m.Meta[r.Name] = Text;
            };
        }
    }
}
