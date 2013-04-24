/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using JohnsonControls.Serialization.Xml;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    //Class that subscribes to an event
    public sealed class MessageProcessingService : IMessageProcessingService
    {
        private enum MessageType
        {
            AlarmMessage = 3,
            HeartbeatMessage = 8
        }

        private readonly XmlSerializer<RealTimeAlarmMessage> _alarmMessageSerializer = new XmlSerializer<RealTimeAlarmMessage>();
        private readonly XmlSerializer<RealTimeHeartbeatMessage> _rtlMessageSerializer = new XmlSerializer<RealTimeHeartbeatMessage>();
        
        private readonly IXmlMessageClient _xmlMessageClient;

        public event EventHandler<AlarmMessageReceivedEventArgs> AlarmMessageReceived;
        public event EventHandler<HeartbeatMessageReceivedEventArgs> HeartbeatMessageReceived;

        public void EnsureConnected()
        {
            _xmlMessageClient.EnsureConnected();
        }

        public MessageProcessingService(IXmlMessageClient messageClient)
        {
            if (messageClient == null) throw new ArgumentNullException("messageClient");

            _xmlMessageClient = messageClient;
            
            _xmlMessageClient.MessageReceived += ProcessMessage;

            _xmlMessageClient.Start();
        }

        // Define what actions to take when the event is raised.
        void ProcessMessage(object sender, MessageReceivedEventArgs e)
        {
            int messageTypeId = GetTypeIdFromXmlString(e.Message);
            switch ((MessageType)messageTypeId)
            {
                case MessageType.AlarmMessage:
                    AlarmMessage alarmMessage = _alarmMessageSerializer.Deserialize(e.Message);
                    OnAlarmMessageReceived(new AlarmMessageReceivedEventArgs(alarmMessage));
                    break;

                case MessageType.HeartbeatMessage:
                    HeartbeatMessage heartbeatMessage = _rtlMessageSerializer.Deserialize(e.Message);
                    OnHeartbeatMessageReceived(new HeartbeatMessageReceivedEventArgs(heartbeatMessage));
                    break;
            }
        }

        private static int GetTypeIdFromXmlString(string message)
        {
            try
            {
                XDocument messageDocument = XDocument.Parse(message);

                // TODO: Upgrade this to use a more efficient process. Perhaps our performance will be good enough using this approach. I guess I'd like someone to take a fairly long XML Alarm Message and parse it one million times using this approach vs. using a XPath query.
                var p2000MessageElement = messageDocument.Element("P2000Message");
                if (p2000MessageElement != null)
                {
                    var messageBaseElement = p2000MessageElement.Element("MessageBase");
                    if (messageBaseElement != null)
                    {
                        var messageTypeElement = messageBaseElement.Element("MessageType");
                        if (messageTypeElement != null)
                            return Int32.Parse(messageTypeElement
                                                   .Value, CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (XmlException)
            {}

            return -1;
        }

        private void OnAlarmMessageReceived(AlarmMessageReceivedEventArgs e)
        {
            EventHandler<AlarmMessageReceivedEventArgs> alarmMessageRecieved = AlarmMessageReceived;
            if (alarmMessageRecieved != null)
            {
                alarmMessageRecieved(this, e);
            }
        }

        private void OnHeartbeatMessageReceived(HeartbeatMessageReceivedEventArgs e)
        {
            EventHandler<HeartbeatMessageReceivedEventArgs> heartbeatMessageReceived = HeartbeatMessageReceived;
            if (heartbeatMessageReceived != null)
            {
                heartbeatMessageReceived(this, e);
            }
        }

        public void Dispose()
        {
            _xmlMessageClient.Stop();
        }
    }
}