/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Net;
using CookComputing.XmlRpc;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Defines the methods described in the alarm section of the 3.x P2000 XML RPC documentation 
    /// </summary>
    /// <remarks>All parameters for all members are required.  Any parameter value of null is
    /// invalid and will throw an <see cref="ArgumentNullException"/></remarks>
    public interface IAlarmService
    {
        /// <summary>
        /// Retrieves the state of an Alarm.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="alarmGuid">The alarm GUID.</param>
        /// <returns>
        /// Returns an <see cref="int"/>  
        /// <value>0</value> = not found, 
        /// <value>1</value> = completed, 
        /// <value>2</value> = responding, 
        /// <value>3</value> = acknowledged, 
        /// <value>4</value> = pending
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception.</exception>
        [XmlRpcMethod("AlarmGetState")]
        int AlarmGetState(string userName, string password, string alarmGuid);

        /// <summary>
        /// Updates the Alarm with the specified state.  Response only applies to
        /// alarmState = 2.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionGuid"> </param>
        /// <param name="xmlDoc"> </param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception.</exception>
        [XmlRpcMethod("AlarmAction")]
        P2000ReturnStructure AlarmAction(string userName, string sessionGuid, string xmlDoc);

        /// <summary>
        /// This function is provided clients to obtain a list of the response texts in the P2000 system. For performance reasons
        /// and to limit the impact of the server, this function supports paging.
        /// </summary>
        /// <param name="userName">The P2000 username associated with the specified session GUID</param>
        /// <param name="sessionGuid">Client’s P2000 session GUID</param>
        /// <param name="xmlDoc">String representation of an XML document defining the request parameter.
        /// Please see <see cref="AlarmGetResponseTextListReply"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is passed in as null.</exception>
        /// <exception cref="ServiceOperationException">Thrown if a Fault Code is returned by the RPC Server
        /// or if there are any <see cref="WebException"/> while trying to contact the XML-RPC Server.
        /// Captured exceptions will be included as an inner exception.</exception>
        [XmlRpcMethod("AlarmGetResponseTextList")]
        P2000ReturnStructure AlarmGetResponseTextList(string userName, string sessionGuid, string xmlDoc);

        /// <summary>
        /// Invokes an API call for AlarmDetails
        /// </summary>
        /// <param name="userName">The P2000 username associated with the specified session GUID</param>
        /// <param name="sessionGuid">Client’s P2000 session GUID</param>
        /// <param name="xmlDoc">XML document that contains the criteria and results configuration for the request</param>
        /// <returns>
        /// P2000ReturnStructure containing the XmlDoc (as a string), with
        /// the details and history of the Alarm with the specified GUID.
        /// </returns>
        [XmlRpcMethod("AlarmDetails")]
        P2000ReturnStructure AlarmDetails(string userName, string sessionGuid, string xmlDoc);

        /// <summary>
        /// Invokes an API call for AlarmGetListEx
        /// </summary>
        /// <param name="userName">The P2000 username associated with the specified session GUID</param>
        /// <param name="sessionGuid">Client’s P2000 session GUID</param>
        /// <param name="xmlDoc">XML document contain the criteria and results configuration for the AlarmGetListEx request</param>
        /// <returns>
        /// P2000ReturnStructure containing the XmlDoc (as a string), with
        /// the list of the Alarms matching the specified criteria.
        /// </returns>
        [XmlRpcMethod("AlarmGetListEx")]
        P2000ReturnStructure AlarmGetListEx(string userName, string sessionGuid, string xmlDoc);

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
        [XmlRpcMethod("AlarmCreate")]
        void AlarmCreate(string userName, string password, string alarmTypeGuid, string partitionGuid, int isPublic,
                           string itemName, string description, string instructions, int priority, string category,
                           string queryString, int acknowledgeRequired, int responseRequired, int popup);

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
        [XmlRpcMethod("AlarmCreateDetailed")]
        void AlarmCreateDetailed(string userName, string password, string alarmTypeGuid, string partitionGuid,
                                   int isPublic, string itemName, string description, string instructions,
                                   int alarmType, int priority, string category, string queryString,
                                   int acknowledgeRequired, int responseRequired, int popup, DateTime alarmTime,
                                   int itemState, int completionState);

        /// <summary>
        /// Update the ItemState on the Alarm with the specified GUID
        /// </summary>
        /// <param name="userName">UserName to login as</param>
        /// <param name="password">Password for the specified user</param>
        /// <param name="alarmGuid">GUID of the Alarm to be updated</param>
        /// <param name="itemState">New State of the Alarm (-1 = Ignore, 0 = Secure, 1 = Alarm)</param>
        [XmlRpcMethod("AlarmUpdateItemState")]
        void AlarmUpdateItemState(string userName, string password, string alarmGuid, int itemState);
    }
}
