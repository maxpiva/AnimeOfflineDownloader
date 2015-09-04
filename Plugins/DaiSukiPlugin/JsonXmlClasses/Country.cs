using System.Xml.Serialization;

namespace DaiSukiPlugin.JsonXmlClasses
{
    [XmlRoot(ElementName = "Content_Targeting")]
    public class Country
    {
        [XmlElement(ElementName = "country_code")]
        public string CountryCode { get; set; }
    }
}
