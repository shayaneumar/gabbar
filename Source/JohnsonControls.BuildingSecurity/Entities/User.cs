/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Timers;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Represents a user of the P2000 system
    /// </summary>
    public class User : IUser
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;
        public IBuildingSecurityClientCookie BuildingSecurityCookie { get; private set; }

        private readonly Timer _selfDestructTimer;

        public IEnumerable<string> Permissions
        {
            get { return _buildingSecurityClient.GetPermissions(BuildingSecurityCookie); }
        }

        public bool HasPermission(string permissionName)
        {
            if (permissionName == null) throw new ArgumentNullException("permissionName");
            return Permissions != null && Permissions.Contains(permissionName, StringComparer.OrdinalIgnoreCase);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Timer is disposed on finalize")]
        public User(IBuildingSecurityClient buildingSecurityClient, IBuildingSecurityClientCookie cookie)
        {
            _buildingSecurityClient = buildingSecurityClient;
            _selfDestructTimer = new Timer {AutoReset = false, Interval = 285000};//4.75 min or 285 seconds
            _selfDestructTimer.Elapsed += DestroyIfInactive;
            _selfDestructTimer.Start();
            Culture = CultureInfo.CurrentCulture;

            BuildingSecurityCookie = cookie;
            if (_buildingSecurityClient.HasPermission(cookie, PermissionNames.CanViewAlarmManager))
            {
                _messageProcessingClient = _buildingSecurityClient.CreateMessageProcessingClient(cookie);
            }
        }

        ~User()
        {
            _selfDestructTimer.Dispose();
        }

        private void DestroyIfInactive(object sender, ElapsedEventArgs e)
        {
            if (!Debugger.IsAttached)//Don't time users out if the debugger is presently attached.
            {
                SignOut();
                if (ParentSessionStore != null) ParentSessionStore.ClearUser(Name);
            }
        }

        /// <summary>
        /// The User's P2000 Identity.
        /// </summary>
        public string Name
        {
            get { return _buildingSecurityClient.GetUserName(BuildingSecurityCookie); }
        }

        /// <summary>
        /// The user's full name
        /// </summary>
        public string FullName
        {
            get { return _buildingSecurityClient.GetFullName(BuildingSecurityCookie); }
        }


        /// <summary>
        /// The Id of the User's Partition
        /// </summary>
        public string PartitionId
        {
            get
            {
                var partition = Partitions.FirstOrDefault();
                return partition != null ? partition.Identifier.ToString() : String.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the users time zone preference
        /// </summary>
        public IUserPreferences UserPreferences
        {
            get
            {
                UserPreferences result;
                return _buildingSecurityClient.TryReadUserPreference(BuildingSecurityCookie, UserSettings.UserTimeZone, out result) ? result : new UserPreferences(TimeZoneInfo.Local.Id);
            }
        }

        /// <summary>
        /// A collection of all the Partition's the User belongs to.
        /// </summary>
        public IEnumerable<Partition> Partitions
        {
            get { return _buildingSecurityClient.GetPartitions(BuildingSecurityCookie); }
        }

        /// <summary>
        /// The users web session Id.
        /// </summary>
        public string UserSessionId
        {
            get { return _userSessionId; }
            set
            {
                if (!string.Equals(_userSessionId,value))
                {
                    _userSessionId = value;
                }
            }
        }

        /// <summary>
        /// The user's Culture based on their browser setting as of login
        /// </summary>
        public CultureInfo Culture { get; private set; }

        public Version VersionP2000
        {
            get
            {
                return _buildingSecurityClient.GetVersion();
            }
        }

        private string _userSessionId = "";
        private readonly Dictionary<string, Dictionary<string, Action<object>>> _channelCallbacks = new Dictionary<string, Dictionary<string, Action<object>>>();
        private IMessageProcessingClient _messageProcessingClient;


        public void AddStreamCallbackForClient(string channelName, string clientId, Action<string, object> callback)
        {
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("", "clientId");
            if (callback == null) throw new ArgumentNullException("callback");
            var clients = GetChannelSpecificCallbacks(channelName);
            lock (clients)
            {
                StartListeningForUpdates();
                clients[clientId] = d => callback(clientId, d);
            }
        }

        public void RemoveStreamCallbackForClient(string channelName, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("", "clientId");
            var channel = GetChannelSpecificCallbacks(channelName);
            lock (channel)
            {
                channel.Remove(clientId);
                StopListeningIfNoClients();
            }
        }

        private Dictionary<string, Action<object>> GetChannelSpecificCallbacks(string channelName)
        {
            lock (_channelCallbacks)
            {
                if (!_channelCallbacks.ContainsKey(channelName))
                {
                    _channelCallbacks.Add(channelName, new Dictionary<string, Action<object>>());
                }
                return _channelCallbacks[channelName];
            }
        }

        private void StopListeningIfNoClients()
        {
            lock (_channelCallbacks)
            {
                if (_messageProcessingClient != null && _channelCallbacks.All(x => x.Value.Count == 0))
                {
                    _messageProcessingClient.UpdateReceived -= UpdateReceived;
                }
            }
        }

        private void StartListeningForUpdates()
        {
            lock (_channelCallbacks)
            {
                if (_messageProcessingClient != null && _channelCallbacks.All(x => x.Value.Count == 0))
                {
                    _messageProcessingClient.UpdateReceived += UpdateReceived;
                }
            }
        }

        private void UpdateReceived(object sender, ChannelUpdateEventArgs e)
        {
            lock (_channelCallbacks)
            {
                var channel = GetChannelSpecificCallbacks(e.ChannelName);
                lock (channel)
                {
                    foreach (var client in channel)
                    {
                        client.Value(e.Update);
                    }
                }
            }
        }

        public void SignOut()
        {
            lock (_channelCallbacks)
            {
                _channelCallbacks.Clear();
                StopListeningIfNoClients();

                if (_messageProcessingClient != null)
                {
                    _messageProcessingClient.Dispose();
                    _messageProcessingClient = null;
                }
                _buildingSecurityClient.SignOut(BuildingSecurityCookie);
            }
        }

        public IBuildingSecuritySessionStore ParentSessionStore { get; set; }

        public void KeepAlive()
        {
            _selfDestructTimer.Stop();
            _selfDestructTimer.Start();
        }
    }
}
