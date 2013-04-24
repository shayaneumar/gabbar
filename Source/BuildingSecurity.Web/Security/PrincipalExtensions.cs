/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Security.Principal;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Security
{
    /// <summary>
    /// Extension methods for <see cref="IPrincipal"/>
    /// </summary>
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Determines if the principal is logged in to the system.
        /// </summary>
        /// <param name="principal">The <see cref="IPrincipal"/> to check the logged in status for.</param>
        /// <param name="sessionStore">The session store that holds information for the logged in users.</param>
        /// <param name="sessionId">The user's current Http sessionId </param>
        /// <returns>True if the <see cref="IPrincipal"/> is logged on to the system, otherwise false.</returns>
        public static bool IsLoggedOn(this IPrincipal principal, IBuildingSecuritySessionStore sessionStore, string sessionId)
        {
            if (principal  != null && sessionStore != null && principal.Identity.IsAuthenticated)
            {
                IUser user;
                return  sessionStore.TryRetrieveUser(principal.Identity.Name, out user) &&
                       user.UserSessionId == sessionId;
            }

            return false;
        }

    }
}
