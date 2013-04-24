using System.Runtime.Serialization;

namespace JohnsonControls.Serialization.Xml
{
    [DataContract]
    public class DataContractTestClass
    {
        [DataMember]
        public string Value { get; set; }
    }
}