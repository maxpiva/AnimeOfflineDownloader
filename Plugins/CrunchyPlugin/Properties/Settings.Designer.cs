﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrunchyPlugin.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.crunchyroll.com/en/videos/{0}/alpha?group=all")]
        public string ShowUrl {
            get {
                return ((string)(this["ShowUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.crunchyroll.com/videos/{0}/updated/ajax_page?pg={1}")]
        public string UpdateUrl {
            get {
                return ((string)(this["UpdateUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<li\\sid.*?group_id=\"(?<id>.*?)\".*?<a.*?title=\"(?<title>.*?)\".*?href=\"(?<url>.*?)\"" +
            ".*?</a>.*?</li>")]
        public string ShowRegex {
            get {
                return ((string)(this["ShowRegex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\#media_group_(?<id>.*?)\".*?\"description\":\"(?<desc>.*?)\"")]
        public string Show2Regex {
            get {
                return ((string)(this["Show2Regex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<li\sid=""showview_videos_media_(?<id>.*?)"".*?<a.*?href=""(?<url>.*?)"".*?<img.*?(src|data-thumbnailUrl)=""(?<image>.*?)"".*?class=""series-title\sblock\sellipsis""\sdir=""auto"">(?<episode>.*?)</span>.*?class=""short-desc""\sdir=""auto"">(?<title>.*?)</p>.*?<script>.*?""description"":(?<description>.*?),""offsetLeft"":")]
        public string EpsRegex {
            get {
                return ((string)(this["EpsRegex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<li\\sid=\"media_group.*?group_id=\"(?<show>.*?)\".*?href=\"(?<url>.*?)\".*?<img.*?src=" +
            "\"(?<image>.*?)\".*?<span.*?>(?<title>.*?)</span>.*?<span.*?>(?<ep>.*?)</span>")]
        public string UpdRegex {
            get {
                return ((string)(this["UpdRegex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("class=\"season-dropdown.*?title=\"(?<season>(.*?))\".*?</ul>")]
        public string SeasonRegex {
            get {
                return ((string)(this["SeasonRegex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<div\\sid=\"sidebar\".*?<img\\sitemprop=\"image\".*?src=\"(?<image>.*?)\".*?/>")]
        public string EpsShowImageRegex {
            get {
                return ((string)(this["EpsShowImageRegex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-r \"{0}\" -a \"{1}\" -y \"{2}\" -o \"{3}\"")]
        public string RTMPDumpArgs {
            get {
                return ((string)(this["RTMPDumpArgs"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("rtmpdump.exe")]
        public string RTMPDumpEXE {
            get {
                return ((string)(this["RTMPDumpEXE"]));
            }
        }
    }
}