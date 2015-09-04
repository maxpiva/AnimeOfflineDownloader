using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADBaseLibrary.Helpers
{
    public class RTMPDumpParser : ShellParser
    {
        private static Regex _percentage=new Regex("\\((?<percent>(.*?))\\%\\)",RegexOptions.Compiled|RegexOptions.Singleline);

        public delegate void ProgressHandler(double percentage);

        public event ProgressHandler OnProgress;

        public RTMPDumpParser()
        {
            this.OnError += RTMPDumpParser_OnLine;
            this.OnLine += RTMPDumpParser_OnLine;
        }

        private void RTMPDumpParser_OnLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                Match m = _percentage.Match(line);
                if (m.Success)
                {
                    double dbl = 0;
                    if (double.TryParse(m.Groups["percent"].Value,NumberStyles.Any,CultureInfo.InvariantCulture, out dbl))
                    {
                        if (OnProgress!=null)
                            OnProgress(dbl);
                    }
                }
                else if (line.Contains("Download complete"))
                {
                    if (OnProgress != null)
                        OnProgress(100);
                }
            }
        }

    }
}
