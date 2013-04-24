/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;

namespace JohnsonControls.BuildingSecurity
{
    public class ChannelUpdateEventArgs : EventArgs
    {
        public string ChannelName { get; private set; }
        public object Update { get; private set; }

        public ChannelUpdateEventArgs(string channelName, object update)
        {
            ChannelName = channelName;
            Update = update;
        }
    }
}
