using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class Link : LinkLabel, IInit
    {
        public virtual void Init(Requirement r, Dictionary<string, object> meta)
        {
            this.Text = r.Name;
            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = false;
            Font = new Font("Segoe UI", 10);
            this.TextAlign=ContentAlignment.MiddleCenter;
            Click += (a, b) =>
            {
                System.Diagnostics.Process.Start((string) r.Options[0]);
            };

        }
    }
}
