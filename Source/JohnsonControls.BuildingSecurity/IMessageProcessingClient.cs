/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    public interface IMessageProcessingClient : IDisposable
    {
        event EventHandler<ChannelUpdateEventArgs> UpdateReceived;
        void EnsureConnected();
    }
}
