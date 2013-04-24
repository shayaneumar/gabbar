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
    /// The <see cref="AlarmResponsesController"/> provides the ability for a
    /// client application to retrieve the list of pre defined alarms responses
    ///  in a negotiated format, particularly JSON or XML, over the standard http protocol.
    /// </summary>
    [RequiredPermission(PermissionNames.CanViewAlarmManager)]
    public class AlarmResponsesController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public AlarmResponsesController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient) : base(sessionStore)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        /// <summary>
        /// Retrieve the complete list of alarm responses.
        /// </summary>
        /// <returns>A collection of strings that are the pre defined alarm responses.</returns>
        public IEnumerable<string> Get()
        {
            return _buildingSecurityClient.RetrieveResponseTexts(BuildingSecurityUser.BuildingSecurityCookie);
        }
    }
}