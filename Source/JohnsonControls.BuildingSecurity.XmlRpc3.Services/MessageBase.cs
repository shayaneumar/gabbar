/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Class contained within a Message, for common attributes that apply to various types of messages
    /// </summary>
    public class MessageBase
    {
        public string BaseVersion { get; set; }
        public string MessageType { get; set; }
        [XmlElement("MessageSubType")]
        public string MessageSubtype { get; set; }
        public string SiteName { get; set; }
        public string PartitionName { get; set; }
        [XmlElement("Public")]
        public string IsPublic { get; set; }
        public string ItemName { get; set; }
        public string QueryString { get; set; }
        public string Category { get; set; }
        public string Escalation { get; set; }
        public string Priority { get; set; }
        public string OperatorName { get; set; }
        public string HistoryFilterKey { get; set; }
        public string HistoryFilterName { get; set; }
        [XmlElement("TimeStampUTC")]
        public string TimestampUtc { get; set; }
    }
}