/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Extends the <see cref="ISessionManagementService"/> with strongly typed versions of the
    /// 3.12 services.
    /// </summary>
    /// <remarks>
    /// The pattern for the additional services defined for 3.12 was to include 
    /// an input parameter of type string and a return value of type string. This
    /// string is expected to be an XML document of the schema specified in the 3.12
    /// RPC documentation. This API is not very useful for a developer. This interface
    /// enhances the API by providing strongly typed versions of the services and handles
    /// all of the XML serialization and deserialization.
    /// </remarks>
    public interface ITypedSessionManagement 
    {
        /// <summary>
        /// Logs in the user with the specified user name and password
        /// </summary>
        /// <param name="userName">The name of the user who is logging in</param>
        /// <param name="password">The plain text version of the password</param>
        /// <returns></returns>
        P2000LoginReply P2000Login(string userName, string password);

        /// <summary>
        /// Logs out the user with the specified user name and session
        /// </summary>
        /// <param name="userName">The name of the user who is logging out</param>
        /// <param name="sessionGuid">The user's session identifier.</param>
        /// <exception cref="ServiceOperationException">if the call fails for any reason,
        /// including invalid user name, invalid session identifier, etc.</exception>
        void P2000Logout(string userName, string sessionGuid);

        /// <summary>
        /// Tells the P2000 the specified user session is still alive. If this method is not called
        /// within 3x the session heartbeat interval returned as specified by the P2000 the the user's
        /// session will be terminated.
        /// </summary>
        /// <exception>An exception will be thrown if the user is invalid, the session is invalid/expired.</exception>
        /// <param name="userName">The name of the user who's session is being extended.</param>
        /// <param name="sessionGuid">The user's session identifier.</param>
        void P2000SessionHeartbeat(string userName, string sessionGuid);
    }
}
