using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ADBaseLibrary.AdobeHDS
{
    [XmlRoot(ElementName = "bootstrapInfo", Namespace = "http://ns.adobe.com/f4m/1.0")]
    public class BootstrapInfo
    {
        [XmlAttribute(AttributeName = "profile")]
        public string Profile { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlIgnore]
        public BootStrap Info { get; set; }
    }

    [XmlRoot(ElementName = "media", Namespace = "http://ns.adobe.com/f4m/1.0")]
    public class Media
    {
        [XmlElement(ElementName = "metadata", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public string Metadata { get; set; }
        [XmlAttribute(AttributeName = "bitrate")]
        public string Bitrate { get; set; }
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
        [XmlAttribute(AttributeName = "bootstrapInfoId")]
        public string BootstrapInfoId { get; set; }
        [XmlIgnore]
        public BootstrapInfo Info { get; set; }
        [XmlIgnore]
        public Metadata MetadataInfo { get; set; }


    }

    [XmlRoot(ElementName = "manifest", Namespace = "http://ns.adobe.com/f4m/1.0")]
    public class Manifest
    {
        [XmlElement(ElementName = "version", Namespace = "uri:akamai.com/f4m/1.0")]
        public string Version { get; set; }
        [XmlElement(ElementName = "bw", Namespace = "uri:akamai.com/f4m/1.0")]
        public string Bw { get; set; }
        [XmlElement(ElementName = "id", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public string Id { get; set; }
        [XmlElement(ElementName = "streamType", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public List<string> StreamType { get; set; }
        [XmlElement(ElementName = "duration", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public string Duration { get; set; }
        [XmlElement(ElementName = "streamBaseTime", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public string StreamBaseTime { get; set; }
        [XmlElement(ElementName = "pv-2.0", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public string Pv20 { get; set; }
        [XmlElement(ElementName = "bootstrapInfo", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public List<BootstrapInfo> BootstrapInfos { get; set; }
        [XmlElement(ElementName = "media", Namespace = "http://ns.adobe.com/f4m/1.0")]
        public List<Media> Medias { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "akamai", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Akamai { get; set; }

        public void Init()
        {
            foreach (BootstrapInfo b in BootstrapInfos)
            {
                MemoryStream ms=new MemoryStream(Convert.FromBase64String(b.Text));
                BoxReader reader=new BoxReader(ms);
                b.Info=new BootStrap();
                string name;
                reader.ReadHeader(out name);
                reader.ReadBootStrap(b.Info);
                reader.Dispose();
            }
            foreach (Media m in Medias)
            {
                MemoryStream ms=new MemoryStream(Convert.FromBase64String(m.Metadata));
                AmfReader reader=new AmfReader(ms);
                m.MetadataInfo = reader.ReadObject<Metadata>();
                reader.Dispose();
                m.Info = BootstrapInfos.First(a => a.Id == m.BootstrapInfoId);
            }
        }

    }
}
