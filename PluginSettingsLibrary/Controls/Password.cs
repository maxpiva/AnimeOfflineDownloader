using System.Collections.Generic;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public class Password : String
    {
        public override void Init(Requirement r, Dictionary<string, object> meta)
        {
            base.Init(r,meta);
            PasswordChar = '*';
        }
    }
}
