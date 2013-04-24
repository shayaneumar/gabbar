/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Text;
using JohnsonControls.Serialization.Xml;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public sealed class SessionManagementService : ITypedSessionManagement
    {
        private const string P2000WebUserInterfaceMode = "2";
        private const string Sha1PasswordMode = "2";

        private readonly XmlSerializer<P2000LoginReplyWrapper> _serializer = 
            new XmlSerializer<P2000LoginReplyWrapper>();

        private readonly P2000XmlRpcProxy<ISessionManagementService> _sessionManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionManagementService"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        public SessionManagementService(Uri serviceUrl)
        {
            _sessionManagementService = new P2000XmlRpcProxy<ISessionManagementService>(serviceUrl);
        }

        /// <summary>
        /// Validates whether the userName and password are valid credentials
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Returns instance of <see cref="P2000ReturnStructure"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="System.Net.WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception including the original call stack.</exception>
        public P2000LoginReply P2000Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Must not be null or whitespace.", "userName");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Must not be null or whitespace.", "password");

            var response = _sessionManagementService.Invoke(proxy => proxy.P2000Login(userName, password.Sha1(Encoding.UTF8), Sha1PasswordMode, P2000WebUserInterfaceMode));

            return _serializer.Deserialize(response.XmlDoc).P2000LoginReply;
        }

        /// <summary>
        /// Logs off the specified userName and sessionGuid.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionGuid">The session GUID.</param>
        /// <returns>
        /// Returns TRUE (1) if session valid, throws ServiceOperationException if fails.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="System.Net.WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception including the original call stack.</exception>
        public void P2000Logout(string userName, string sessionGuid)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Must not be null or whitespace.", "userName");
            if (string.IsNullOrWhiteSpace(sessionGuid)) throw new ArgumentException("Must not be null or whitespace.", "sessionGuid");

            _sessionManagementService.Invoke(proxy => proxy.P2000Logout(userName, sessionGuid));
        }

        /// <summary>
        /// Sends a heart beat message to the P2000 which is required to maintain an open session
        /// </summary>
        public void P2000SessionHeartbeat(string userName, string sessionGuid)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Must not be null or whitespace.", "userName");
            if (string.IsNullOrWhiteSpace(sessionGuid)) throw new ArgumentException("Must not be null or whitespace.", "sessionGuid");

            _sessionManagementService.Invoke(proxy => proxy.P2000SessionHeartBeat(userName, sessionGuid));
        }
    }
}
