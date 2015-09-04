using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ADBaseLibrary
{
    public static class TemplateParser
    {
        public static List<string> Variables { get; } = Enum.GetNames(typeof (TemplateVariables)).ToList();

        private static Regex _varRegex=new Regex("{(?<variable>.*?)}",RegexOptions.Singleline|RegexOptions.Compiled);

        public static bool ValidateTemplate(string template, out string unknownvariable)
        {
            MatchCollection m = _varRegex.Matches(template);
            foreach (Match match in m)
            {
                if (match.Success)
                {
                    string t = match.Groups["variable"].Value.ToLower();
                    int idx = t.IndexOf(":",StringComparison.InvariantCulture);
                    if (idx > 0)
                        t = t.Substring(0, idx);
                    if (!Variables.Contains(t))
                    {
                        unknownvariable = t;
                        return false;
                    }
                }
            }
            unknownvariable = null;
            return true;
        }

        public static string FilenameFromEpisode(Episode ep, Quality q, string template)
        {
            List<object> vars=new List<object>();
            MatchCollection m = _varRegex.Matches(template);
            int cnt = 0;
            foreach (Match match in m)
            {
                if (match.Success)
                {
                    string t = match.Groups["variable"].Value.ToLower();
                    int idx = t.IndexOf(":", StringComparison.InvariantCulture);
                    if (idx > 0)
                        t = t.Substring(0, idx);
                    if (Variables.Contains(t))
                    {
                        TemplateVariables tv = (TemplateVariables) Enum.Parse(typeof (TemplateVariables), t);
                        switch (tv)
                        {
                            case TemplateVariables.show:
                                vars.Add(ep.ShowName);
                                break;
                            case TemplateVariables.episodealpha:
                                vars.Add(ep.EpisodeAlpha ?? string.Empty);
                                break;
                            case TemplateVariables.episodenumeric:
                                vars.Add(ep.EpisodeNumeric);
                                break;
                            case TemplateVariables.seasonalpha:
                                vars.Add(ep.SeasonAlpha ?? string.Empty);
                                break;
                            case TemplateVariables.seasonalphaorshow:
                                if (string.IsNullOrEmpty(ep.SeasonAlpha))
                                    vars.Add(ep.ShowName ?? string.Empty);
                                else
                                    vars.Add(ep.SeasonAlpha);
                                break;
                            case TemplateVariables.seasonnumeric:
                                vars.Add(ep.SeasonNumeric);
                                break;
                            case TemplateVariables.index:
                                vars.Add(ep.Index);
                                break;
                            case TemplateVariables.plugin:
                                vars.Add(ep.PluginName);
                                break;
                            case TemplateVariables.resolution:
                                vars.Add(q.ToText());
                                break;
                        }
                        template = template.Replace("{" + t, "{" + cnt);
                        cnt++;
                    }
                }
            }
            string fname= string.Format(template, vars.ToArray());
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                fname = fname.Replace(c.ToString(), string.Empty);
            return fname;
        }
    }
}
