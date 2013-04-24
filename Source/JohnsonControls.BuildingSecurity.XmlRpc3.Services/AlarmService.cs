/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using JohnsonControls.Serialization.Xml;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Defines the methods described in the alarm section of the 3.x P2000 XML RPC documentation 
    /// </summary>
    /// <remarks>All parameters for all members are required.  Any parameter value of null is
    /// invalid and will throw an <see cref="ArgumentNullException"/></remarks>
    public class AlarmService : ITypedAlarmService
    {
        private const int RespondingState = 2;

        private readonly P2000XmlRpcProxy<IAlarmService> _alarmServiceProxy;

        private readonly string _realTimeServiceAddress;

        public AlarmService(Uri serviceUrl, string realTimeServiceAddress)
        {
            _alarmServiceProxy = new P2000XmlRpcProxy<IAlarmService>(serviceUrl);
            _realTimeServiceAddress = realTimeServiceAddress;
        }

        /// <summary>
        /// Host name or a string representation of the IP address for the Real-Time Service
        /// </summary>
        public string RealTimeServiceAddress
        {
            get { return _realTimeServiceAddress; }
        }

        /// <summary>
        /// Maximum number of records that may be returned in a set from an RPC request
        /// </summary>
        public int MaximumRecordsPerRequest
        {
            get { return 50; }
        }

        /// <summary>
        /// Used to ack, respond, or complete an alarm.
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
        public AlarmActionReply AlarmAction(string userName, string sessionGuid, IEnumerable<AlarmIdSequenceTuple> alarmGuids, 
            int alarmState, string response)
        {
            AlarmIdSequenceTuple[] alarmIdSequenceNumberTuples = alarmGuids.ToArray();
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (alarmGuids == null) throw new ArgumentNullException("alarmGuids");
            if (alarmState < 0 || alarmState > 3) throw new ArgumentOutOfRangeException("alarmState");
            if (response == null) throw new ArgumentNullException("response");

            var request = new AlarmActionRequestWrapper
                              {
                                  AlarmActionRequest =
                                      new AlarmActionRequest
                                          {
                                              AlarmActionFilter =
                                                  new AlarmActionFilter
                                                      {
                                                          AlarmGuid =
                                                              new MultipleCVAlarmGuidFilter
                                                                  {
                                                                      CurrentValues =
                                                                          alarmIdSequenceNumberTuples.Select(x => x.Item1.ToString())
                                                                          .ToArray()
                                                                  }
                                                      },
                                              Command = alarmState.ToString(CultureInfo.InvariantCulture),
                                              Parameters = new Parameters()
                                          }
                              };

            // If the Guid/SequenceNumber array only contains 1 value, then the sequence number
            // must be passed to the P2000.
            if (alarmIdSequenceNumberTuples.Count() == 1)
            {
                request.AlarmActionRequest.Parameters.ConditionSequenceNumber =
                    alarmIdSequenceNumberTuples[0].Item2.ToString(CultureInfo.InvariantCulture);
            }

            if (alarmState == RespondingState)
            {
                request.AlarmActionRequest.Parameters.AlarmResponseText = response;
            }

            string serializedRequest = XmlSerializer<AlarmActionRequestWrapper>.Instance.Serialize(request);

            P2000ReturnStructure responseStructure =
                _alarmServiceProxy.Invoke(proxy => proxy.AlarmAction(userName, sessionGuid, serializedRequest));

            return XmlSerializer<AlarmActionReplyWrapper>.Instance.Deserialize(responseStructure.XmlDoc).AlarmActionReply;
        }

        /// <summary>
        /// This function is provided clients to obtain a list of the response texts in the P2000 system. For performance reasons
        /// and to limit the impact of the server, this function supports paging.
        /// </summary>
        /// <param name="userName">The P2000 username associated with the specified session GUID</param>
        /// <param name="sessionGuid">Client’s P2000 session GUID</param>
        /// <param name="sortOrder">A SortOrder object containing an array of SortKeys, and the sort Order</param>
        /// <param name="paging">A Paging object containing the number of records per page, and page number being requested</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception.</exception>
        public AlarmGetResponseTextListReply AlarmGetResponseTextList(string userName, string sessionGuid, SortOrder sortOrder, Paging paging)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");

            // Create default Request object with Paging "Turned off"
            var alarmRequest = new P2000AlarmGetResponseTextListRequest(
                new AlarmGetResponseTextListRequest
                    {
                        Filter = new AlarmResponseTextFilter(),
                        Paging = paging,
                        SortOrder = sortOrder
                    });

            // Serialize the request object
            string xmlDoc = XmlSerializer<P2000AlarmGetResponseTextListRequest>.Instance.Serialize(alarmRequest);

            // Call the P2000, and get back a Serialized response within the returned string
            var responseStructure = _alarmServiceProxy.Invoke(proxy => proxy.AlarmGetResponseTextList(userName, sessionGuid, xmlDoc));

            // Deserialize the response into the P2000AlarmGetResponseTextListReply structure.
            var responseObj =
                XmlSerializer<P2000AlarmGetResponseTextListReply>.Instance.Deserialize(responseStructure.XmlDoc);

            return responseObj.AlarmGetResponseTextListReply;
        }

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
        public AlarmDetailsReply AlarmDetails(string userName, string sessionGuid, string alarmGuid, int recordsPerPage, SortOrder sortOrder)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (alarmGuid == null) throw new ArgumentNullException("alarmGuid");

            var alarmDetailRequest =
                new P2000AlarmDetailsRequestWrapper(new AlarmDetailsRequest(new AlarmDetailsFilter(alarmGuid), recordsPerPage.ToString(CultureInfo.InvariantCulture),
                                                                            sortOrder));

            string xmlDoc = (new XmlSerializer<P2000AlarmDetailsRequestWrapper>()).Serialize(alarmDetailRequest);

            P2000ReturnStructure response = _alarmServiceProxy.Invoke(proxy => proxy.AlarmDetails(userName, sessionGuid, xmlDoc));

            return (new XmlSerializer<P2000AlarmDetailsReplyWrapper>()).Deserialize(response.XmlDoc).AlarmDetailsReply;
        }

        /// <summary>
        /// Invokes the AlarmGetListEx service on the P2000 to retrieve the current list of active alarms.
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
        public AlarmGetListExReply AlarmGetListEx(string userName, string sessionGuid, string partition, string alarmGuid, string alarmSiteName, string alarmTypeName, string itemName, string operatorName, int recordsPerPage, SortOrder sortOrder)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");

            var alarmGetListExRequest =
                new P2000AlarmGetListExRequestWrapper(
                    new AlarmGetListExRequest(
                        new AlarmFilter(partition, alarmGuid, alarmSiteName, alarmTypeName, itemName, operatorName),
                        recordsPerPage.ToString(CultureInfo.InvariantCulture), sortOrder));
            string xmlDoc = (new XmlSerializer<P2000AlarmGetListExRequestWrapper>()).Serialize(alarmGetListExRequest);
            P2000ReturnStructure response = _alarmServiceProxy.Invoke(proxy => proxy.AlarmGetListEx(userName, sessionGuid, xmlDoc));

            return
                (new XmlSerializer<P2000AlarmGetListExReplyWrapper>()).Deserialize(response.XmlDoc).AlarmGetListExReply;
        }

        /// <summary>
        /// Creates an Alarm using the minimal set of properties.
        /// </summary>
        /// <param name="userName">UserName to login as</param>
        /// <param name="password">Password for the specified user</param>
        /// <param name="alarmTypeGuid">Unique ID for the Alarm</param>
        /// <param name="partitionGuid">GUID of the Partition associated with the Alarm</param>
        /// <param name="isPublic">Indicates if the Alarm is marked as public (viewable by users with access to any partition)</param>
        /// <param name="itemName">Name of the Item that generated the Alarm</param>
        /// <param name="description">Description of the Alarm</param>
        /// <param name="instructions">Instructions to resolve the cause of the Alarm</param>
        /// <param name="priority">Alarm Priority (0-255)</param>
        /// <param name="category">Alarm Category (uses default if left blank)</param>
        /// <param name="queryString">Alarm Query String</param>
        /// <param name="acknowledgeRequired">Indicates if Acknowledgement is required for this Alarm</param>
        /// <param name="responseRequired">Indicates if a Response is required for this Alarm</param>
        /// <param name="popup">Indicates if the Alarm should popup when displayed in the Alarm Manager</param>
        /// <exception cref="ArgumentNullException">Thrown if any string parameter is null</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception.</exception>
        public void AlarmCreate(string userName, string password, string alarmTypeGuid, string partitionGuid, int isPublic,
                         string itemName, string description, string instructions, int priority, string category,
                         string queryString, int acknowledgeRequired, int responseRequired, int popup)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            if (alarmTypeGuid == null) throw new ArgumentNullException("alarmTypeGuid");
            if (partitionGuid == null) throw new ArgumentNullException("partitionGuid");
            if (itemName == null) throw new ArgumentNullException("itemName");
            if (description == null) throw new ArgumentNullException("description");
            if (instructions == null) throw new ArgumentNullException("instructions");
            if (category == null) throw new ArgumentNullException("category");
            if (queryString == null) throw new ArgumentNullException("queryString");
            _alarmServiceProxy.Invoke(proxy => proxy.AlarmCreate(userName, password, alarmTypeGuid, partitionGuid, isPublic,
                         itemName, description, instructions, priority, category,
                         queryString, acknowledgeRequired, responseRequired, popup));
        }
        
        /// <summary>
        /// Creates an Alarm using the detailed set of properties.
        /// </summary>
        /// <param name="userName">UserName to login as</param>
        /// <param name="password">Password for the specified user</param>
        /// <param name="alarmTypeGuid">Unique ID for the Alarm</param>
        /// <param name="partitionGuid">GUID of the Partition associated with the Alarm</param>
        /// <param name="isPublic">Indicates if the Alarm is marked as public (viewable by users with access to any partition)</param>
        /// <param name="itemName">Name of the Item that generated the Alarm</param>
        /// <param name="description">Description of the Alarm</param>
        /// <param name="instructions">Instructions to resolve the cause of the Alarm</param>
        /// <param name="priority">Alarm Priority (0-255)</param>
        /// <param name="category">Alarm Category (uses default if left blank)</param>
        /// <param name="queryString">Alarm Query String</param>
        /// <param name="acknowledgeRequired">Indicates if Acknowledgement is required for this Alarm</param>
        /// <param name="responseRequired">Indicates if a Response is required for this Alarm</param>
        /// <param name="popup">Indicates if the Alarm should popup when displayed in the Alarm Manager</param>
        /// <param name="alarmType">P2000 Alarm Type</param>
        /// <param name="alarmTime">UTC Time when the Alarm was activated</param>
        /// <param name="itemState">Current State of the Alarm (-1 = Ignore, 0 = Secure, 1 = Alarm)</param>
        /// <param name="completionState">Item State required for the Alarm to be completed</param>
        /// <exception cref="ArgumentNullException">Thrown if any string parameter is null</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception.</exception>
        public void AlarmCreateDetailed(string userName, string password, string alarmTypeGuid, string partitionGuid,
                                   int isPublic, string itemName, string description, string instructions,
                                   int alarmType, int priority, string category, string queryString,
                                   int acknowledgeRequired, int responseRequired, int popup, DateTime alarmTime,
                                   int itemState, int completionState)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            if (alarmTypeGuid == null) throw new ArgumentNullException("alarmTypeGuid");
            if (partitionGuid == null) throw new ArgumentNullException("partitionGuid");
            if (itemName == null) throw new ArgumentNullException("itemName");
            if (description == null) throw new ArgumentNullException("description");
            if (instructions == null) throw new ArgumentNullException("instructions");
            if (category == null) throw new ArgumentNullException("category");
            if (queryString == null) throw new ArgumentNullException("queryString");
            _alarmServiceProxy.Invoke(proxy => proxy.AlarmCreateDetailed(userName, password, alarmTypeGuid, partitionGuid,
                                   isPublic, itemName, description, instructions,
                                   alarmType, priority, category, queryString,
                                   acknowledgeRequired, responseRequired, popup, alarmTime,
                                   itemState, completionState));
        }

        /// <summary>
        /// Update the ItemState on the Alarm with the specified GUID
        /// </summary>
        /// <param name="userName">UserName to login as</param>
        /// <param name="password">Password for the specified user</param>
        /// <param name="alarmGuid">GUID of the Alarm to be updated</param>
        /// <param name="itemState">New State of the Alarm (-1 = Ignore, 0 = Secure, 1 = Alarm)</param>
        public void AlarmUpdateItemState(string userName, string password, string alarmGuid, int itemState)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            if (alarmGuid == null) throw new ArgumentNullException("alarmGuid");

            _alarmServiceProxy.Invoke(proxy => proxy.AlarmUpdateItemState(userName, password, alarmGuid, itemState));
        }
    }
}
