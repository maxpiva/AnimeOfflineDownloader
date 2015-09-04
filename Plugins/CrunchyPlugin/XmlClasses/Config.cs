using System.Collections.Generic;
using System.Xml.Serialization;

namespace CrunchyPlugin.XmlClasses
{
    [XmlRoot(ElementName = "metadata")]
    public class Metadata
    {
        [XmlElement(ElementName = "width")]
        public string Width { get; set; }

        [XmlElement(ElementName = "height")]
        public string Height { get; set; }

        [XmlElement(ElementName = "duration")]
        public string Duration { get; set; }
    }

    [XmlRoot(ElementName = "pingback")]
    public class Pingback
    {
        [XmlElement(ElementName = "hash")]
        public string Hash { get; set; }

        [XmlElement(ElementName = "time")]
        public string Time { get; set; }
    }

    [XmlRoot(ElementName = "stream_info")]
    public class StreamInfo
    {
        [XmlElement(ElementName = "host")]
        public string Host { get; set; }

        [XmlElement(ElementName = "file")]
        public string File { get; set; }

        [XmlElement(ElementName = "media_id")]
        public string Media_id { get; set; }

        [XmlElement(ElementName = "media_type")]
        public string Media_type { get; set; }

        [XmlElement(ElementName = "video_encode_id")]
        public string Video_encode_id { get; set; }

        [XmlElement(ElementName = "video_format")]
        public string Video_format { get; set; }

        [XmlElement(ElementName = "video_encode_quality")]
        public string Video_encode_quality { get; set; }

        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "token")]
        public string Token { get; set; }

        [XmlElement(ElementName = "exclusive")]
        public string Exclusive { get; set; }

        [XmlElement(ElementName = "pingback")]
        public Pingback Pingback { get; set; }

        [XmlElement(ElementName = "seekParameter")]
        public string SeekParameter { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }
    }

    [XmlRoot(ElementName = "subtitle")]
    public class Subtitle
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "link")]
        public string Link { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "user")]
        public string User { get; set; }

        [XmlAttribute(AttributeName = "default")]
        public string Default { get; set; }

        [XmlAttribute(AttributeName = "delay")]
        public string Delay { get; set; }

        [XmlElement(ElementName = "iv")]
        public string Iv { get; set; }

        [XmlElement(ElementName = "data")]
        public string Data { get; set; }
    }

    [XmlRoot(ElementName = "subtitles")]
    public class Subtitles
    {
        [XmlElement(ElementName = "media_id")]
        public string Media_id { get; set; }

        [XmlElement(ElementName = "subtitle")]
        public List<Subtitle> Subtitle { get; set; }
    }

    [XmlRoot(ElementName = "media_metadata")]
    public class MediaMetadata
    {
        [XmlElement(ElementName = "media_id")]
        public string Media_id { get; set; }

        [XmlElement(ElementName = "media_type")]
        public string Media_type { get; set; }

        [XmlElement(ElementName = "parent_media_type")]
        public string Parent_media_type { get; set; }

        [XmlElement(ElementName = "video_format")]
        public string Video_format { get; set; }

        [XmlElement(ElementName = "video_encode_quality")]
        public string Video_encode_quality { get; set; }

        [XmlElement(ElementName = "series_title")]
        public string Series_title { get; set; }

        [XmlElement(ElementName = "episode_title")]
        public string Episode_title { get; set; }

        [XmlElement(ElementName = "episode_number")]
        public string Episode_number { get; set; }

        [XmlElement(ElementName = "episode_image_url")]
        public string Episode_image_url { get; set; }

        [XmlElement(ElementName = "countdown_seconds")]
        public string Countdown_seconds { get; set; }
    }

    [XmlRoot(ElementName = "config")]
    public class Config
    {
        [XmlElement(ElementName = "stream_info")]
        public StreamInfo Stream_info { get; set; }

        [XmlElement(ElementName = "subtitles")]
        public Subtitles Subtitles { get; set; }

        [XmlElement(ElementName = "media_metadata")]
        public MediaMetadata Media_metadata { get; set; }

        [XmlElement(ElementName = "subtitle")]
        public Subtitle Subtitle { get; set; }
    }
}
