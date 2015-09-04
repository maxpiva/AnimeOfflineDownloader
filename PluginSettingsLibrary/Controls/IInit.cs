
using System.Collections.Generic;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public interface IInit
    {
        void Init(Requirement r, Dictionary<string, object> meta);
    }
}
