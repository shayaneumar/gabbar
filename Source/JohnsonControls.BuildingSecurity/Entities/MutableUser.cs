/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JohnsonControls.BuildingSecurity
{
    public class MutableUser : IUser
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string PartitionId { get; set; }
        public IUserPreferences UserPreferences { get; set; }
        public IEnumerable<Partition> Partitions { get; set; }
        public string UserSessionId { get; set; }
        public CultureInfo Culture { get; set; }
        public Version VersionP2000 { get; set; }

        public void AddStreamCallbackForClient(string channelName, string clientId, Action<string, object> callback)
        {}

        public void RemoveStreamCallbackForClient(string channelName, string clientId)
        {}

        public void KeepAlive()
        {}

        public bool HasConnectionIssue { get; set; }
        public bool CanViewAlarmManager { get; set; }
        public bool CanViewReports { get; set; }
        public bool CanViewSystemSetup { get; set; }

        public void SignOut()
        {}

        public IBuildingSecuritySessionStore ParentSessionStore { get; set; }
        public IBuildingSecurityClientCookie BuildingSecurityCookie { get; set; }

        public IEnumerable<string> Permissions { get; set; }
        public bool HasPermission(string permissionName)
        {
            if (permissionName == null) throw new ArgumentNullException("permissionName");
            return Permissions != null && Permissions.Contains(permissionName.ToUpperInvariant());
        }
    }
}
