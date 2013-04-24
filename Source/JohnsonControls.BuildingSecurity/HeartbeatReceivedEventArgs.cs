/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    public class HeartbeatReceivedEventArgs : ChannelUpdateEventArgs 
    {
        public HeartbeatReceivedEventArgs(DateTimeOffset timestampUtc) : base("heartbeatChannelPushed", timestampUtc)
        {}
    }
}
