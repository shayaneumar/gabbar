/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Net;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    // TODO: Unit Tests for this class
    public sealed class MessageProcessingClient : IMessageProcessingClient
    {
        public event EventHandler<ChannelUpdateEventArgs> UpdateReceived;



        public void EnsureConnected()
        {
            _messageProcessingService.EnsureConnected();
        }

        private readonly IMessageProcessingService _messageProcessingService;
        private readonly Func<TimeZoneInfo> _timeZone;
        private readonly CultureInfo _culture;

        public MessageProcessingClient(IMessageProcessingService messageProcessingService, Func<TimeZoneInfo> timeZone, CultureInfo culture)
        {
            if (messageProcessingService == null) throw new ArgumentNullException("messageProcessingService");

            _messageProcessingService = messageProcessingService;
            _timeZone = timeZone;
            _culture = culture;
            _messageProcessingService.AlarmMessageReceived += ProcessAlarmMessage;
            _messageProcessingService.HeartbeatMessageReceived += ProcessHeartbeatMessage;
        }

        // Define what actions to take when the event is raised.
        private void ProcessAlarmMessage(object sender, AlarmMessageReceivedEventArgs e)
        {
            Alarm alarm = e.AlarmMessage.ConvertToAlarm(_timeZone(), _culture);

            OnUpdateReceived(new AlarmReceivedEventArgs(alarm));
        }

        // Define what actions to take when the event is raised.
        private void ProcessHeartbeatMessage(object sender, HeartbeatMessageReceivedEventArgs e)
        {
            DateTimeOffset timestampUtc = DateTimeOffset.Parse(e.HeartbeatMessage.MessageBase.TimestampUtc,
                                                               CultureInfo.InvariantCulture);

            OnUpdateReceived(new HeartbeatReceivedEventArgs(timestampUtc));
        }

        public void OnUpdateReceived(ChannelUpdateEventArgs e)
        {
            EventHandler<ChannelUpdateEventArgs> handler = UpdateReceived;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Factory method that returns the message processing client.
        /// </summary>
        /// <param name="remoteEndpoint">EndPoint (address and port) of the Real-Time Service</param>
        /// <param name="userName">UserName of the user connecting to the Real-Time Service</param>
        /// <param name="sessionGuid">GUID of the User's current session</param>
        /// <param name="timeZone">TimeZone of the User initiating the current session</param>
        /// <param name="culture">Culture of the User initiating the current session</param>
        /// <returns>MessageProcessingClient based on the specified parameters</returns>
        public static IMessageProcessingClient GetMessageProcessingClient(DnsEndPoint remoteEndpoint, string userName, string sessionGuid, Func<TimeZoneInfo> timeZone, CultureInfo culture)
        {
            if (remoteEndpoint == null) throw new ArgumentNullException("remoteEndpoint");
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(sessionGuid)) throw new ArgumentNullException("sessionGuid");

            // TODO: Fix Warnings
            var xmlMessageClient = new XmlMessageClient(remoteEndpoint, userName, sessionGuid);
            var messageProcessingService = new MessageProcessingService(xmlMessageClient);
            var messageProcessingClient = new MessageProcessingClient(messageProcessingService, timeZone, culture);

            return messageProcessingClient;
        }

        public void Dispose()
        {
            _messageProcessingService.Dispose();
        }
    }
}
