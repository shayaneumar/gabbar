/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Timers;
using JohnsonControls.BuildingSecurity.XmlRpc3.Globalization;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;
using JohnsonControls.Collections;
using JohnsonControls.Diagnostics;
using JohnsonControls.Exceptions;
using JohnsonControls.Serialization;
using JohnsonControls.XmlRpc;
using SortKey = JohnsonControls.BuildingSecurity.XmlRpc3.Services.SortKey;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    /// <summary>
    /// Represents a client that can send messages to a 3.12 P2000 Building Security Server.
    /// </summary>
    public class BuildingSecurityClient : IBuildingSecurityClient
    {
        private readonly ITypedAlarmService _alarmService;
        private readonly ITypedSessionManagement _sessionService;
        private readonly ITypedSystemInformationService _systemInformationService;
        private readonly ITypedApplicationPreference _applicationPreferenceService;
        private readonly IDataSerializerFactory _serializationFactory;
        private readonly Timer _heartbeatTimer;
        private readonly ConcurrentDictionary<string, BuildingSecurityClientCookie> _cookiesToKeepAlive;

        private delegate string SettingReader(string userName, string sessionId, string key);

        /// <summary>
        /// Creates an instance of <see cref="BuildingSecurityClient"/>
        /// </summary>
        /// <param name="alarmService">Reference to the IAlarmService to use</param>
        /// <param name="sessionService"> </param>
        /// <param name="systemInformationService"></param>
        /// <param name="applicationPreferenceService">Service that allows the Saving, Reading, and Deleting of user preferences in the P2000 server.</param>
        /// <param name="serializationFactory">The xml serialization factory to be used when saving and retrieving settings.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <param name="alarmService"/>, 
        /// <param name="sessionService"/>, or <param name="applicationPreferenceService"></param> is null</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lightweight resource, which is disposed on finalize")]
        public BuildingSecurityClient(ITypedAlarmService alarmService, ITypedSessionManagement sessionService, ITypedSystemInformationService systemInformationService, ITypedApplicationPreference applicationPreferenceService, IDataSerializerFactory serializationFactory)
        {
            if (alarmService == null) throw new ArgumentNullException("alarmService");
            if (sessionService == null) throw new ArgumentNullException("sessionService");
            if (systemInformationService == null) throw new ArgumentNullException("systemInformationService");
            if (applicationPreferenceService == null) throw new ArgumentNullException("applicationPreferenceService");
            if (serializationFactory == null) throw new ArgumentNullException("serializationFactory");

            _alarmService = alarmService;
            _sessionService = sessionService;
            _systemInformationService = systemInformationService;
            _applicationPreferenceService = applicationPreferenceService;
            _serializationFactory = serializationFactory;
            _heartbeatTimer = new Timer { AutoReset = true, Interval = 40000 };//40Seconds
            _heartbeatTimer.Elapsed += Heartbeat;
            _heartbeatTimer.Start();
            _cookiesToKeepAlive = new ConcurrentDictionary<string, BuildingSecurityClientCookie>();
        }

        ~BuildingSecurityClient()
        {
            _heartbeatTimer.Dispose();
        }

        public bool TrySignIn(string userName, string password, out IBuildingSecurityClientCookie cookie, out string errorMessage)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentNullException("password");

            P2000LoginReply loginReply;
            try
            {
                loginReply = _sessionService.P2000Login(userName, password);
            }
            catch (ServiceOperationException se)
            {
                var faultCode = se.FaultCode;
                errorMessage = Translator.GetString(CategoryType.FaultCodes, faultCode, CultureInfo.CurrentCulture);
                cookie = null;
                return false;
            }

            var partitions = loginReply.UserDetails.Partitions.Select(p => p.ConvertToPartition()).ToList();
            var sessionId = new Guid(loginReply.SessionInfo.SessionGuid);
            var permissions = new Dictionary<string, bool> 
            { 
                { PermissionNames.CanViewWebUserInterface, loginReply.CanViewWebUserInterface},
                { PermissionNames.CanViewAlarmManager, loginReply.CanViewAlarmManager },
                { PermissionNames.CanEditAlarmDisplayOptions, loginReply.CanEditAlarmManagerSettings},
                { PermissionNames.CanViewAlarmDisplayOptions, loginReply.CanViewAlarmManagerSettings || loginReply.CanViewAlarmManager },
                { PermissionNames.CanViewReports, loginReply.CanViewReports},
                { PermissionNames.CanViewReportsServerSettings, loginReply.CanViewReportsServerSettings },
                { PermissionNames.CanEditReportsServerSettings, loginReply.CanEditReportsServerSettings },
                { PermissionNames.CanRunReports, loginReply.CanViewRunReports},
                { PermissionNames.CanScheduleReports, loginReply.CanAddScheduleReports},
                { PermissionNames.CanViewScheduledReports, loginReply.CanViewScheduleReports},
                { PermissionNames.CanEditScheduledReports, loginReply.CanEditScheduleReports},
                { PermissionNames.CanDeleteScheduledReports, loginReply.CanDeleteScheduleReports},
                { PermissionNames.CanEditSystemSettings, loginReply.CanEditSiteSettings},
            }.ToDictionary(x => x.Key.ToUpperInvariant(), x => x.Value);//Ensure casing of permissions is upper case//Ensure casing of permissions is upper case

            cookie = new BuildingSecurityClientCookie(userName: loginReply.UserDetails.UserName, sessionId: sessionId.ToString(), fullName: loginReply.UserDetails.UserFullName, partitionList: partitions, permissions: permissions);
            errorMessage = "";
            return true;
        }

        private SystemInformation GetSystemInformation(string username, string sessionId)
        {
            if (username == null) throw new ArgumentNullException("username");
            if (sessionId == null) throw new ArgumentNullException("sessionId");

            P2000GetSystemInfoReply systemInfoReply = _systemInformationService.P2000GetSystemInfo(username, sessionId);

            var systemInformation = new SystemInformation(
                new Guid(systemInfoReply.EnterpriseSiteGuid),
                systemInfoReply.EnterpriseSiteName,
                int.Parse(systemInfoReply.XmlRtlPort, CultureInfo.InvariantCulture),
                int.Parse(systemInfoReply.SessionHeartbeatInterval, CultureInfo.InvariantCulture), systemInfoReply.LocaleName);

            return systemInformation;
        }

        public Version GetVersion()
        {
            P2000VersionReply versionReply = _systemInformationService.P2000GetVersionEx();

            var version = new Version(
                versionReply.LastUpdated,
                versionReply.MajorVersion,
                versionReply.MinorVersion,
                versionReply.BuildNumber,
                versionReply.RevisionNumber);

            return version;
        }

        private void Heartbeat(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var tmpKeepAliveList = _cookiesToKeepAlive.ToList().Select(x=>x.Value);//try to prevent race condition by cloning early.
            Parallel.ForEach(tmpKeepAliveList, cookie => 
            {
                if (cookie != null)
                {
                    try
                    {
                        _sessionService.P2000SessionHeartbeat(cookie.UserName, cookie.SessionId);
                    }
                    catch (ServiceTimedOutException se)
                    {
                        Log.Warning("Heartbeat for {0} timed out. Exception={1}, Trace={2}", cookie.UserName, se, se.StackTrace);
                    }
                    catch (ServiceOperationException ex)
                    {
                        Log.Error("Heartbeat for {0} failed with exception: {1}", cookie.UserName, ex);
                    }
                }
            });
        }

        /// <summary>
        /// Logoff of the building security server and terminate the session.
        /// </summary>
        public void SignOut(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            BuildingSecurityClientCookie tmpCookie;
            _cookiesToKeepAlive.TryRemove(cookie.Id, out tmpCookie);
            try
            {
                _sessionService.P2000Logout(bsCookie.UserName, bsCookie.SessionId);
            }
            catch (AuthenticationRequiredException)
            {
                // Disregard AuthenticationRequiredException, the User must have already been signed out of the P2000
            }
            catch (ServiceOperationException se)
            {
                Log.Error("Logout of {0} failed with. Exception={1}, Trace={2}", bsCookie.UserName, se, se.StackTrace);
            }
        }

        public DataChunk<Alarm> RetrieveActiveAlarms(IBuildingSecurityClientCookie cookie, TimeZoneInfo timeZone, CultureInfo culture, string afterId = null, bool sorted = false)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (timeZone == null) throw new ArgumentNullException("timeZone");

            var sortOrder = sorted ? GetSortOrder(bsCookie.UserName, bsCookie.SessionId, afterId) : GetSimpleSortOrder(afterId);

            var response = _alarmService.AlarmGetListEx(bsCookie.UserName, bsCookie.SessionId, null, null, null, null, null, null, _alarmService.MaximumRecordsPerRequest, sortOrder);
            //For right now lets just say if the result is empty we are done. Also seems to be a bug in which the last key maybe null if it is done
            return new DataChunk<Alarm>(response.AlarmMessages.Select(x => x.ConvertToAlarm(timeZone, culture)), response.SortOrder.SortKeys[0].LastKey == null || !response.AlarmMessages.Any());
        }

        private static SortOrder GetSimpleSortOrder(string afterId)
        {
            Guid tmpGuid;
            afterId = !Guid.TryParse(afterId, out tmpGuid) ? null : tmpGuid.ToString().ToUpperInvariant();
            return new SortOrder(new[]
                                    {
                                        new SortKey(sequenceNumber: "0", name: "AlarmGuid", startKey: afterId,lastKey: null)
                                    });
        }

        private SortOrder GetSortOrder(string userName, string sessionId, string afterId)
        {
            Guid tmpGuid;
            afterId = !Guid.TryParse(afterId, out tmpGuid) ? null : tmpGuid.ToString().ToUpperInvariant();

            string lastKeyAlarmTimestamp = null;
            if (afterId != null)
            {
                var alarm =_alarmService.AlarmGetListEx(userName, sessionId, null, afterId, null, null, null, null, 1, null).AlarmMessages.FirstOrDefault();
                if (alarm == null)
                {
                    afterId = null; //Alarm was invalid
                }
                else
                {
                    //P2000 is very picky about datatime formatting
                    lastKeyAlarmTimestamp = DateTime.Parse(alarm.MessageDetails.AlarmTimestamp, CultureInfo.InvariantCulture)
                        .ToString("yyyy-MM-dd hh:mm:ss.fff", CultureInfo.CurrentCulture);
                }
            }

            var sortOrder = new SortOrder(new[]
                                              {
                                                  new SortKey(sequenceNumber: "0", name: "AlarmTimestamp",startKey: lastKeyAlarmTimestamp, lastKey: null),
                                                  new SortKey(sequenceNumber: "1", name: "AlarmGuid", startKey: afterId,lastKey: null)
                                              });
            return sortOrder;
        }

        public IEnumerable<HistoryEntry> RetrieveAlarmDetails(IBuildingSecurityClientCookie cookie, string alarmId, TimeZoneInfo timeZoneInfo)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if(bsCookie == null) throw new ArgumentException(@"Invalid cookie","cookie");
            IEnumerable<HistoryEntry> result = Enumerable.Empty<HistoryEntry>();
            try
            {
                AlarmDetailsReply response;
                var sortKey = new SortKey(sequenceNumber: "0", name: "AlarmHistoryGuid",startKey:null, lastKey: null);
                do
                {
                    response = _alarmService.AlarmDetails(bsCookie.UserName, bsCookie.SessionId, alarmId, _alarmService.MaximumRecordsPerRequest, new SortOrder(new[] { sortKey }));
                    result = result.Concat(response.AlarmHistories.Select(alarmHistory => alarmHistory.ConvertToActionEntry(timeZoneInfo)));
                    sortKey.StartKey = response.SortOrder.SortKeys[0].LastKey;
                } while (response.SortOrder.SortKeys[0].LastKey != null && response.AlarmHistories.Any());
                return result;
            }
            catch (ServiceOperationException)
            {
                // TODO: This is not acceptable to return an empty list and mask the exception
                // We need to either let the exception bubble up or find some other way
                // to communicate an error to the caller.
                return Enumerable.Empty<HistoryEntry>();
            }
        }

        public IList<AlarmServiceResponse> AcknowledgeAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (alarmIds == null) throw new ArgumentNullException("alarmIds");

            return UpdateAlarm(bsCookie.UserName, bsCookie.SessionId, alarmIds, AlarmState.Acknowledged);
        }

        public IList<AlarmServiceResponse> CompleteAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (alarmIds == null) throw new ArgumentNullException("alarmIds");

            return UpdateAlarm(bsCookie.UserName, bsCookie.SessionId, alarmIds, AlarmState.Completed);
        }

        public IList<AlarmServiceResponse> RespondToAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds, string response)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (alarmIds == null) throw new ArgumentNullException("alarmIds");
            if (string.IsNullOrWhiteSpace(response)) throw new ArgumentException("", "response");

            return UpdateAlarm(bsCookie.UserName, bsCookie.SessionId, alarmIds, AlarmState.Responding, response);
        }

        public IEnumerable<string> RetrieveResponseTexts(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");

            var result = new SortedSet<string>();
            AlarmGetResponseTextListReply response;
            var sortOrder = new SortOrder(new[] {new SortKey(sequenceNumber: "0", name: "AlarmResponseText", startKey: null, lastKey: null)});
            int pageNumber = 0;
            do
            {
                response = _alarmService.AlarmGetResponseTextList(bsCookie.UserName, bsCookie.SessionId, sortOrder, new Paging(pageNumber, Paging.MaxRecordsPerPage, Paging.AllRecordsRecordCount));
                foreach (var text in response.ResponseTexts.Select(x => x.AlarmResponseText))
                {
                    result.Add(text);
                }
                pageNumber++;
            } while (response.Paging.RecordCount > response.Paging.RecordsPerPage * pageNumber);
            // Result may <> count as SortedSet only includes unique responses
            // Requesting Page number 100, when only 50 pages exist, will return the contents of page 50

            // Return all "Response Texts" in the response.
            return result;
        }

        /// <summary>
        /// Call AlarmService to set the AlarmState and Response on all of the Alarms in the specified list
        /// </summary>
        /// <param name="userName">The username of the user making this request.</param>
        /// <param name="alarmIds">List of GUIDs for each of the Alarms to be updated</param>
        /// <param name="alarmState">New AlarmState value for each of the Alarms</param>
        /// <param name="response">New Response value for each of the Alarms; only used when the AlarmState is being set to Responding</param>
        /// <param name="sessionId">The current session Id for the user making this request.</param>
        /// <returns>
        /// List of AlarmServiceResponse objects (ServiceResponse with GUID for the Alarm)
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if alarmIds is null.</exception>
        /// <remarks>
        /// It is expected that if alarmState = Responding then response cannot be null. Behavior is unspecified
        /// otherwise.
        /// </remarks>
        private IList<AlarmServiceResponse> UpdateAlarm(string userName, string sessionId, IEnumerable<AlarmIdSequenceTuple> alarmIds, AlarmState alarmState, string response = "")
        {
            // ReSharper disable InvocationIsSkipped
            Debug.Assert(alarmIds != null);
            Debug.Assert(alarmState != AlarmState.Responding || !string.IsNullOrWhiteSpace(response),
                         "The response cannot be null or just whitespace if alarmState = Responding.");
            // ReSharper restore InvocationIsSkipped

            var results = new List<AlarmServiceResponse>();

            foreach (var id in alarmIds)
            {
                int serviceResponseCode = 0;
                string text;

                try
                {
                    _alarmService.AlarmAction(userName, sessionId,
                                              new[] {new Services.AlarmIdSequenceTuple(id.Item1, id.Item2)},
                                              GetAlarmState(alarmState),
                                              response);
                    text = string.Empty;
                }
                catch (ServiceOperationException exception)
                {
                    // If the AlarmService threw a ServiceOperationException, then set the serviceResponseCode
                    // (converted based on the exception.FaultCode)
                    serviceResponseCode = exception.FaultCode;
                    text = Translator.GetString(CategoryType.FaultCodes, exception.FaultCode, CultureInfo.CurrentCulture);
                }

                // Add a new AlarmServiceResponse object to the list
                results.Add(new AlarmServiceResponse(serviceResponseCode, text, id.Item1));
            }
            return new ReadOnlyCollection<AlarmServiceResponse>(results);
        }

        /// <summary>
        /// Returns a AlarmState (int) based on the specified alarmState (enum)
        /// </summary>
        /// <param name="alarmState">AlarmState to be converted to an int</param>
        /// <returns>AlarmState (int) based on the specified alarmState</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the specified alarmState is not recognized</exception>
        private static int GetAlarmState(AlarmState alarmState)
        {
            switch (alarmState)
            {
                case AlarmState.Unknown:        return 0;
                case AlarmState.Completed:      return 1;
                case AlarmState.Responding:     return 2;
                case AlarmState.Acknowledged:   return 3;
                case AlarmState.Pending:        return 4;
                default: throw new InvalidEnumArgumentException("alarmState", (int)alarmState, typeof(AlarmState));
            }
        }

        public void SaveApplicationPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, T value) where T : class
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("", "settingName");
            if (value ==null) throw new ArgumentNullException("value");
            
            var serializer = _serializationFactory.GetSerializer<T>();
            SaveApplicationPreference(bsCookie.UserName, bsCookie.SessionId, settingName, serializer.Serialize(value));
        }

        private void SaveApplicationPreference(string userName, string sessionId, string key, string value)
        {
            _applicationPreferenceService.ApplicationPreferenceSave(userName, sessionId, key, PreferenceType.Application, value);
        }

        public bool TryReadApplicationPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, out T result)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("", "settingName");
            
            return TryReadPreference(bsCookie.UserName, bsCookie.SessionId, settingName, out result, ReadApplicationPreference);
        }

        private bool TryReadPreference<T>(string userName, string sessionId, string settingName, out T result, SettingReader settingReader)
        {
            var deserializer = _serializationFactory.GetSerializer<T>();
            string rawValue;

            try
            {
                rawValue = settingReader(userName, sessionId, settingName);
            }
            catch (ServiceOperationException)
            {
                result = default(T);
                return false;
            }

            if (rawValue == null)
            {
                result = default(T);
                return false;
            }

            try
            {
                result = deserializer.Deserialize(rawValue);
                return true;
            }
            catch (SerializationException)
            {
                result = default(T);
                return false;
            }
        }

        private string ReadApplicationPreference(string userName, string sessionId, string key)
        {
            return _applicationPreferenceService.ApplicationPreferenceRead(userName, sessionId, key, PreferenceType.Application);
        }

        public void DeleteApplicationPreference(IBuildingSecurityClientCookie cookie, string settingName)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("", "settingName");
            
            _applicationPreferenceService.ApplicationPreferenceDelete(bsCookie.UserName, bsCookie.SessionId, settingName, PreferenceType.Application);
        }

        /// <summary>
        /// Saves an application preference for a specified application and preference type.
        /// </summary>
        /// <param name="userName">The username of the user making this request.</param>
        /// <param name="sessionId">The current session Id for the user making this request.</param>
        /// <param name="key">The data key used to store and retrieve data.  This is unique to each piece of data stored.</param>
        /// <param name="value">String representation of the application preference in an XML document.  This value must be enclosed in XML tags.</param>
        private void SaveUserPreference(string userName, string sessionId, string key, string value)
        {
            _applicationPreferenceService.ApplicationPreferenceSave(userName, sessionId, key, PreferenceType.User, value);
        }

        private string ReadUserPreference(string userName, string sessionId, string key)
        {
            return _applicationPreferenceService.ApplicationPreferenceRead(userName, sessionId, key, PreferenceType.User);
        }

        /// <summary>
        /// Gets the user preferences object.  Returns preferences stored on the P2000 if they exist.
        /// If they do not exist, a UserPreferences object is returned with the defaults.
        /// </summary>
        /// <returns></returns>
        public bool TryReadUserPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, out T result)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("", "settingName");
            
            return TryReadPreference(bsCookie.UserName, bsCookie.SessionId, settingName, out result, ReadUserPreference);
        }

        public void SaveUserPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, T value) where T : class
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("", "settingName");
            if (value == null) throw new ArgumentNullException("value");
            
            var serializer = _serializationFactory.GetSerializer<T>();
            SaveUserPreference(bsCookie.UserName, bsCookie.SessionId, settingName, serializer.Serialize(value));
        }

        public void KeepAlive(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            
            _cookiesToKeepAlive.TryAdd(bsCookie.Id, bsCookie);
        }

        public void DeleteUserPreference(IBuildingSecurityClientCookie cookie, string settingName)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("", "settingName");

            _applicationPreferenceService.ApplicationPreferenceDelete(bsCookie.UserName, bsCookie.SessionId, settingName, PreferenceType.User);
        }

        public IEnumerable<Partition> GetPartitions(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            return bsCookie.PartitionList ?? Enumerable.Empty<Partition>();
        }

        public string GetFullName(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            return bsCookie.FullName;
        }

        public string GetUserName(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            return bsCookie.UserName;
        }

        public IMessageProcessingClient CreateMessageProcessingClient(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            var rtlPort = GetSystemInformation(bsCookie.UserName, bsCookie.SessionId).RealTimeServicePort;

            return MessageProcessingClient.GetMessageProcessingClient(new DnsEndPoint(_alarmService.RealTimeServiceAddress, rtlPort), bsCookie.UserName, bsCookie.SessionId, () => GetUsersTimezone(bsCookie), CultureInfo.CurrentCulture);
        }

        private TimeZoneInfo GetUsersTimezone(BuildingSecurityClientCookie cookie)
        {
            UserPreferences userPreferences = TryReadUserPreference(cookie, UserSettings.UserTimeZone, out userPreferences) ? userPreferences : new UserPreferences(TimeZoneInfo.Local.Id);
            return userPreferences.SelectedTimeZoneInfo;

        }

        public bool HasPermission(IBuildingSecurityClientCookie cookie, string permissionName)
        {
            if (permissionName == null) throw new ArgumentNullException("permissionName");
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            bool result;
            return bsCookie.Permissions.TryGetValue(permissionName.ToUpperInvariant(), out result) && result;
        }

        public IEnumerable<string> GetPermissions(IBuildingSecurityClientCookie cookie)
        {
            var bsCookie = cookie as BuildingSecurityClientCookie;
            if (bsCookie == null) throw new ArgumentException(@"Invalid cookie", "cookie");
            return bsCookie.Permissions.Where(x => x.Value).Select(x => x.Key);
        }
        
        /// <summary>
        /// Retrieves a collection of cases.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <returns>A collection of <see cref="Case"/> objects.</returns>
        public DataChunk<Case> RetrieveCases(IBuildingSecurityClientCookie cookie)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a case.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="id">Id of the Case to be returned.</param>
        /// <returns>A <see cref="Case"/> object.</returns>
        public Case RetrieveCase(IBuildingSecurityClientCookie cookie, string id)
        {
            throw new NotImplementedException();
        }

        public Case UpdateCase(IBuildingSecurityClientCookie cookie, string id, dynamic updates)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a Case based on the specified caseDetails object
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="caseTitle">Title for the case</param>
        /// <returns>Globally unique Id of the case that was created</returns>
        public string CreateCase(IBuildingSecurityClientCookie cookie, string caseTitle)
        {
            throw new NotImplementedException();
        }

        public CaseNote CreateCaseNote(IBuildingSecurityClientCookie cookie, string caseId, string text)
        {
            throw new NotImplementedException();
        }
    }
}
