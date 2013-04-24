/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using JohnsonControls.Collections;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// Access a Building Security server
    /// </summary>
    public interface IBuildingSecurityClient
    {
        /// <summary>
        /// Retrieves a partial collection of non-completed alarms for a given user. To retrieve all alarms the caller must
        /// call this method multiple times using the id of the last alarm in each request as the sequencing identifier to be passed
        /// into <see cref="afterId"/>.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="timeZone">TimeZone to localize date/times to</param>
        /// <param name="culture">Culture to localize text to</param>
        /// <param name="afterId">The Id of the last alarm returned. Null if caller wants to retrieve the first set of alarms.</param>
        /// <param name="sorted">If sort order of the returned alarms matters</param>
        /// <returns>A collection of <see cref="Alarm"/> objects.</returns>
        DataChunk<Alarm> RetrieveActiveAlarms(IBuildingSecurityClientCookie cookie, TimeZoneInfo timeZone, CultureInfo culture, string afterId = null, bool sorted = false);

        /// <summary>
        /// Retrieves the collection of entries that comprise the history of the alarm.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="alarmId">The alarm Id that identifies the alarm we are requesting the history for.</param>
        /// <param name="timeZoneInfo">The selected UI time zone to display all times in.</param>
        /// <returns>A enumerable collection of <see cref="HistoryEntry"/>.</returns>
        IEnumerable<HistoryEntry> RetrieveAlarmDetails(IBuildingSecurityClientCookie cookie, string alarmId, TimeZoneInfo timeZoneInfo);

        /// <summary>
        /// Sends a message to the server that the specified alarms are to be acknowledged.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="alarmIds">An IEnumerable of Tuples that contain the alarm guid and the
        /// associated Condition Sequence Number</param>
        /// <returns>
        /// List of AlarmServiceResponse objects (ServiceResponse with GUID for the Alarm)
        /// </returns>
        IList<AlarmServiceResponse> AcknowledgeAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds);

        /// <summary>
        /// Sends a message to the server that the specified alarms are to be completed.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="alarmIds">An IEnumerable of Tuples that contain the alarm guid and the
        /// associated Condition Sequence Number</param>
        /// <returns>List of AlarmServiceResponse objects (ServiceResponse with GUID for the Alarm)</returns>
        IList<AlarmServiceResponse> CompleteAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds);

        /// <summary>
        /// Sends a common response message to the server for each specified alarm.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="alarmIds">An IEnumerable of Tuples that contain the alarm guid and the
        /// associated Condition Sequence Number</param>
        /// <param name="response">The response to send for each of the Alarms</param>
        /// <returns>List of AlarmServiceResponse objects (ServiceResponse with GUID for the Alarm)</returns>
        /// <exception cref="ArgumentNullException">Thrown if response is null</exception>
        IList<AlarmServiceResponse> RespondToAlarm(IBuildingSecurityClientCookie cookie, IEnumerable<AlarmIdSequenceTuple> alarmIds, string response);

        /// <summary>
        /// Retrieves the collection of pre-defined alarm responses for the currently
        /// logged in user.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <returns>A enumerable collection of <see cref="string"/>s.</returns>
        IEnumerable<string> RetrieveResponseTexts(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// Logon to the building security server and initiate a session.
        /// </summary>
        /// <param name="userName">>The name of the user to be validated.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="cookie">If sign-in is successful will contain a cookie which can be used for future calls </param>
        /// <param name="errorMessage">If sign-in will contain a localized error message</param>
        /// <returns>A Session object that contains information about the session initiated.</returns>
        /// <exception cref="BuildingSecurityAuthenticationFailedException">Thrown if authentication failed</exception>
        bool TrySignIn(string userName, string password, out IBuildingSecurityClientCookie cookie, out string errorMessage);

        /// <summary>
        /// Logoff of the building security server and terminate the session.
        /// </summary>
        void SignOut(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// Returns the Version information about the P2000 server.
        /// </summary>
        /// <returns></returns>
        Version GetVersion();

        /// <summary>
        /// Saves an application preference for a specified application and preference type.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="settingName">The name of the setting.  This is unique to each piece of data stored.</param>
        /// <param name="value">The value to be saved (Must be xml serializable)</param>
        /// <remarks>To reduce chances of collisions the setting name should follow the following format:
        /// threeLetterCompanyId.applicationName.featureName.settingName</remarks>
        void SaveApplicationPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, T value) where T : class;

        /// <summary>
        /// Gets and returns the saved string for the specified applicationKey.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="settingName">The name of the setting.  This is unique to each piece of data stored.</param>
        /// <param name="result">The deserialized value. T must be xml serializable</param>
        /// <returns>true if the preference was retrieved successfully.</returns>
        bool TryReadApplicationPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, out T result);

        /// <summary>
        /// Deletes the data for the specified applicationKey.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="settingName">The name of the setting.  This is unique to each piece of data stored.</param>
        void DeleteApplicationPreference(IBuildingSecurityClientCookie cookie, string settingName);

        /// <summary>
        /// Gets the user preferences object.  Returns preferences stored on the 2000 if they exist.
        /// If they do not exist, a UserPreferences object is returned with the defaults.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="settingName">The name of the setting.  This is unique to each piece of data stored.</param>
        /// <param name="result">The deserialized value. T must be xml serializable</param>
        /// <returns>true if the preference was retrieved successfully.</returns>
        /// <remarks>To reduce chances of collisions the setting name should follow the following format:
        /// threeLetterCompanyId.applicationName.featureName.settingName</remarks>
        bool TryReadUserPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, out T result);

        /// <summary>
        /// Saves (Persists) the user preferences.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="settingName">The name of the setting.  This is unique to each piece of data stored.</param>
        /// <param name="value">The value to be saved (Must be xml serializable)</param>
        void SaveUserPreference<T>(IBuildingSecurityClientCookie cookie, string settingName, T value) where T : class;

        /// <summary>
        /// Deletes the data for the specified applicationKey.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="settingName">The name of the setting.  This is unique to each piece of data stored.</param>
        void DeleteUserPreference(IBuildingSecurityClientCookie cookie, string settingName);

        /// <summary>
        /// Attempts to keep the <see cref="cookie"/> alive/valid.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        void KeepAlive(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// A collection of all the Partition's which the cookie provides access to.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        IEnumerable<Partition> GetPartitions(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// The username of the user who this cookie authenticates caller as.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <returns>The username of the user who this cookie authenticates caller as.</returns>
        string GetFullName(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// The full name of the user who this cookie authenticates caller as.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <returns>The full name of the user who this cookie authenticates caller as.</returns>
        string GetUserName(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// Creates a message processing client which will raise alarm related
        /// events.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <remarks>It is the caller's responsibility to dispose of the returned client.</remarks>
        IMessageProcessingClient CreateMessageProcessingClient(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// Determines if the specified cookie provides access to a permission.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="permissionName">The name of the permission to be checked.</param>
        /// <returns>Returns true if the cookie provides access, false if it does not.</returns>
        bool HasPermission(IBuildingSecurityClientCookie cookie, string permissionName);

        IEnumerable<string> GetPermissions(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// Retrieves a collection of cases.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <returns>A collection of <see cref="Case"/> objects.</returns>
        DataChunk<Case> RetrieveCases(IBuildingSecurityClientCookie cookie);

        /// <summary>
        /// Retrieve a case.
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="id">Id of the Case to be returned.</param>
        /// <returns>A <see cref="Case"/> object.</returns>
        Case RetrieveCase(IBuildingSecurityClientCookie cookie, string id);

        /// <summary>
        /// Updates a case with new information. (Like changing the title or the owner)
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="id">Id of the Case to be updated.</param>
        /// <param name="updates">An property bag containing the changes the caller would like to make to the case.</param>
        /// <returns>The updated case</returns>
        Case UpdateCase(IBuildingSecurityClientCookie cookie, string id, object updates);

        /// <summary>
        /// Creates a Case based on the specified caseDetails object
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="caseTitle">Title for the case</param>
        /// <returns>Globally unique Id of the case that was created</returns>
        string CreateCase(IBuildingSecurityClientCookie cookie, string caseTitle);

        /// <summary>
        /// Adds a Note with the specified text to the case with the specified caseId
        /// </summary>
        /// <param name="cookie">A <see cref="IBuildingSecurityClientCookie"/> which was previously given to caller by this instance.</param>
        /// <param name="caseId">Id of the Case to add the Note to.</param>
        /// <param name="text">Text to be included in the Note.</param>
        /// <returns>Globally unique Id of the Note that was created</returns>
        CaseNote CreateCaseNote(IBuildingSecurityClientCookie cookie, string caseId, string text);
    }
}
