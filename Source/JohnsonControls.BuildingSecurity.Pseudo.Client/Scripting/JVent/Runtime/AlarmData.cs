/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime
{
    [DataContract]
    public class AlarmData
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Guid? PartitionId { get; set; }

        [DataMember]
        public string PartitionName { get; set; }

        [DataMember]
        public bool? IsPublic { get; set; }

        [DataMember]
        public DateTimeOffset? MessageDateTime { get; set; }

        [DataMember]
        public string AlarmTypeDescription { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public int? Priority { get; set; }

        [DataMember]
        public bool? IsResponseRequired { get; set; }

        [DataMember]
        public bool? IsAcknowledgeRequired { get; set; }

        [DataMember]
        public int? AlarmState { get; set; }

        [DataMember]
        public string AlarmStateDescription { get; set; }

        [DataMember]
        public DateTimeOffset? StateDateTime { get; set; }

        [DataMember]
        public int? ConditionSequence { get; set; }

        [DataMember]
        public string Site { get; set; }

        [DataMember]
        public int? SourceState { get; set; }

        [DataMember]
        public string SourceStateDescription { get; set; }

        [DataMember]
        public int? Escalation { get; set; }

        [DataMember]
        public string Instructions { get; set; }

        [DataMember]
        public string IsPublicDescription { get; set; }

        [DataMember]
        public bool? IsPending { get; set; }

        [DataMember]
        public bool? IsCompletable { get; set; }

        [DataMember]
        public bool? IsRespondable { get; set; }

        [DataMember]
        public bool? IsRemovable { get; set; }

        [DataMember]
        public bool? IsCompleted { get; set; }
    }
}