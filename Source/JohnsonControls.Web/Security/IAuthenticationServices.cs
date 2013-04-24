/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using JohnsonControls.BuildingSecurity;

namespace JohnsonControls.Web.Security
{
    /// <summary>
    /// Manages authentication services for Web applications.
    /// </summary>
    public interface IAuthenticationServices
    {
        /// <summary>
        /// Gets a value that indicates whether the current user has been authenticated and is logged on to the building security system.
        /// </summary>
        bool IsCurrentUserLoggedOn { get; }


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
        bool TryValidateUser(string userName, string password, out IUser user, out string errorMessage);

        /// <summary>
        /// Creates an authentication ticket for the supplied user name and adds it to the cookies collection of the response, or to the URL if you are using cookieless authentication.
        /// </summary>
        /// <param name="userName">The name of an authenticated user.</param>
        /// <param name="cookiePersistence">The persistence of the cookie. See <see cref="CookiePersistence"/></param>
        void SetAuthenticationCookie(string userName, CookiePersistence cookiePersistence);

        /// <summary>
        /// Removes the authentication ticket from the browser.
        /// </summary>
        void RemoveAuthenticationCookie();
    }
}
