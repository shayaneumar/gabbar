/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Class contains each of the attributes specific to an Alarm Message (both on demand list and incoming Real-Time).
    /// </summary>
    [XmlRoot("MessageDetails")]
    public class AlarmMessageDetails
    {
        public string MessageVersion { get; set; }
        public string AlarmGuid { get; set; }
        [XmlElement("AlarmID")]
        public string AlarmId { get; set; }
        public string AlarmType { get; set; }
        public string AlarmOptionsGuid { get; set; }
        public string AlarmTypeName { get; set; }
        [XmlElement("AlarmTypeID")]
        public string AlarmTypeId { get; set; }
        public string AlarmTypeGuid { get; set; }
        [XmlElement("AckRequired")]
        public string AcknowledgeRequired { get; set; }
        public string ResponseRequired { get; set; }
        public string InstructionText { get; set; }
        public string AlarmState { get; set; }
        public string AlarmStateName { get; set; }
        public string AlarmTimestamp { get; set; }
        public string ConditionState { get; set; }
        public string ConditionStateName { get; set; }
        public string ConditionSequenceNumber { get; set; }
        public string ConditionSequence { get; set; }
        public string ConditionCompletionState { get; set; }
        public string ConditionCompletionStateName { get; set; }
        public string ConditionTimestamp { get; set; }
        [XmlElement("ConditionTimestampUTC")]
        public string ConditionTimestampUtc { get; set; }
        public string Popup { get; set; }
        public string Description { get; set; }
        public string AlarmSiteName { get; set; }
        public string AlarmResponseText { get; set; }
        public string CanAcknowledge { get; set; }
        public string CanRespond { get; set; }
        public string CanComplete { get; set; }
    }
}