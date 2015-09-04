using System.Collections.Generic;
using System.Xml.Serialization;

namespace CrunchyPlugin.XmlClasses
{
    [XmlRoot(ElementName = "style")]
    public class Style
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "font_name")]
        public string Font_name { get; set; }
        [XmlAttribute(AttributeName = "font_size")]
        public string Font_size { get; set; }
        [XmlAttribute(AttributeName = "primary_colour")]
        public string Primary_colour { get; set; }
        [XmlAttribute(AttributeName = "secondary_colour")]
        public string Secondary_colour { get; set; }
        [XmlAttribute(AttributeName = "outline_colour")]
        public string Outline_colour { get; set; }
        [XmlAttribute(AttributeName = "back_colour")]
        public string Back_colour { get; set; }
        [XmlAttribute(AttributeName = "bold")]
        public string Bold { get; set; }
        [XmlAttribute(AttributeName = "italic")]
        public string Italic { get; set; }
        [XmlAttribute(AttributeName = "underline")]
        public string Underline { get; set; }
        [XmlAttribute(AttributeName = "strikeout")]
        public string Strikeout { get; set; }
        [XmlAttribute(AttributeName = "scale_x")]
        public string Scale_x { get; set; }
        [XmlAttribute(AttributeName = "scale_y")]
        public string Scale_y { get; set; }
        [XmlAttribute(AttributeName = "spacing")]
        public string Spacing { get; set; }
        [XmlAttribute(AttributeName = "angle")]
        public string Angle { get; set; }
        [XmlAttribute(AttributeName = "border_style")]
        public string Border_style { get; set; }
        [XmlAttribute(AttributeName = "outline")]
        public string Outline { get; set; }
        [XmlAttribute(AttributeName = "shadow")]
        public string Shadow { get; set; }
        [XmlAttribute(AttributeName = "alignment")]
        public string Alignment { get; set; }
        [XmlAttribute(AttributeName = "margin_l")]
        public string Margin_l { get; set; }
        [XmlAttribute(AttributeName = "margin_r")]
        public string Margin_r { get; set; }
        [XmlAttribute(AttributeName = "margin_v")]
        public string Margin_v { get; set; }
        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }

    }

    [XmlRoot(ElementName = "event")]
    public class Event
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "start")]
        public string Start { get; set; }
        [XmlAttribute(AttributeName = "end")]
        public string End { get; set; }
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "margin_l")]
        public string Margin_l { get; set; }
        [XmlAttribute(AttributeName = "margin_r")]
        public string Margin_r { get; set; }
        [XmlAttribute(AttributeName = "margin_v")]
        public string Margin_v { get; set; }
        [XmlAttribute(AttributeName = "effect")]
        public string Effect { get; set; }
        [XmlAttribute(AttributeName = "text")]
        public string Text { get; set; }

    }



    [XmlRoot(ElementName = "subtitle_script")]
    public class SubtitleScript
    {
        [XmlArray("styles")]
        [XmlArrayItem("style")]
        public List<Style> Styles { get; set; }
        [XmlArray("events")]
        [XmlArrayItem("event")]
        public List<Event> Events { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "play_res_x")]
        public string Play_res_x { get; set; }
        [XmlAttribute(AttributeName = "play_res_y")]
        public string Play_res_y { get; set; }
        [XmlAttribute(AttributeName = "lang_code")]
        public string Lang_code { get; set; }
        [XmlAttribute(AttributeName = "lang_string")]
        public string Lang_string { get; set; }
        [XmlAttribute(AttributeName = "created")]
        public string Created { get; set; }
        [XmlAttribute(AttributeName = "progress_string")]
        public string Progress_string { get; set; }
        [XmlAttribute(AttributeName = "status_string")]
        public string Status_string { get; set; }
        [XmlAttribute(AttributeName = "wrap_style")]
        public string Wrap_style { get; set; }


    }

}
