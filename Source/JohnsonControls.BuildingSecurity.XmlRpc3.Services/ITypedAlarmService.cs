/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Extends the <see cref="IAlarmService"/> with strongly typed versions of the
    /// 3.12 services.
    /// </summary>
    /// <remarks>
    /// The pattern for the additional services defined for 3.12 was to just have
    /// one input parameter of type string and a return value of type string. This
    /// string is expected to be an XML document of the schema specified in the 3.12
    /// RPC documentation. This API is not very useful for a developer. This interface
    /// enhances the API by providing strongly typed versions of the services and handles
    /// all of the XML serialization and deserialization.
    /// </remarks>
    public interface ITypedAlarmService
    {
        /// <summary>
        /// Retrieves a list of Response Texts from the P2000.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionGuid">The session GUID.</param>
        /// <param name="sortOrder">A SortOrder object containing an array of SortKeys, and the sort Order</param>
        /// <param name="paging">A Paging object containing the number of records per page, and page number being requested</param>
        /// <returns><see cref="AlarmGetResponseTextListReply"/></returns>
        AlarmGetResponseTextListReply AlarmGetResponseTextList(string userName, string sessionGuid, SortOrder sortOrder, Paging paging);

        /// <summary>
        /// Alarms the update.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionGuid">The session GUID.</param>
        /// <param name="alarmGuids">An IEnumerable of Tuples that contain the alarm guid and the
        /// associated Condition Sequence Number</param>
        /// <param name="alarmState">State of the alarm.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="alarmState"/> is not one
        /// of the following: 1 = completed, 2 = responding, 3 = acknowledged.</exception>
        AlarmActionReply AlarmAction(string userName, string sessionGuid, IEnumerable<AlarmIdSequenceTuple> alarmGuids, int alarmState, string response);

        /// <summary>
        /// Invokes the AlarmDetails method on the P2000 to retrieve the list of details for a given alarm.
        /// </summary>
        /// <param name="userName">The P2000 username associated with the specified session GUID</param>
        /// <param name="sessionGuid">Client’s P2000 session GUID</param>
        /// <param name="alarmGuid">GUID (Alarm.ItemGuid) of the AlarmDetails to be returned</param>
        /// <param name="recordsPerPage">Number of Records Per Page to be returned</param>
        /// <param name="sortOrder">A SortOrder object containing an array of SortKeys, and the sort Order</param>
        /// <returns>
        /// AlarmDetailsReply object containing the details and history of the Alarm with the
        /// specified GUID.
        /// </returns>
        AlarmDetailsReply AlarmDetails(string userName, string sessionGuid, string alarmGuid, int recordsPerPage, SortOrder sortOrder);

        /// <summary>
        /// Calls the IAlarmService.AlarmGetListEx method, then translates the response XML
        /// into an AlarmGetListExReply object
        /// </summary>
        /// <param name="userName">The P2000 username associated with the specified session GUID</param>
        /// <param name="sessionGuid">Client’s P2000 session GUID</param>
        /// <param name="partition">Name of the Partition to filter on</param>
        /// <param name="alarmGuid">GUID (Alarm.ItemGuid) of the Alarm to be returned</param>
        /// <param name="alarmSiteName">Name of the Site to filter on</param>
        /// <param name="alarmTypeName">Name of the Alarm Type to filter on</param>
        /// <param name="itemName">Name of the Item associated with the Alarm(s) to filter on</param>
        /// <param name="operatorName">Name of the Operator who updated the Alarm(s) to filter on</param>
        /// <param name="recordsPerPage">Number of Records Per Page to be returned</param>
        /// <param name="sortOrder">A SortOrder object containing an array of SortKeys, and the sort Order</param>
        /// <returns>AlarmGetListExReply object containing the deserialized list of Alarms</returns>
        AlarmGetListExReply AlarmGetListEx(string userName, string sessionGuid, string partition, string alarmGuid, string alarmSiteName, string alarmTypeName, string itemName, string operatorName, int recordsPerPage, SortOrder sortOrder);

        /// <summary>
        /// Host name or a string representation of the IP address for the Real-Time Service
        /// </summary>
        string RealTimeServiceAddress { get; }
        
        /// <summary>
        /// Maximum number of records that may be returned in a set from an RPC request
        /// </summary>
        int MaximumRecordsPerRequest { get; }
    }
}
