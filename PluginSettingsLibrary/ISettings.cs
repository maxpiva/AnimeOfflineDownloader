using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginSettingsLibrary
{
    public interface ISettings
    {
        void Init();
        Task<bool> Verify(bool force);
    }
}
