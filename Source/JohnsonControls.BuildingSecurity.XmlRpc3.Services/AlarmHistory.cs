/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Class with detailed attributes about an Alarm when it was updated
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class AlarmHistory
    {
        /// <summary>Alarm Condition State</summary>
        public string ConditionState { get; set; }
        
        /// <summary>Alarm Condition State Description</summary>
        public string ConditionStateName { get; set; }

        /// <summary>Alarm Condition Timestamp (W3C Format)</summary>
        public string ConditionTimestamp { get; set; }

        /// <summary>Alarm Description</summary>
        public string Description { get; set; }

        ///<summary>Alarm State</summary>
        public string AlarmState { get; set; }

        /// <summary>Alarm State Description</summary>
        public string AlarmStateName { get; set; }

        /// <summary>Alarm Timestamp (W3C Format)</summary>
        public string AlarmTimestamp { get; set; }

        /// <summary>Name of the operator who actions the alarm</summary>
        public string OperatorName { get; set; }

        /// <summary>Operator’s Site Name</summary>
        public string OperatorSiteName { get; set; }

        /// <summary>Alarm Escalation Level</summary>
        public string EscalationLevel { get; set; }

        /// <summary>Alarm Response Text</summary>
        public string AlarmResponseText { get; set; }

        /// <summary>Partition Name</summary>
        public string PartitionName { get; set; }
        
        /// <summary>Indicates if the Alarm is publicly visible outside of the Partition</summary>
        [XmlElement("Public")]
        public string IsPublic { get; set; }
        
        /// <summary>Name of the Item that generated the Alarm</summary>
        public string ItemName { get; set; }
        
        /// <summary>Alarm's Category</summary>
        public string Category { get; set; }

        /// <summary>Alarm History GUID</summary>
        public string AlarmHistoryGuid { get; set; }
        
        /// <summary>Alarm Site Name</summary>
        public string AlarmSiteName { get; set; }
    }
}