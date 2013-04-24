/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Represents an Alarm in P2000.  Inherits from Event
    /// </summary>
    [DataContract]
    public class Alarm : Message
    {
        /// <summary>
        /// Creates an immutable Alarm object
        /// </summary>
        /// <param name="id">Event id</param>
        /// <param name="description">Description of the Alarm</param>
        /// <param name="partitionId">The Id of the partition the event occurred on</param>
        /// <param name="partitionName">The name of the partition the event occurred on</param>
        /// <param name="isPublic">True if the event is public, otherwise false</param>
        /// <param name="messageDateTime">The Date and Time the alarm occurred.</param>
        /// <param name="alarmTypeDescription">The name of the alarm type</param>
        /// <param name="category">The category of the alarm</param>
        /// <param name="priority">The alarms priority</param>
        /// <param name="isResponseRequired">True if the alarm requires a response, otherwise false</param>
        /// <param name="isAcknowledgeRequired">True if the alarm requires acknowledgement, otherwise false</param>
        /// <param name="alarmState">The ID of the AlarmState or condition</param>
        /// <param name="alarmStateDescription">The name of the AlarmState or condition</param>
        /// <param name="stateDateTime">The timestamp of the last time the AlarmState change</param>
        /// <param name="conditionSequence">incremented every time the triggering item changes state</param>
        /// <param name="site">The site object from which the alarm was generated.</param>
        /// <param name="sourceState">The ID of the source state.</param>
        /// <param name="sourceStateDescription">The source state description.</param>
        /// <param name="escalation">The escalation level of the alarm.</param>
        /// <param name="instructions">The instructions given with this alarm.</param>
        /// <param name="isPublicDescription">The text description for the IsPublic field.</param>
        /// <param name="isPending">True if the Alarm is in a Pending state, otherwise false</param>
        /// <param name="isCompletable">True if the Alarm can be Completed, otherwise false</param>
        /// <param name="isRespondable">True if one can Respond to the Alarm, otherwise false</param>
        /// <param name="isRemovable">True if the Alarm can be Removed, otherwise false</param>
        /// <param name="isCompleted">True if the Alarm can be completed, otherwise false</param>
        public Alarm(Guid id, string description, Guid partitionId, string partitionName, bool isPublic, DateTimeOffset messageDateTime,
            string alarmTypeDescription, string category, int priority, bool isResponseRequired, bool isAcknowledgeRequired,
            int alarmState, string alarmStateDescription, DateTimeOffset stateDateTime, int conditionSequence, string site,
            int sourceState, string sourceStateDescription, int escalation, string instructions, string isPublicDescription,
            bool isPending,
            bool isCompletable,
            bool isRespondable,
            bool isRemovable,
            bool isCompleted
            )
            : base(id, description, partitionId, partitionName, isPublic, messageDateTime)
        {
            AlarmState = alarmState;
            AlarmStateDescription = alarmStateDescription;
            StateDateTime = stateDateTime;
            StateDateString = StateDateTime.ToString("d", CultureInfo.CurrentCulture);
            StateTimeString = StateDateTime.ToString("t", CultureInfo.CurrentCulture);
            StateDateTimeString = StateDateTime.ToString("G", CultureInfo.CurrentCulture);
            StateDateTimeSortableString = StateDateTime.ToString("s", CultureInfo.InvariantCulture);
            ConditionSequence = conditionSequence;

            IsAcknowledgeRequired = isAcknowledgeRequired;
            IsResponseRequired = isResponseRequired;
            Priority = priority;
            Category = category;
            AlarmTypeDescription = alarmTypeDescription;
            SourceState = sourceState;
            SourceStateDescription = sourceStateDescription;
            Site = site;
            Escalation = escalation;
            Instructions = instructions;
            IsPublicDescription = isPublicDescription;

            IsPending = isPending;
            IsCompletable = isCompletable;
            IsRespondable = isRespondable;
            IsRemovable = isRemovable;
            IsCompleted = isCompleted;
        }

        /// <summary>
        /// Gets the description of the alarm type.
        /// </summary>
        /// <value>
        /// The type of the alarm. example: "Muster Zone Status"
        /// </value>
        [DataMember]
        public string AlarmTypeDescription { get; private set; }

        /// <summary>
        /// Gets the category of the alarm.
        /// </summary>
        [DataMember]
        public string Category { get; private set; }

        /// <summary>
        /// Gets the alarm priority.
        /// </summary>
        /// <value>
        /// The alarm priority. (0 -255)
        /// </value>
        [DataMember]
        public int Priority { get; private set; }

        /// <summary>
        /// True if the alarm requires response before completion, otherwise false
        /// </summary>
        public bool IsResponseRequired { get; private set; }

        /// <summary>
        /// True if the alarm requires acknowledgement, otherwise false
        /// </summary>
        public bool IsAcknowledgeRequired { get; private set; }

        /// <summary>
        /// Gets the ID of the AlarmState
        /// </summary>
        public int AlarmState { get; private set; }

        /// <summary>
        /// Gets the label of the AlarmState
        /// </summary>
        /// <value>
        /// completed, responding, acknowledged, pending
        /// </value>
        [DataMember]
        public string AlarmStateDescription { get; private set; }

        /// <summary>
        /// The timestamp of the last time the AlarmState changed, represented in the local time.
        /// </summary>
        /// <remarks>
        /// This time is in the local time of the current machine.
        /// </remarks>
        public DateTimeOffset StateDateTime { get; private set; }

        /// <summary>
        /// Gets the date portion of the <see cref="StateDateTime"/> as a short date string formatted for the current culture
        /// </summary>
        [DataMember]
        public string StateDateString { get; private set; }

        /// <summary>
        /// Gets the time portion of the <see cref="StateDateTime"/> as a short time string formatted for the current culture
        /// </summary>
        [DataMember]
        public string StateTimeString { get; private set; }

        /// <summary>
        /// Returns the <see cref="StateDateTime"/> as a general date time (long time) string formatted for the current culture
        /// </summary>
        [DataMember]
        public string StateDateTimeString { get; private set; }

        /// <summary>
        /// Returns the <see cref="StateDateTime"/> as a sortable date time string
        /// </summary>
        [DataMember]
        public string StateDateTimeSortableString { get; private set; }

        /// <summary>
        /// Incremented every time the triggering item changes state
        /// </summary>
        [DataMember]
        public int ConditionSequence { get; private set; }

        /// <summary>
        /// Gets the source state.
        /// </summary>
        [DataMember]
        public int SourceState { get; private set; }

        /// <summary>
        /// Gets the source state description.
        /// </summary>
        [DataMember]
        public string SourceStateDescription { get; private set; }

        /// <summary>
        /// Gets the site name.
        /// </summary>
        [DataMember]
        public string Site { get; private set; }

        /// <summary>
        /// Gets the escalation level.
        /// </summary>
        /// <remarks>Valid value range is 0 - 255</remarks>
        [DataMember]
        public int Escalation { get; private set; }

        /// <summary>
        /// Gets the Instruction text for alarm
        /// </summary>
        [DataMember]
        public string Instructions { get; private set; }

        /// <summary>
        /// Gets the description concerning an alarm's public status
        /// </summary>
        [DataMember]
        public string IsPublicDescription { get; private set; }

        /// <summary>
        /// True if the Alarm is in a Pending state, otherwise false
        /// </summary>
        [DataMember]
        public bool IsPending { get; private set; }

        /// <summary>
        /// True if the Alarm can be Completed, otherwise false
        /// </summary>
        [DataMember]
        public bool IsCompletable { get; private set; }

        /// <summary>
        /// True if one can Respond to the Alarm, otherwise false
        /// </summary>
        [DataMember]
        public bool IsRespondable { get; private set; }

        /// <summary>
        /// True if the Alarm can be Removed, otherwise false
        /// </summary>
        [DataMember]
        public bool IsRemovable { get; private set; }

        /// <summary>
        /// True if the Alarm can be completed, otherwise false
        /// </summary>
        [DataMember]
        public bool IsCompleted { get; private set; }


        public Alarm CloneWith(Guid? id = null,
            string description = null,
            Guid? partitionId = null,
            string partitionName = null,
            bool? isPublic = null,
            DateTimeOffset? messageDateTime = null,
            string alarmTypeDescription = null,
            string category = null,
            int? priority = null,
            bool? isResponseRequired = null,
            bool? isAcknowledgeRequired = null,
            int? alarmState = null,
            string alarmStateDescription = null,
            DateTimeOffset? stateDateTime = null,
            int? conditionSequence = null,
            string site = null,
            int? sourceState = null,
            string sourceStateDescription = null,
            int? escalation = null,
            string instructions = null,
            string isPublicDescription = null,
            bool? isPending = null,
            bool? isCompletable = null,
            bool? isRespondable = null,
            bool? isRemovable = null,
            bool? isCompleted = null
            )
        {
            return new Alarm(id:id??Id,
                description:description??Description,
                partitionId:partitionId??PartitionId,
                partitionName:partitionName??PartitionName,
                isPublic:isPublic??IsPublic,
                messageDateTime:messageDateTime??MessageDateTime,
                alarmTypeDescription:alarmTypeDescription??AlarmTypeDescription,
                category:category??Category,
                priority:priority??Priority,
                isResponseRequired:isResponseRequired??IsResponseRequired,
                isAcknowledgeRequired:isAcknowledgeRequired??IsAcknowledgeRequired,
                alarmState:alarmState??AlarmState,
                alarmStateDescription: alarmStateDescription ?? AlarmStateDescription,
                stateDateTime:stateDateTime??StateDateTime,
                conditionSequence:conditionSequence??ConditionSequence,
                site:site??Site,
                sourceState:sourceState??SourceState,
                sourceStateDescription:sourceStateDescription??SourceStateDescription,
                escalation:escalation??Escalation,
                instructions:instructions??Instructions,
                isPublicDescription:isPublicDescription??IsPublicDescription,
                isPending:isPending??IsPending,
                isCompletable:isCompletable??IsCompletable,
                isRespondable:isRespondable??IsRespondable,
                isRemovable:isRemovable??IsRemovable,
                isCompleted:isCompleted??IsCompleted);
        }
    }
}