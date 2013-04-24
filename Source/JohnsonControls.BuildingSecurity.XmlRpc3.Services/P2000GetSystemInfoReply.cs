/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public class P2000GetSystemInfoReply
    {
        public string EnterpriseSiteGuid { get; set; }
        public string EnterpriseSiteName { get; set; }
        [XmlElement("XMLRTLPort")]
        public string XmlRtlPort { get; set; }
        [XmlElement("SessionHeartBeatInterval")]
        public string SessionHeartbeatInterval { get; set; }
        public string LocaleName { get; set; }
    }
}
