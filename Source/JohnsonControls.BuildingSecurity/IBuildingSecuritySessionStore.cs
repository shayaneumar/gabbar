/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Contains information about all the currently logged in users.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Store indicates this type contains multiple values.")]
    public interface IBuildingSecuritySessionStore : IEnumerable<IUser>
    {
        /// <summary>
        /// Returns a User with a given <paramref name="userName"/>, if it exists in the SessionStore's cache.
        /// </summary>
        /// <param name="userName">the identification of the user to retrieve</param>
        /// <exception cref="InvalidOperationException">thrown if the <paramref name="userName"/>
        ///  key cannot be found in the SessionStore's user cache
        /// </exception>
        /// <returns>The <see cref="IUser"/> object associated with <paramref name="userName"/></returns>
        IUser RetrieveUser(string userName);

        /// <summary>
        /// Returns true if a user with a given <paramref name="userName"/>, exists in the SessionStore's cache, otherwise false.
        /// </summary>
        /// <param name="userName">The user's userName.</param>
        /// <param name="user">The <see cref="IUser"/> object to populate if the user is found.</param>
        /// <returns>true if a User with a given <paramref name="userName"/>, exists in the SessionStore's cache, otherwise false.</returns>
        bool TryRetrieveUser(string userName, out IUser user);

        /// <summary>
        /// Adds a user to the SessionStore's user cache.
        /// </summary>
        /// <param name="user">The User object to store in the session cache</param>
        void AddUser(IUser user);

        /// <summary>
        /// Removes the current user from SessionStore's the cache
        /// </summary>
        void ClearUser(string userName);
    }
}
