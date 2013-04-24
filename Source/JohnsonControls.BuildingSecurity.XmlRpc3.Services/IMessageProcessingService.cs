/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public interface IMessageProcessingService : IDisposable
    {
        event EventHandler<AlarmMessageReceivedEventArgs> AlarmMessageReceived;
        event EventHandler<HeartbeatMessageReceivedEventArgs> HeartbeatMessageReceived;
        void EnsureConnected();
    }
}