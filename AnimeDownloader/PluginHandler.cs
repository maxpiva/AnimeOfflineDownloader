using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADBaseLibrary;

namespace AnimeDownloader
{

    public class PluginHandler
    {

        public delegate void LogHandler(LogType logtype, string data);
        public event LogHandler OnLogging;



        private Dictionary<string, ISession> PluginSessions = new Dictionary<string, ISession>();
        private Dictionary<string, object> GlobalMetadata = new Dictionary<string, object>();
        private Dictionary<string, Dictionary<string, object>> AuthorizationMetadata = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, IPlugin> Plugins = PluginHandler.LoadedPlugins;

        public List<string> AvailableAuthorizations()
        {
            return AuthorizationMetadata.Keys.ToList();
        }
        public async Task UpdateSettings()
        {
            PluginSessions = Settings.AuthorizedPluginsFromSettings();
            GlobalMetadata = Settings.GlobalMetadataFromSettings();
            AuthorizationMetadata = Settings.AuthorizationMetadataFromSettings();
            foreach (IPlugin p in Plugins.Values)
                await p.SetRequirements(GlobalMetadata);
        }

        private void DoLogging(Response r)
        {
            if (OnLogging != null)
            {
                LogType l = r.Status == ResponseStatus.Ok ? LogType.Info : LogType.Error;
                OnLogging(l, r.ErrorMessage);
            }
        }
        public async Task<T> AuthWrapper<T>(string plugin, Func<IPlugin, ISession,Task<T>> function) where T: Response,new()
        {
            if (!Plugins.ContainsKey(plugin))
            {
                T t = new T();
                t.Status=ResponseStatus.MissingPlugin;
                t.ErrorMessage = "Plugin '" + plugin + "' not found.";
                DoLogging(t);
                return t;
            }
            IPlugin p = Plugins[plugin];
            int retry = 0;
            T r = null;
            do
            {
                if (!PluginSessions.ContainsKey(plugin))
                {
                    if (!AuthorizationMetadata.ContainsKey(plugin))
                    {
                        T t = new T();
                        t.Status = ResponseStatus.MissingAuthenticationParameters;
                        t.ErrorMessage = "Authentication parameters for Plugin '" + plugin + "' are missing.";
                        DoLogging(t);
                        return t;
                    }
                    ISession ses=await p.Authenticate(AuthorizationMetadata[plugin]);
                    if (ses.Status != ResponseStatus.Ok)
                    {
                        T t = new T();
                        ses.PropagateError(t);
                        DoLogging(t);
                        return t;                        
                    }
                    PluginSessions.Add(plugin,ses);
                    Settings.SaveAuthorizations(PluginSessions);
                }
                ISession s = PluginSessions[plugin];
                r = await function(p, s);
                if (r.Status != ResponseStatus.Ok)
                {
                    if (r.Status == ResponseStatus.LoginRequired)
                    {
                        PluginSessions.Remove(plugin);
                        Settings.SaveAuthorizations(PluginSessions);
                    }
                    DoLogging(r);
                }
                retry++;
            } while (r.Status == ResponseStatus.LoginRequired && retry<=3);
            return r;
        }

        public async Task<Shows> Shows(string plugin)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Shows(session));
        }
        public async Task<Episodes> Episodes(string plugin, string showId)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Episodes(session,showId));
        }
        public async Task<Updates> Updates(string plugin)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Updates(session));
        }
        public async Task<Archive> Download(string plugin, string episodeId, string downloadpath, Quality quality, CancellationToken token, IProgress<Progress> progress)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Download(session,episodeId,downloadpath,quality,token,progress));            
        }

    }
}
