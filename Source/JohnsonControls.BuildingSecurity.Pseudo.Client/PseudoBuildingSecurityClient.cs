/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime;
using JohnsonControls.Collections;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    public class PseudoBuildingSecurityClient : IBuildingSecurityClient
    {
        private readonly PseudoMessageProcessingClient _messageProcessing = new PseudoMessageProcessingClient();
        internal readonly InMemoryAlarmRepository AlarmRepository;
        internal readonly InMemoryCaseRepository CaseRepository;
        internal readonly InMemoryUserRepository UserRepository;
        private readonly Dictionary<string, object> _applicationSettings = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _userSettings = new Dictionary<string, object>();
        private readonly IList<string> _cannedResponseTexts = new List<string>();

        public PseudoBuildingSecurityClient()
        {
            AlarmRepository = new InMemoryAlarmRepository(a => _messageProcessing.Update(a));
            CaseRepository = new InMemoryCaseRepository(c => _messageProcessing.CaseUpdate(c));
            UserRepository = new InMemoryUserRepository();
        }

        public PseudoBuildingSecurityClient(InMemoryCaseRepository caseRepository)
        {
            AlarmRepository = new InMemoryAlarmRepository(a => _messageProcessing.Update(a));
            CaseRepository = caseRepository;
            UserRepository = new InMemoryUserRepository();
        }

        public DataChunk<Alarm> RetrieveActiveAlarms(IBuildingSecurityClientCookie cookie, TimeZoneInfo timeZone, CultureInfo culture, string afterId = null, bool sorted = false)
        {
            return new DataChunk<Alarm>(AlarmRepository.ToList(), true);
        }

        public IEnumerable<HistoryEntry> RetrieveAlarmDetails(IBuildingSecurityClientCookie cookie, string alarmId, TimeZoneInfo timeZoneInfo)
        {
            return AlarmRepository.GetResponses(Guid.Parse(alarmId));
        }

        public IList<AlarmServiceResponse> AcknowledgeAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds)
        {
            IList<AlarmServiceResponse> result = new List<AlarmServiceResponse>();
            foreach (var alarmIdSequenceTuple in alarmIds)
            {
                AlarmRepository.UpdateAlarm(new AlarmData { Id = alarmIdSequenceTuple.Item1.ToString(), IsPending = false });
                result.Add(new AlarmServiceResponse(0, "", alarmIdSequenceTuple.Item1));
            }

            return result;
        }

        public IList<AlarmServiceResponse> CompleteAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds)
        {
            IList<AlarmServiceResponse> result = new List<AlarmServiceResponse>();
            foreach (var alarmIdSequenceTuple in alarmIds)
            {
                AlarmRepository.CompleteAlarm(alarmIdSequenceTuple.Item1);
                result.Add(new AlarmServiceResponse(0, "", alarmIdSequenceTuple.Item1));
            }

            return result;
        }

        public IList<AlarmServiceResponse> RespondToAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds, string response)
        {
            IList<AlarmServiceResponse> result = new List<AlarmServiceResponse>();
            var user = ((PseudoCookie)cookie).UserName;
            foreach (var alarmIdSequenceTuple in alarmIds)
            {
                AlarmRepository.RespondToAlarm(alarmId: alarmIdSequenceTuple.Item1, response: response, userid: user);
                AlarmRepository.UpdateAlarm(new AlarmData { Id = alarmIdSequenceTuple.Item1.ToString(), IsPending = false, IsResponseRequired = false });
                result.Add(new AlarmServiceResponse(0, "", alarmIdSequenceTuple.Item1));
            }

            return result;
        }

        public IEnumerable<string> RetrieveResponseTexts(IBuildingSecurityClientCookie cookie)
        {
            lock (_cannedResponseTexts)
            {
                return _cannedResponseTexts.ToList();
            }
        }

        internal void AddCannedResponseText(string text)
        {
            lock (_cannedResponseTexts)
            {
                _cannedResponseTexts.Add(text);
            }
        }

        public bool TrySignIn(string userName, string password, out IBuildingSecurityClientCookie cookie, out string errorMessage)
        {
            if (string.Equals(userName, "lockout", StringComparison.InvariantCultureIgnoreCase))
            {
                cookie = null;
                errorMessage = "User account disabled. (This string comes from the simulator)";
                return false;
            }
            cookie = new PseudoCookie(userName);
            errorMessage = "";
            return true;
        }

        public void SignOut(IBuildingSecurityClientCookie cookie)
        {
        }

        public Version GetVersion()
        {
            var simulatorVersion = GetType().Assembly.GetName().Version;
            return new Version("NA", "SIM-" + simulatorVersion.Major, simulatorVersion.Minor.ToString(CultureInfo.InvariantCulture),
                simulatorVersion.Build.ToString(CultureInfo.InvariantCulture), simulatorVersion.Revision.ToString(CultureInfo.InvariantCulture));
        }

        public void SaveApplicationPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, T value) where T : class
        {
            _applicationSettings[settingName] = value;
        }

        public bool TryReadApplicationPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, out T result)
        {
            object val;
            if (_applicationSettings.TryGetValue(settingName, out val))
            {
                try
                {
                    result = (T)val;
                    return true;
                }
                catch (InvalidCastException)
                {
                }
            }
            result = default(T);
            return false;
        }

        public void DeleteApplicationPreference(IBuildingSecurityClientCookie cookie, string settingName)
        {
            _applicationSettings.Remove(settingName);
        }

        public bool TryReadUserPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, out T result)
        {
            object val;
            if (_userSettings.TryGetValue(GetUserSettingKey(((PseudoCookie)cookie).UserName, settingName), out val) && val is T)
            {
                try
                {
                    result = (T)val;
                    return true;
                }
                catch (InvalidCastException)
                {
                }
            }
            result = default(T);
            return false;
        }

        private static string GetUserSettingKey(string username, string settingName)
        {
            return username + '♣' + settingName;
        }

        public void SaveUserPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, T value) where T : class
        {
            _userSettings[GetUserSettingKey(((PseudoCookie)cookie).UserName, settingName)] = value;
        }

        public void DeleteUserPreference(IBuildingSecurityClientCookie cookie, string settingName)
        {
            _userSettings.Remove(GetUserSettingKey(((PseudoCookie)cookie).UserName, settingName));
        }

        public void KeepAlive(IBuildingSecurityClientCookie cookie)
        {
        }

        public IEnumerable<Partition> GetPartitions(IBuildingSecurityClientCookie cookie)
        {
            yield return new Partition("Super User", new Guid("AD336167-531E-49BF-80C1-0A669793C3B0"));
        }

        public string GetFullName(IBuildingSecurityClientCookie cookie)
        {
            return ((PseudoCookie)cookie).UserName;
        }

        public string GetUserName(IBuildingSecurityClientCookie cookie)
        {
            return ((PseudoCookie)cookie).UserName;
        }

        public IMessageProcessingClient CreateMessageProcessingClient(IBuildingSecurityClientCookie cookie)
        {
            return _messageProcessing;
        }

        public bool HasPermission(IBuildingSecurityClientCookie cookie, string permissionName)
        {
            return true;
        }

        public IEnumerable<string> GetPermissions(IBuildingSecurityClientCookie cookie)
        {
            return UserRepository.GetPermissionsOf(((PseudoCookie)cookie).UserName);
        }

        /// <summary>
        /// Retrieves a collection of cases.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <returns>A collection of <see cref="Case"/> objects.</returns>
        public DataChunk<Case> RetrieveCases(IBuildingSecurityClientCookie cookie)
        {
            return new DataChunk<Case>(CaseRepository.ToList(), true);
        }

        /// <summary>
        /// Retrieve a case.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="id">Id of the Case to be returned.</param>
        /// <returns>A <see cref="Case"/> object.</returns>
        public Case RetrieveCase(IBuildingSecurityClientCookie cookie, string id)
        {
            return CaseRepository.RetrieveCase(id);
        }

        public Case UpdateCase(IBuildingSecurityClientCookie cookie, string id, object updates)
        {
            //Transform dynamic into an object understood by the subsystem (in this case CaseRepo) that we are interacting with
            var updateBundle = new CaseData {Id = id};

            updates.TryGetValue(@"Owner", (string o) => updateBundle.Owner = o);
            updates.TryGetValue(@"Title", (string t) => updateBundle.Title = t);
            updates.TryGetValue(@"Status", (string cs) => updateBundle.Status = cs);

            CaseRepository.UpdateCase(updateBundle);
            return CaseRepository.RetrieveCase(id);
        }

        /// <summary>
        /// Creates a Case based on the specified caseDetails object
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="caseTitle">Title for the case</param>
        /// <returns>Globally unique Id of the case that was created</returns>
        public string CreateCase(IBuildingSecurityClientCookie cookie, string caseTitle)
        {
            string currentUser = GetUserName(cookie);
            return CaseRepository.CreateCase(new CaseData { Title = caseTitle, CreatedBy = currentUser, CreatedDateTime = DateTimeOffset.UtcNow, Owner = currentUser, Status = @"open"});
        }

        public CaseNote CreateCaseNote(IBuildingSecurityClientCookie cookie, string caseId, string text)
        {
            var caseNoteData = new CaseNoteData
                {
                    CaseId = caseId,
                    Text = text,
                    CreatedBy = GetUserName(cookie)
                };

            return CaseRepository.CreateCaseNote(DateTime.UtcNow, caseNoteData);
        }
    }
}
