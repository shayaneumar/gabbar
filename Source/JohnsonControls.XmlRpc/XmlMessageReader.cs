/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Net.Sockets;
using System.Text;
using JohnsonControls.Diagnostics;

namespace JohnsonControls.XmlRpc
{
    /// <summary>
    /// Encapsulates one socket for receiving messages from a server sending messages
    /// in the P2000 XML Protocol schema.
    /// </summary>
    internal sealed class XmlMessageReader
    {
        private const int BufferSize = 1024;
        private bool _stopped = true;

        private readonly byte[] _readBuffer;
        private readonly XmlMessageClient _client;

        private readonly Socket _socket;
        private MessageStage _currentStage;

        private readonly StringBuilder _unproccessedData;

        /// <summary>
        /// Signals that a message has been received. The message will be
        /// part of the event arguments.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Creates an instance of <see cref="XmlMessageReader"/> using the 
        /// specified socked.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="client">This should be the client that created the reader.</param>
        public XmlMessageReader(Socket socket, XmlMessageClient client)
        {
            if (socket == null) throw new ArgumentNullException("socket");

            _unproccessedData = new StringBuilder();
            _socket = socket;
            _socket.ReceiveTimeout = 1000;
            _readBuffer = new byte[BufferSize];
            _currentStage = MessageStage.ContentType;
            _client = client;
        }

        /// <summary>
        /// Starts reading messages.
        /// </summary>
        public void Start()
        {
            _stopped = false;
            ReadMessagesAsync();
        }

        private void ReadMessagesAsync()
        {
            if (_stopped) return; //Don't bother if we have been stopped
            try
            {
                if (IsShutdownRequested)
                {
                    Stop();
                }
                else
                {
                    _socket.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, OnDataRecieved, null);
                }
            }
            catch (ObjectDisposedException e)
            {
                Log.Error("An ObjectDisposedException occurred while reading from the real time list. Connection will be terminated. Exception={0}, Trace={1}", e.Message, e.StackTrace);
                Stop();
            }
        }

        private void OnDataRecieved(IAsyncResult ar)
        {
            if (_stopped) return; //Don't bother if we have been stopped
            try
            {
                SocketError socketError;
                int numBytesRead = _socket.EndReceive(ar, out socketError);
                if (numBytesRead == 0)
                {
                    Stop();
                }
                else
                {
                    string data = Encoding.ASCII.GetString(_readBuffer, 0, numBytesRead);
                    _unproccessedData.Append(data);
                    ProcessData();
                    ReadMessagesAsync();
                }
            }
            catch (SocketException e)
            {
                // Probably a receive timeout
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    ReadMessagesAsync();
                }
                else
                {
                    Log.Error("A SocketException occurred while reading from the real time list. Connection will be terminated. Exception={0}, Trace={1}", e.Message, e.StackTrace);
                    Stop();
                }
            }
            catch (ObjectDisposedException e)
            {
                Log.Error("An ObjectDisposedException occurred while reading from the real time list. Connection will be terminated. Exception={0}, Trace={1}", e.Message, e.StackTrace);
                Stop();
            }
        }


        public  void Stop()
        {
            _stopped = true ;
        }

        private bool IsShutdownRequested { get { return _client.IsShutdownRequested; } }

        private void ProcessData()
        {
            if (_unproccessedData.Length > 0)
            {
                switch (_currentStage)
                {
                    case MessageStage.ContentType:
                        ProcessContentType();
                        break;

                    case MessageStage.Body:
                        ProcessMessageBody();
                        break;
                }
            }
        }

        private void ProcessContentType()
        {
            int index = _unproccessedData.ToString().IndexOf("\r\n", StringComparison.Ordinal);
            if (index != -1)
            {
                _currentStage = MessageStage.Body;

                _unproccessedData.Remove(0,index+2);//Remove the header
                ProcessData();
            }
        }

        private void ProcessMessageBody()
        {
            var data = _unproccessedData.ToString();
            int index = data.IndexOf("\r\n", StringComparison.Ordinal);
            if (index != -1)
            {
                OnMessageReceived(_unproccessedData.ToString(0, index));

                _currentStage = MessageStage.ContentType;

                _unproccessedData.Remove(0, index + 2);
                ProcessData();
            }
        }

        private void OnMessageReceived(string message)
        {
            EventHandler<MessageReceivedEventArgs> messageRecieved = MessageReceived;
            if (messageRecieved != null)
            {
                messageRecieved(this, new MessageReceivedEventArgs(message.Trim()));
            }
        }
    }
}