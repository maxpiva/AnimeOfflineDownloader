using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ADBaseLibrary.Subtitles
{
    public class TTML
    {
        private static Regex BrRegex = new Regex("<br\\s*[/]?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static string MatchRegex = "<{0}[^>]*>(?<data>.*?)</{0}>";
        private static Regex SpanRegex = new Regex("<span(?<span>[^>]*)>(?<data>.*?)</span>", RegexOptions.IgnoreCase|RegexOptions.Compiled|RegexOptions.Singleline);
        private static Regex StyleRegex = new Regex("style=\"(?<style>.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex RemoveTags = new Regex("<(style|script)[^<>]*>.*?</\\1>|</?[a-z][a-z0-9]*[^<>]*>", RegexOptions.Compiled);
        private Tt subtitles;

        private static Dictionary<string,string> knowcolors=new Dictionary<string, string>();

        static TTML()
        {
            foreach (KnownColor c in Enum.GetValues(typeof (KnownColor)))
            {
                Color m = System.Drawing.Color.FromKnownColor(c);
                int final = m.ToArgb();
                string ret= "&H";
                ret += ((0xFF - (final >> 24)) & 0xFF).ToString("X2");
                ret += (final & 0xFF).ToString("X2");
                ret += ((final >> 8) & 0xFF).ToString("X2");
                ret += ((final >> 16) & 0xFF).ToString("X2");
                knowcolors.Add(c.ToString().ToLower(), ret);
            }
        }
        public TTML(Stream stream)
        {
            FromStream(stream);
        }
        public TTML(string xml)
        {
            Stream s=new MemoryStream(Encoding.UTF8.GetBytes(xml));
            FromStream(s);
            s.Dispose();
        }

        private void FromStream(Stream s)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Tt));
            serializer.UnknownElement += Serializer_UnknownElement;
            serializer.UnknownNode += Serializer_UnknownNode;
            subtitles = (Tt)serializer.Deserialize(s);
        }



        private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            P p = e.ObjectBeingDeserialized as P;
            if (p != null)
            {
                if (p.Text == null)
                    p.Text = string.Empty;
                if (e.Name == "#text")
                     p.Text += e.Text;

            }
        }

        private void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            P p = e.ObjectBeingDeserialized as P;
            if (p != null)
            {
                p.Text += e.Element.OuterXml.Replace(" xmlns=\"http://www.w3.org/2006/04/ttaf1\"",string.Empty);
            }

        }



        public Dictionary<string, string> ToAss()
        {
            Dictionary<string,string> ret=new Dictionary<string, string>();
            Dictionary<string, string> styles = GetAssStyles();
            if (subtitles.Body?.Div != null && subtitles.Body.Div.Count > 0)
            {
                foreach (Div d in subtitles.Body.Div)
                {
                    StringBuilder bld = new StringBuilder();
                    bld.AppendLine("[Script Info]");
                    bld.AppendLine("Title: " + d.Lang);
                    bld.AppendLine("ScriptType: v4.00+");
                    bld.AppendLine("WrapStyle: 0");
                    bld.AppendLine("PlayResX: 640");
                    bld.AppendLine("PlayResY: 360");
                    bld.AppendLine("");
                    bld.AppendLine("[V4+ Styles]");
                    bld.AppendLine("Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding");
                    foreach (string s in styles.Keys)
                        bld.AppendLine(styles[s]);
                    bld.AppendLine("");
                    bld.AppendLine("[Events]");
                    bld.AppendLine("Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text");
                    foreach (P p in d.P)
                        bld.AppendLine(ParagraphToAss(styles, p));
                    ret.Add(d.Lang, bld.ToString());
                }
            }
            return ret;

        }

        private Dictionary<string, string> GetAssStyles()
        {
            Dictionary<string, string> ret=new Dictionary<string, string>();
            if (subtitles.Head?.Styling?.Style == null || subtitles.Head.Styling.Style.Count==0)
                return ret;
            foreach (Style s in subtitles.Head.Styling.Style)
            {
                StringBuilder bld = new StringBuilder();
                bld.Append("Style: Style"+s.Id);
                bld.Append(",");
                bld.Append(string.IsNullOrEmpty(s.FontFamily) ? "Trebuchet MS Bold" : s.FontFamily);
                bld.Append(",");
                bld.Append(GetFontSize(s));
                bld.Append(",");
                bld.Append(GetAssColor(s.Color));
                bld.Append(",&H000000FF,");
                int defaultoutline = 2;
                int defaultbold = 0;
                int defaultitalic = 0;
                int defaultunderline = 0;
                int defaultstrikethrough = 0;
                if (string.IsNullOrEmpty(s.TextOutline))
                    bld.Append("&H00020713");
                else
                {
                    string[] nn = s.TextOutline.Split(' ');
                    if (nn.Length > 1)
                    {
                        string k= Regex.Replace(nn[1], @"[^\d]", "");
                        int.TryParse(k, out defaultoutline);
                    }
                    bld.Append(GetAssColor(nn[0], "&H00000000"));
                }
                bld.Append(",");
                bld.Append(GetAssColor(s.BackgroundColor, "&H00000000"));
                bld.Append(",");
                if (s.FontWeight != null)
                {
                    if (s.FontWeight.ToLower().Contains("bold"))
                        defaultbold = -1;
                }
                bld.Append(defaultbold);
                bld.Append(",");
                if (s.FontStyle != null)
                {
                    if (!s.FontStyle.ToLower().Contains("normal") && !s.FontStyle.Contains("inherit"))
                        defaultitalic = -1;
                }
                bld.Append(defaultitalic);
                bld.Append(",");
                if (s.TextDecoration != null)
                {
                    if (s.TextDecoration.ToLower().Contains("underline"))
                        defaultunderline = -1;
                    else if (s.TextDecoration.ToLower().Contains("lineThrough"))
                        defaultstrikethrough = -1;
                }
                bld.Append(defaultunderline);
                bld.Append(",");
                bld.Append(defaultstrikethrough);
                bld.Append(",100,100,0,0,1,");
                bld.Append(defaultoutline);
                bld.Append(",0,");
                int defaultalign = 2;
                if (s.TextAlign != null)
                {
                    switch (s.TextAlign.ToLower())
                    {
                        case "left":
                        case "start":
                            defaultalign = 1;
                            break;
                        case "center":
                            defaultalign = 2;
                            break;
                        case "end":
                        case "right":
                            defaultalign = 3;
                            break;
                    }
                }
                bld.Append(defaultalign);
                bld.Append(",20,20,18,1");
                ret.Add(s.Id, bld.ToString());
            }
            return ret;
        }

        private string ParagraphToAss(Dictionary<string, string> styles, P p)
        {
            StringBuilder bld=new StringBuilder();
            string style = "1";
            if (p.Style != null)
            {
                if (styles.ContainsKey(p.Style))
                    style = p.Style;
            }
            bld.Append("Dialogue: 0,");
            bld.Append(p.Begin.Substring(1,p.Begin.Length-2));
            bld.Append(",");
            bld.Append(p.End.Substring(1,p.End.Length-2));
            bld.Append(",Style");
            bld.Append(style);
            bld.Append(",,0,0,0,,");
            bld.Append(string.Join("", ProcessParagraph(styles, style, p.Text.Trim())));
            return bld.ToString();
        }


        private List<string> ProcessParagraph(Dictionary<string,string> styles, string style, string p)
        {
            List<string> dta=new List<string>();
            if (string.IsNullOrEmpty(p))
                return dta;
            Match sp = SpanRegex.Match(p);
            if (sp.Success)
            {
                string span = sp.Groups["span"].Value;
                string data = sp.Groups["data"].Value;
                Match ss = StyleRegex.Match(span);
                if ((ss.Success) && (styles.ContainsKey(ss.Groups["style"].Value)))
                {
                    string nstyle = ss.Groups["style"].Value;
                    dta.AddRange(ProcessParagraph(styles, style, p.Substring(0,sp.Index)));
                    dta.Add("{\\r" + nstyle + "}");
                    dta.AddRange(ProcessParagraph(styles,nstyle,data));
                    dta.Add("{\\r" + style + "}");
                    dta.AddRange(ProcessParagraph(styles,style,p.Substring(sp.Index+sp.Length)));
                }
                else
                {
                    dta.AddRange(ProcessParagraph(styles, style, p.Substring(0, sp.Index)));
                    dta.AddRange(ProcessParagraph(styles, style, data));
                    dta.AddRange(ProcessParagraph(styles, style, p.Substring(sp.Index + sp.Length)));
                }
                return dta;
            }
            Match bld = (new Regex(string.Format(MatchRegex, "b"), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Match(p);
            if (bld.Success)
            {
                string data = bld.Groups["data"].Value;
                dta.AddRange(ProcessParagraph(styles,style,p.Substring(0,bld.Index)));
                dta.Add("{\\b1}");
                dta.AddRange(ProcessParagraph(styles, style, data));
                dta.Add("{\\b0}");
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(bld.Index + bld.Length)));
                return dta;
            }
            bld = (new Regex(string.Format(MatchRegex, "i"), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Match(p);
            if (bld.Success)
            {
                string data = bld.Groups["data"].Value;
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(0, bld.Index)));
                dta.Add("{\\i1}");
                dta.AddRange(ProcessParagraph(styles, style, data));
                dta.Add("{\\i0}");
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(bld.Index + bld.Length)));
                return dta;
            }
            bld = (new Regex(string.Format(MatchRegex, "u"), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Match(p);
            if (bld.Success)
            {
                string data = bld.Groups["data"].Value;
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(0, bld.Index)));
                dta.Add("{\\u1}");
                dta.AddRange(ProcessParagraph(styles, style, data));
                dta.Add("{\\u0}");
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(bld.Index + bld.Length)));
                return dta;
            }
            bld = (new Regex(string.Format(MatchRegex, "del"), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Match(p);
            if (!bld.Success)
                bld = (new Regex(string.Format(MatchRegex, "s"), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Match(p);
            if (!bld.Success)
                bld = (new Regex(string.Format(MatchRegex, "strike"), RegexOptions.Singleline | RegexOptions.IgnoreCase)).Match(p);
            if (bld.Success)
            {
                string data = bld.Groups["data"].Value;
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(0, bld.Index)));
                dta.Add("{\\s1}");
                dta.AddRange(ProcessParagraph(styles, style, data));
                dta.Add("{\\s0}");
                dta.AddRange(ProcessParagraph(styles, style, p.Substring(sp.Index + sp.Length)));
                return dta;
            }
            p = p.Replace("\r", " ").Replace("\n", " ").Replace("\t"," ");
            p = p.Replace("  ", " ");
            p = p.Replace("  ", " ");
            p = BrRegex.Replace(p, "\\N");
            p = p.Replace("\\N ", "\\N");
            p = p.Replace(" \\N", "\\N");
            p = RemoveTags.Replace(p, string.Empty);
            p = WebUtility.HtmlDecode(p);
            p = p.Replace("{", string.Empty).Replace("}", string.Empty);
            dta.Add(p);
            return dta;
        }
        private int GetFontSize(Style s)
        {
            if (string.IsNullOrEmpty(s.FontSize))
                return 26;
            int o;
            if (int.TryParse(s.FontSize, out o))
                return o;
            return 26;
        }

        private string GetAssColor(string color, string def= "&H00FFFFFF")
        {
            if (string.IsNullOrEmpty(color))
                return def;
            KnownColor c;
            string ret;
            if (knowcolors.ContainsKey(color))
                return knowcolors[color];
            if (color.StartsWith("#"))
            {
                ret = "&H";
                color = color.Substring(1);
                if (color.Length > 6)
                {
                    ret += color.Substring(0, 2);
                    color = color.Substring(2);
                }
                else
                    ret += "00";
                ret += color.Substring(4, 2);
                ret += color.Substring(2, 2);
                ret += color.Substring(0, 2);
                return ret;
            }
            int i;
            if (int.TryParse(color, out i))
            {
                ret = "&H" + i.ToString("X8");
            }
            return def;
        }
    }
    
    [XmlRoot(ElementName = "style", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class Style
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "textOutline", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string TextOutline { get; set; }
        [XmlAttribute(AttributeName = "color", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string Color { get; set; }
        [XmlAttribute(AttributeName = "backgroundColor", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string BackgroundColor { get; set; }
        [XmlAttribute(AttributeName = "textAlign", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string TextAlign { get; set; }
        [XmlAttribute(AttributeName = "fontSize", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string FontSize { get; set; }
        [XmlAttribute(AttributeName = "fontWeight", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string FontWeight { get; set; }
        [XmlAttribute(AttributeName = "wrapOption", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string WrapOption { get; set; }
        [XmlAttribute(AttributeName = "fontFamily", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string FontFamily { get; set; }
        [XmlAttribute(AttributeName = "fontStyle", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string FontStyle { get; set; }
        [XmlAttribute(AttributeName = "textDecoration", Namespace = "http://www.w3.org/2006/04/ttaf1#styling")]
        public string TextDecoration { get; set; }
    }


    [XmlRoot(ElementName = "styling", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class Styling
    {
        [XmlElement(ElementName = "style", Namespace = "http://www.w3.org/2006/04/ttaf1")]
        public List<Style> Style { get; set; }
    }

    [XmlRoot(ElementName = "head", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class Head
    {
        [XmlElement(ElementName = "styling", Namespace = "http://www.w3.org/2006/04/ttaf1")]
        public Styling Styling { get; set; }
    }

    [XmlRoot(ElementName = "p", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class P
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
        [XmlAttribute(AttributeName = "end")]
        public string End { get; set; }
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        //[XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "div", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class Div
    {
        [XmlElement(ElementName = "p", Namespace = "http://www.w3.org/2006/04/ttaf1")]
        public List<P> P { get; set; }
        [XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang { get; set; }
    }

    [XmlRoot(ElementName = "body", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class Body
    {
        [XmlElement(ElementName = "div", Namespace = "http://www.w3.org/2006/04/ttaf1")]
        public List<Div> Div { get; set; }
    }

    [XmlRoot(ElementName = "tt", Namespace = "http://www.w3.org/2006/04/ttaf1")]
    public class Tt
    {
        [XmlElement(ElementName = "head", Namespace = "http://www.w3.org/2006/04/ttaf1")]
        public Head Head { get; set; }
        [XmlElement(ElementName = "body", Namespace = "http://www.w3.org/2006/04/ttaf1")]
        public Body Body { get; set; }
    }

}
