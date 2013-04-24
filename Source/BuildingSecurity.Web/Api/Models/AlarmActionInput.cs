/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Models
{
    /// <summary>
    /// All the parameters required to perform an action on a collection of alarms.
    /// </summary>
    [DataContract]
    public class AlarmActionInput
    {
        private IEnumerable<AlarmIdSequenceTuple> _alarms;

        // This class is used for serialization only. It is not intended for use other than serializing
        // JSON or XML passed to a web method. It is intended as input only to a web api call.
        public AlarmActionInput(string action, IEnumerable<AlarmIdSequenceTuple> alarms, string response)
        {
            Action = action;
            Alarms = alarms;
            Response = response;
        }

        /// <summary>
        /// The action to perform on the alarms (Acknowledge, Respond, Complete).
        /// </summary>
        [DataMember(Name = "action")]
        public string Action { get; private set; }

        /// <summary>
        /// A collection of <see cref="AlarmIdSequenceTuple"/> that identifies the alarms to take action on.
        /// </summary>
        [DataMember(Name = "alarms")]
        public IEnumerable<AlarmIdSequenceTuple> Alarms { get { return _alarms ?? Enumerable.Empty<AlarmIdSequenceTuple>(); } private set { _alarms = value; } }

        /// <summary>
        /// The response to apply to the specified alarms, only used if action equals Respond.
        /// </summary>
        [DataMember(Name = "response")]
        public string Response { get; private set; }
    }
}
