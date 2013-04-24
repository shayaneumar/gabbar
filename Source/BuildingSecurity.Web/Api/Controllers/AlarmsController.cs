/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Collections;

namespace BuildingSecurity.Web.Api.Controllers
{
    /// <summary>
    /// The <see cref="AlarmsController"/> provides the ability for a
    /// client application to retrieve the list of active alarms
    /// in a negotiated format, particularly JSON or XML, over the standard http protocol.
    /// </summary>
    [RequiredPermission(PermissionNames.CanViewAlarmManager)]
    public class AlarmsController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public AlarmsController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient) : base(sessionStore)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        /// <summary>
        /// Retrieve the complete list of active alarms.
        /// </summary>
        /// <returns>A collection of <see cref="Alarm"/>.</returns>
        public DataChunk<Alarm> Get(string id = null, string after = null, bool sorted = false)
        {
            if (id == null)
            {
                return _buildingSecurityClient.RetrieveActiveAlarms(BuildingSecurityUser.BuildingSecurityCookie, BuildingSecurityUser.UserPreferences.SelectedTimeZoneInfo, BuildingSecurityUser.Culture, after, sorted);
            }

            // Retrieving a specific alarm is not implemented yet.
            throw new HttpResponseException(HttpResponses.NotImplementedMessage);
        }

        /// <summary>
        /// Take an action on a collection of alarms. The actions supported are (Acknowledge, Respond, Complete).
        /// </summary>
        /// <param name="actionInput">A <see cref="AlarmActionInput"/> that contains all the parameters needed to take an action on a collection of alarms.</param>
        /// <returns>A collection of <see cref="ActionResult"/>.</returns>
        public IEnumerable<ActionResult> PostAction(AlarmActionInput actionInput)
        {
            if (actionInput == null || actionInput.Action == null || !actionInput.Alarms.Any())
            {
                throw new HttpResponseException(HttpResponses.BadRequestMessage);
            }
            return PerformAction(BuildingSecurityUser, actionInput.Action, actionInput.Alarms, actionInput.Response);
        }

        private IEnumerable<ActionResult> PerformAction(IUser user, string action, IEnumerable<AlarmIdSequenceTuple> alarms, string response)
        {
            switch (action.ToUpperInvariant())
            {
                case "ACKNOWLEDGE":
                    return AcknowledgeAlarms(user, alarms);

                case "RESPOND":
                    return RespondToAlarms(user, alarms, response);

                case "COMPLETE":
                    return CompleteAlarms(user, alarms);

                default:
                    throw new HttpResponseException(HttpResponses.BadRequestMessage);
            }
        }

        private IEnumerable<ActionResult> AcknowledgeAlarms(IUser user, IEnumerable<AlarmIdSequenceTuple> alarms)
        {
            return TransformResponses(_buildingSecurityClient.AcknowledgeAlarm(user.BuildingSecurityCookie, alarms));
        }

        private IEnumerable<ActionResult> RespondToAlarms(IUser user, IEnumerable<AlarmIdSequenceTuple> alarms, string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                throw new HttpResponseException(HttpResponses.BadRequestMessage);
            }

            return TransformResponses(_buildingSecurityClient.RespondToAlarm(user.BuildingSecurityCookie, alarms, response));
        }

        private IEnumerable<ActionResult> CompleteAlarms(IUser user, IEnumerable<AlarmIdSequenceTuple> alarms)
        {
            return TransformResponses(_buildingSecurityClient.CompleteAlarm(user.BuildingSecurityCookie, alarms));
        }

        private static IEnumerable<ActionResult> TransformResponses(IEnumerable<AlarmServiceResponse> responses)
        {
            return responses.Select(r => new ActionResult(r.Id, r.Success ? string.Empty : r.Text));
        }
    }
}
