using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ADBaseLibrary
{
    public class DownloadPluginInfo
    {
        public const string Username = "Username";
        public const string Password = "Password";
        public const string ProxyEnabled = "Proxy Enabled";
        public const string ProxyAddress = "Proxy Address";
        public const string ProxyPort = "Proxy Port";
        public const string ProxyUsername = "Proxy Username";
        public const string ProxyPassword = "Proxy Password";

        public virtual string Name { get { return string.Empty; }}
        public virtual string Version { get { return string.Empty; }}
        public virtual string RegisterUrl {  get { return string.Empty; } }
        public override string ToString()
        {
            return Name;
        }

        public virtual List<Requirement> AuthenticationRequirements
        {
            get
            {
                return new List<Requirement>
                {
                    new Requirement { Name=Username, RequirementType = RequirementType.Required|RequirementType.String},
                    new Requirement { Name=Password, RequirementType  = RequirementType.Required|RequirementType.Password}
                };
            }

        }

        public virtual List<Requirement> GlobalRequirements
        {
            get
            {
                return new List<Requirement>
                {
                    new Requirement { Name=ProxyEnabled, RequirementType = RequirementType.Required|RequirementType.Bool},
                    new Requirement { Name=ProxyAddress, RequirementType = RequirementType.String},
                    new Requirement { Name=ProxyPort, RequirementType = RequirementType.Integer},
                    new Requirement { Name=ProxyUsername, RequirementType = RequirementType.String},
                    new Requirement { Name=ProxyPassword, RequirementType = RequirementType.Password}
                }; 
            }

        }

        public virtual Dictionary<string, object> DefaultAuthenticationData
        {
            get 
            {
                return new Dictionary<string, object>
                {
                    {Username, string.Empty}, 
                    {Password, string.Empty}
                };
            }
        }



        public virtual Dictionary<string, object> DefaultGlobalData
        {
            get
            {
                return new Dictionary<string, object>
                {
                    {ProxyEnabled, false},
                    {ProxyAddress, string.Empty},
                    {ProxyPort, 8080},
                    {ProxyUsername, string.Empty},
                    {ProxyPassword, string.Empty}
                };
            }
        }
    }
}
