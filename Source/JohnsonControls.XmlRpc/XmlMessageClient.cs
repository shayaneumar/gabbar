/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Globalization;
using JohnsonControls.Diagnostics;

namespace JohnsonControls.XmlRpc
{
    /// <summary>
    /// A TCP client that listens for RTL messages that are packaged according to the
    /// P2000 XML Protocol.
    /// </summary>
    /// <remarks>
    /// This class doesn't actually check to make sure messages are XML. It merely checks
    /// to see that they are packaged on the wire according to the XML Protocol package schema
    /// that specifies a string that contains a content type followed by a carriage return 
    /// followed by the actual message followed by a carriage return. The message is raised
    /// up to a client thru the use of the MessageReceived event.
    /// </remarks>
    public sealed class XmlMessageClient : IXmlMessageClient, IDisposable
    {
        private readonly DnsEndPoint _remoteEndpoint;

        private readonly string _username;
        private readonly string _sessionGuid;

        private Socket _socket;
        private XmlMessageReader _xmlMessageReader;

        private bool _wasStarted;
        private bool _shutDownRequested;
        private readonly object _shutDownLock = new object();

        private const string XmlRtlRouteMessage = "<?xml version=\"1.0\" encoding=\"utf-16\"?><XmlRTLRouteMessage><UserName>{0}</UserName><SessionGuid>{1}</SessionGuid><Command>1</Command></XmlRTLRouteMessage>";

        public void EnsureConnected()
        {
            lock (this)
            {
                bool connected = _socket.Connected;

                if (connected)//sometimes returns true when should be false.
                {
                    // This is how you can determine whether a socket is still connected.
                    bool blockingState = _socket.Blocking;
                    try
                    {
                        _socket.Blocking = false;
                        _socket.Send(new byte[1], 0, 0);
                    }
                    catch (SocketException e)
                    {
                        // 10035 == WSAEWOULDBLOCK == we are still connected
                        connected = e.NativeErrorCode == (10035);
                        Log.Error("{0}'s connection to real time list failed. Code={1}, Message={2}\nTrace={3}", _username, e.SocketErrorCode, e.Message, e.StackTrace);
                    }
                    finally
                    {
                        _socket.Blocking = blockingState;
                    }
                }

                if (!connected)
                {
                    Log.Warning("{0}'s connection to real time list was lost. Attempting to reconnect.", _username);
                    Restart();
                }
            }
        }

        /// <summary>
        /// Signals that a message has been received. The message will be
        /// part of the event arguments.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Creates an <see cref="XmlMessageClient"/> that listens for messages
        /// on the specified port of all network interfaces of the local machine.
        /// </summary>
        /// <param name="remoteEndpoint">DnsEndPoint of the P2000 server</param>
        /// <param name="userName">P2000 username of the user who will be initiating the RTL session</param>
        /// <param name="sessionGuid">Guid of the currently authenticated user session</param>
        public XmlMessageClient(DnsEndPoint remoteEndpoint, string userName, string sessionGuid) 
        {
            _shutDownRequested = false;

            _remoteEndpoint = remoteEndpoint;

            _username = userName;
            _sessionGuid = sessionGuid;
        }

        /// <summary>
        /// Starts the listener. This method may be invoked only one time.
        /// </summary>
        /// <remarks>
        /// This method is not thread safe. You must arrange to have only one thread ever
        /// attempt to start this instance.
        /// </remarks>
        public void Start()
        {
            if (_wasStarted) throw new InvalidOperationException("A connection has already been started.");
            Restart();
        }

        private void Restart()
        {
            _wasStarted = true;
            if(_socket !=null)_socket.Dispose();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_remoteEndpoint);
            StartXmlReaderOnSocket(_socket, _username, _sessionGuid);
        }

        /// <summary>
        /// Stops the listener and disposes of any resources.
        /// </summary>
        /// <remarks>
        /// This method is thread safe.
        /// </remarks>
        public void Stop()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            lock (_shutDownLock)
            {
                // Only shut down once
                if (!IsShutdownRequested)
                {
                    _shutDownRequested = true;
                    if (_xmlMessageReader != null)
                    {
                        _xmlMessageReader.MessageReceived -= OnMessageReceived;
                        _xmlMessageReader.Stop();
                        _xmlMessageReader = null;
                    }
                    
                    if (_socket != null)
                    {
                        if (_socket.Connected)
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                        }

                        _socket.Close();
                        _socket = null;
                    }
                }
            }
        }

        /// <summary>
        /// Indicates if the listener has been told to shutdown.
        /// </summary>
        internal bool IsShutdownRequested { get { return _shutDownRequested; } }

        #region Private Helpers
        private void Send(string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            _socket.BeginSend(byteData, 0, byteData.Length, 0, myContext => _socket.EndSend(myContext), null);
        }

        private void StartXmlReaderOnSocket(Socket socket, string username, string sessionGuid)
        {
            _xmlMessageReader = new XmlMessageReader(socket, this);
            _xmlMessageReader.MessageReceived += OnMessageReceived;
            _xmlMessageReader.Start();
            Send(string.Format(CultureInfo.InvariantCulture, XmlRtlRouteMessage, SecurityElement.Escape(username), SecurityElement.Escape(sessionGuid)));
            Log.Information("Started real time list for {0}.", _username);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> messageRecieved = MessageReceived;
            if (messageRecieved != null)
            {
                messageRecieved(this, e);
            }
        }
        #endregion
    }
}
