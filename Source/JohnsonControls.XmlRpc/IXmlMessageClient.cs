/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.XmlRpc
{
    public interface IXmlMessageClient
    {
        void Start();
        void Stop();
        void EnsureConnected();
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}