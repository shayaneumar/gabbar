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
    public class CaseData
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Status { get; set; }

        [IgnoreDataMember]
        public CaseStatus StatusEnum {
            get
            {
                CaseStatus status;
                if (Enum.TryParse(Status, ignoreCase: true, result: out status))
                {
                    return status;
                }

                return CaseStatus.Open;
            }
        }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTimeOffset CreatedDateTime { get; set; }

        [DataMember]
        public string Owner { get; set; }
    }
}
