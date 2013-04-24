/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace BuildingSecurity.Web.Api.Models
{
    /// <summary>
    /// The result returned from a call to action an alarm.
    /// </summary>
    [DataContract]
    public class ActionResult
    {
        public ActionResult(Guid id, string error)
        {
            AlarmId = id;
            Error = error ?? string.Empty;
        }

        /// <summary>
        /// The id of the alarm that this result applies to.
        /// </summary>
        [DataMember(Name = "id")]
        public Guid AlarmId { get; private set; }

        /// <summary>
        /// The error that occurred while performing an operation, otherwise, an empty string if no error occurred.
        /// </summary>
        [DataMember(Name = "error")]
        public string Error { get; private set; }
    }
}
