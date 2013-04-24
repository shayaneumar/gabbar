/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Wrapper for HeartbeatMessage, used for incoming Heartbeats.
    /// </summary>
    [XmlRoot("P2000Message")]
    public class RealTimeHeartbeatMessage: HeartbeatMessage
    {
    }
}