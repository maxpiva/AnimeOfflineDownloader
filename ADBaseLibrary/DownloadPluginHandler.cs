using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class DownloadPluginHandler
    {
        public delegate void LogHandler(LogType logtype, string data);
        public delegate void SettingsChanged();
        
        public event LogHandler OnLogging;
        public event SettingsChanged OnSettingsChanged;
        



        private static DownloadPluginHandler _instance;
        public static DownloadPluginHandler Instance { get { return _instance ?? (_instance = new DownloadPluginHandler()); } }
        public List<string> AvailableAuthorizations
        {
            get
            {
                if (Settings.Instance.AuthorizationsMetadataDictionary != null)
                    return Settings.Instance.AuthorizationsMetadataDictionary.Keys.ToList();
                return new List<string>();
            }
        }

        public Dictionary<string, IDownloadPlugin> Plugins { get; private set; }
        public Dictionary<string, DownloadPluginInfo> PluginInfos { get; private set; }
            
        [ImportMany(typeof(IDownloadPlugin))]
        public IEnumerable<IDownloadPlugin> List { get; set; }




        public async Task Init()
        {
            foreach (IDownloadPlugin p in Plugins.Values)
                await p.SetRequirements(Settings.Instance.GlobalMetadataDictionary);
        }

        private void DoLogging(Response r)
        {
            if (OnLogging != null)
            {
                LogType l = r.Status == ResponseStatus.Ok ? LogType.Info : LogType.Error;
                OnLogging(l, r.ErrorMessage);
            }
        }

        private void DoChangeSettings()
        {
            if (OnSettingsChanged != null)
                OnSettingsChanged();
        }
        public async Task<T> AuthWrapper<T>(string plugin, Func<IDownloadPlugin, ISession, Task<T>> function) where T : Response, new()
        {
            if (!Plugins.ContainsKey(plugin))
            {
                T t = new T();
                t.Status = ResponseStatus.MissingPlugin;
                t.ErrorMessage = "Plugin '" + plugin + "' not found.";
                DoLogging(t);
                return t;
            }
            IDownloadPlugin p = Plugins[plugin];
            int retry = 0;
            T r;
            do
            {
                if (!Settings.Instance.OpenSessionsDictionary.ContainsKey(plugin))
                {
                    if (!Settings.Instance.AuthorizationsMetadataDictionary.ContainsKey(plugin))
                    {
                        T t = new T();
                        t.Status = ResponseStatus.MissingAuthenticationParameters;
                        t.ErrorMessage = "Authentication parameters for Plugin '" + plugin + "' are missing.";
                        DoLogging(t);
                        return t;
                    }
                    ISession ses = await p.Authenticate(Settings.Instance.AuthorizationsMetadataDictionary[plugin]);
                    if (ses.Status != ResponseStatus.Ok)
                    {
                        T t = new T();
                        ses.PropagateError(t);
                        DoLogging(t);
                        return t;
                    }
                    Settings.Instance.OpenSessionsDictionary.Add(plugin, ses);
                    DoChangeSettings();
                }
                ISession s = Settings.Instance.OpenSessionsDictionary[plugin];
                r = await function(p, s);
                if (r.Status != ResponseStatus.Ok)
                {
                    if (r.Status == ResponseStatus.LoginRequired)
                    {
                        Settings.Instance.OpenSessionsDictionary.Remove(plugin);
                        DoChangeSettings();
                    }
                    DoLogging(r);
                }
                retry++;
            } while (r.Status == ResponseStatus.LoginRequired && retry <= Settings.Instance.NumberOfRetries);
            return r;
        }

        public async Task<Shows> Shows(string plugin)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Shows(session));
        }
        public async Task<Episodes> Episodes(string plugin, Show show)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Episodes(session, show));
        }
        public async Task<Updates> Updates(string plugin)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Updates(session));
        }
        public async Task<Response> Download(string plugin, Episode ep, string template, string downloadpath, Quality quality, Format format, CancellationToken token, IProgress<DownloadInfo> progress)
        {
            return await AuthWrapper(plugin, async (plg, session) => await plg.Download(session, ep, template, downloadpath, quality, format, token, progress));
        }

        public void Exit()
        {
            foreach (IDownloadPlugin plugin in Plugins.Values)
            {
                plugin.Exit();
            }
        }


        public DownloadPluginHandler()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string dirname = System.IO.Path.GetDirectoryName(assembly.GetName().CodeBase);
            if (dirname != null)
            {
                if (dirname.StartsWith(@"file:\"))
                    dirname = dirname.Substring(6);
                AggregateCatalog catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                catalog.Catalogs.Add(new DirectoryCatalog(dirname));
                var container = new CompositionContainer(catalog);
                container.ComposeParts(this);
            }
            Plugins = new Dictionary<string, IDownloadPlugin>();
            PluginInfos=new Dictionary<string, DownloadPluginInfo>();
            foreach (IDownloadPlugin f in List)
            {
                DownloadPluginInfo pinfo = f.Information();
                Plugins.Add(pinfo.Name, f);
                PluginInfos.Add(pinfo.Name,pinfo);
            }
        }


    }

}
