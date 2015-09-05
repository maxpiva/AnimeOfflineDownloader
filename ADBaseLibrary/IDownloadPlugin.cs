using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    [InheritedExport]
    public interface IDownloadPlugin
    {
        DownloadPluginInfo Information();
        Task<Response> SetRequirements(Dictionary<string, object> globalmetadata);
        Task<ISession> Authenticate(Dictionary<string, object> authenticationmetadata);
        ISession Deserialize(Dictionary<string,string> data);
        Task<Shows> Shows(ISession session);
        Task<Shows> Shows(ISession session, ShowType type);
        Task<Episodes> Episodes(ISession session, Show show);
        Task<Response> Download(ISession session, Episode episode, string template, string downloadpath, Quality quality, Format format, CancellationToken token, IProgress<DownloadInfo> progress);
        Task<Updates> Updates(ISession session);
        void Exit();
    }
}
