/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity
{
    public class AlarmReceivedEventArgs : ChannelUpdateEventArgs
    {
        public AlarmReceivedEventArgs(Alarm alarm) : base("alarmChannelPushed", alarm)
        {}
    }
}
