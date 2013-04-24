/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers
{
    /// <summary>
    /// The <see cref="HistoryController"/> provides the ability for a client application
    /// to retrieve the list of action entries(alarm history) associated with an alarm
    /// in a negotiated format, particularly JSON or XML, over the standard http protocol.
    /// </summary>
    [RequiredPermission(PermissionNames.CanViewAlarmManager)]
    public class HistoryController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public HistoryController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient) : base(sessionStore)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        /// <summary>
        /// Retrieve the history for an alarm.
        /// </summary>
        /// <param name="id">The id of the alarm whose history is being retrieved.</param>
        /// <returns>A collection of <see cref="HistoryEntry"/>, i.e. the alarm history.</returns>
        public IEnumerable<HistoryEntry> Get(string id)
        {
            return _buildingSecurityClient.RetrieveAlarmDetails(BuildingSecurityUser.BuildingSecurityCookie, id, BuildingSecurityUser.UserPreferences.SelectedTimeZoneInfo);
        }
    }
}
