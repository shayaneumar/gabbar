/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Globalization;

namespace JohnsonControls.BuildingSecurity
{
    //TODO: This looks like it has become a combination of a DTO and a service
    // I think this should be restructured something like:
    // * An IUser that just has data
    // * An IUserSession that has an IUser property plus the service methods
    public interface IUser
    {
        /// <summary>
        /// The User's P2000 Identity.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The user's full name
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// The Id of the User's Partition
        /// </summary>
        string PartitionId { get; }

        /// <summary>
        /// Gets or sets the user preferences.  (TimeZone Information)
        /// </summary>
        /// <value>
        /// The user preferences.
        /// </value>
        IUserPreferences UserPreferences { get; }

        /// <summary>
        /// A collection of all the Partition's the User belongs to.
        /// </summary>
        IEnumerable<Partition> Partitions { get; }

        /// <summary>
        /// The users web session Id.
        /// </summary>
        string UserSessionId { get; set; }
        
        CultureInfo Culture { get; }
        Version VersionP2000 { get; }

        void AddStreamCallbackForClient(string channelName, string clientId, Action<string, object> callback);
        void RemoveStreamCallbackForClient(string channelName, string clientId);

        void KeepAlive();

        void SignOut();

        IBuildingSecuritySessionStore ParentSessionStore { get; set; }
        IBuildingSecurityClientCookie BuildingSecurityCookie { get;}
        IEnumerable<string> Permissions { get; }
        bool HasPermission(string permissionName);
    }
}
