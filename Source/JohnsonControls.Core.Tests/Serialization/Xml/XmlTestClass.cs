using System.Xml.Serialization;

namespace JohnsonControls.Serialization.Xml
{
    [XmlRoot(ElementName = "string")]
    public class XmlTestClass
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}