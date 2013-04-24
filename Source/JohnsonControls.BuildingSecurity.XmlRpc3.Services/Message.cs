/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Base class of all incoming Messages (Alarms, Heartbeats, etc.)
    /// </summary>
    public class Message
    {
        public MessageBase MessageBase { get; set; }
        public MessageDecode MessageDecode { get; set; }
    }
}