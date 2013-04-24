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
    public class ResponseData
    {
        [DataMember]
        public string Id { get; set; }
        public Guid NormalizedId { get { return IdHelpers.GetGuid(Id); } }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string AlarmStatus { get; set; }

        [DataMember]
        public string AlarmState { get; set; }

        [DataMember]
        public string Response { get; set; }
    }
}