/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime
{
    [DataContract]
    public class CaseNoteData
    {
        [DataMember]
        public string CaseId { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}