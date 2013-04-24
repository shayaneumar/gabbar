/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Diagnostics;
using System.Threading;
using System.Web.Security;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Web.Security;

namespace BuildingSecurity.Web.Security
{
    // TODO: Make sure we remove the direct dependency on Thread.CurrentPrincipal

    /// <summary>
    /// See <see cref="IAuthenticationServices"/> for further information
    /// </summary>
    public class AuthenticationServices : IAuthenticationServices
    {

        private readonly IBuildingSecuritySessionStore _sessionStore;
        private readonly IHttpSessionManager _sessionManager;
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public AuthenticationServices(IBuildingSecurityClient buildingSecurityClient, IBuildingSecuritySessionStore sessionStore, IHttpSessionManager sessionManager)
        {
            _buildingSecurityClient = buildingSecurityClient;
            _sessionStore = sessionStore;
            _sessionManager = sessionManager;
        }

        public bool IsCurrentUserLoggedOn
        {
            get
            {
                return Thread.CurrentPrincipal.IsLoggedOn(_sessionStore, _sessionManager.RetrieveSessionId());
            }
        }

        /// <summary>
        /// Signs in to the P2000.  If authentication fails, the IUser returned will have HasError set to true.
        /// </summary>
        /// <param name="userName">The users login name</param>
        /// <param name="password">The users password</param>
        /// <param name="user"> </param>
        /// <param name="errorMessage"> </param>
        /// <returns>IUser.  If the p2000 authentication failed, the IUser will have it's HasError value set to false.</returns>
        /// <remarks>
        /// If the HasError property is true, the p2000 error message will be stored in User.ErrorMessage
        /// </remarks>
        public bool TryValidateUser(string userName, string password, out IUser user, out string errorMessage)
        {
            IBuildingSecurityClientCookie cookie;
            if (_buildingSecurityClient.TrySignIn(userName, password,out cookie, out errorMessage))
            {
                user = new User(_buildingSecurityClient, cookie);
                //see if the user session exists.
                IUser existingUser;
                if (_sessionStore.TryRetrieveUser(user.Name, out existingUser))
                {
                    //if it does, log out this just created p2000 session and update the users asp.net UserSession with the new Id.
                    _buildingSecurityClient.SignOut(cookie);
                    existingUser.UserSessionId = _sessionManager.RetrieveSessionId();
                    user = existingUser;
                    return true;
                }
                //This is a new user
                _buildingSecurityClient.KeepAlive(cookie);
                _sessionStore.AddUser(user);
                user.UserSessionId = _sessionManager.RetrieveSessionId();
                return  true;
            }
            user = null;
            return false;
        }

        public void SetAuthenticationCookie(string userName, CookiePersistence cookiePersistence)
        {
            FormsAuthentication.SetAuthCookie(userName, IsCookiePersistent(cookiePersistence));
        }

        public void RemoveAuthenticationCookie()
        {
            FormsAuthentication.SignOut();
        }

        private static bool IsCookiePersistent(CookiePersistence cookiePersistence)
        {
            switch (cookiePersistence)
            {
                case CookiePersistence.SingleSession:
                    return false;

                case CookiePersistence.AcrossSessions:
                    return true;

                default:
                    // Unknown value - should this switch be updated?
                    Debugger.Break();
                    break;
            }

            // If we can't determine, default to more restrictive
            return false;
        }
    }
}
