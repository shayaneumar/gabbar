/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Net;
using CookComputing.XmlRpc;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Provides methods for initiating, maintaining and terminating
    /// a session with the P2000
    /// </summary>
    public interface ISessionManagementService
    {
        /// <summary>
        /// Validates whether the userName and password are valid credentials
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password in whatever representation is required by the passwordMode.</param>
        /// <param name="passwordMode"> </param>
        /// <param name="interfaceType"> </param>
        /// <returns>Returns instance of <see cref="P2000ReturnStructure"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception including the original call stack.</exception>
        [XmlRpcMethod("P2000Login")]
        P2000ReturnStructure P2000Login(string userName, string password, string passwordMode, string interfaceType);

        /// <summary>
        /// Logs off the user with the specified name and session.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionGuid">The session GUID.</param>
        /// <returns>Returns TRUE (1) if session valid, throws ServiceOperationException if fails.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception including the original call stack.</exception>
        [XmlRpcMethod("P2000Logout")]
        int P2000Logout(string userName, string sessionGuid);

        /// <summary>
        /// Tells the P2000 the specified user session is still alive. If this method is not called
        /// within 3x the session heartbeat interval returned as specified by the P2000 the the user's
        /// session will be terminated.
        /// </summary>
        /// <exception>An exception will be thrown if the user is invalid, the session is invalid/expired.</exception>
        /// <param name="userName">The name of the user who's session is being extended.</param>
        /// <param name="sessionGuid">The user's session identifier.</param>
        [XmlRpcMethod("P2000SessionHeartBeat")]
        int P2000SessionHeartBeat(string userName, string sessionGuid);
    }
}
