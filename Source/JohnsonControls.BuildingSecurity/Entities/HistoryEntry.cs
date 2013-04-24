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
    /// An entry in the history of an alarm.
    /// </summary>
    [DataContract(Name = "historyEntry")]
    public class HistoryEntry
    {
        public HistoryEntry(string id, DateTimeOffset timestamp, string operatorName, string alarmStatus, string alarmState, string response)
        {
            Id = id;
            Timestamp = timestamp.ToString("G", CultureInfo.CurrentCulture);
            SortableTimestamp = timestamp.ToString("s", CultureInfo.InvariantCulture);
            OperatorName = operatorName;
            AlarmStatus = alarmStatus;
            AlarmState = alarmState;
            Response = response;
        }

        /// <summary>
        /// The id of the action entry
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// The timestamp of the action.
        /// </summary>
        [DataMember(Name = "timestamp")]
        public string Timestamp { get; private set; }

        /// <summary>
        /// The timestamp of the action in a sortable format.
        /// </summary>
        [DataMember(Name = "sortableTimestamp")]
        public string SortableTimestamp { get; private set; }

        /// <summary>
        /// The operator who took the action.
        /// </summary>
        [DataMember(Name = "operatorName")]
        public string OperatorName { get; private set; }

        /// <summary>
        /// The status of the parent alarm after this action.
        /// </summary>
        [DataMember(Name = "alarmStatus")]
        public string AlarmStatus { get; private set; }

        /// <summary>
        /// The state of the parent alarm after this action.
        /// </summary>
        [DataMember(Name = "alarmState")]
        public string AlarmState { get; private set; }

        /// <summary>
        /// The response that the operator entered as part of a respond action.
        /// Other actions are not expected to have a response.
        /// </summary>
        [DataMember(Name = "response")]
        public string Response { get; private set; }
    }
}
