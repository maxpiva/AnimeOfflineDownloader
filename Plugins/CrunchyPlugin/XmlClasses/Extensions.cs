using System;
using System.Text;

namespace CrunchyPlugin.XmlClasses
{
    public static class Extensions
    {
        public static string ToAss(this Event e)
        {
            return "Dialogue: 0," + e.Start + "," + e.End + "," + e.Style + "," + e.Name + "," + e.Margin_l + "," +
                   e.Margin_r + "," + e.Margin_v + "," + e.Effect + "," + e.Text;
        }

        public static string ToAss(this Style s)
        {
            return "Style: " + s.Name + "," + s.Font_name + "," + s.Font_size + "," + s.Primary_colour + "," +
                   s.Secondary_colour + "," + s.Outline_colour + "," + s.Back_colour + "," + s.Bold + "," + s.Italic +
                   "," + s.Underline + "," + s.Strikeout + "," + s.Scale_x + "," + s.Scale_y + "," + s.Spacing + "," +
                   s.Angle + "," + s.Border_style + "," + s.Outline + "," + s.Shadow + "," + s.Alignment + "," +
                   s.Margin_l + "," + s.Margin_r + "," + s.Margin_v + "," + s.Encoding;
        }

        public static string ToAss(this SubtitleScript ss)
        {
            StringBuilder bld=new StringBuilder();
            int width = 640;
            int height = 360;
            if (!string.IsNullOrEmpty(ss.Play_res_x))
                width = int.Parse(ss.Play_res_x);
            if (!string.IsNullOrEmpty(ss.Play_res_y))
                height = Int32.Parse(ss.Play_res_y);
            bld.AppendLine("[Script Info]");
            bld.AppendLine("Title: " + ss.Title);
            bld.AppendLine("ScriptType: v4.00+");
            bld.AppendLine("WrapStyle: " + ss.Wrap_style);
            bld.AppendLine("PlayResX: " + width);
            bld.AppendLine("PlayResY: " + height);
            bld.AppendLine("");
            bld.AppendLine("[V4+ Styles]");
            bld.AppendLine("Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding");
            foreach (Style s in ss.Styles)
                bld.AppendLine(s.ToAss());
            bld.AppendLine("");
            bld.AppendLine("[Events]");
            bld.AppendLine("Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text");
            foreach (Event e in ss.Events)
                bld.AppendLine(e.ToAss());
            return bld.ToString();
        }

    }
}
