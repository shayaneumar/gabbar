/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Timers;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    public class PseudoMessageProcessingClient : IMessageProcessingClient
    {
        readonly Timer _hearBeatTimer = new Timer(5000);
        public PseudoMessageProcessingClient()
        {
            _hearBeatTimer.Elapsed +=
                (sender, args) => OnUpdateReceived(new ChannelUpdateEventArgs("heartbeatChannelPushed", DateTimeOffset.UtcNow));
            _hearBeatTimer.AutoReset = true;
            _hearBeatTimer.Start();
        }

        public event EventHandler<ChannelUpdateEventArgs> UpdateReceived;

        public void OnUpdateReceived(ChannelUpdateEventArgs e)
        {
            EventHandler<ChannelUpdateEventArgs> handler = UpdateReceived;
            if (handler != null) handler(this, e);
        }

        public void EnsureConnected()
        {
        }

        public void Dispose()
        {
            _hearBeatTimer.Dispose();
        }

        public void Update(Alarm alarm)
        {
            OnUpdateReceived(new AlarmReceivedEventArgs(alarm));
        }

        public void CaseUpdate(Case updatedCase)
        {
            OnUpdateReceived(new CaseReceivedEventArgs(updatedCase));
        }
    }
}
