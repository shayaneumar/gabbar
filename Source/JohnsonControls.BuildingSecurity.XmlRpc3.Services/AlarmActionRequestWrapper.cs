/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [XmlRoot("P2000Request")]
    public class AlarmActionRequestWrapper
    {
        public AlarmActionRequest AlarmActionRequest { get; set; }
    }
}
