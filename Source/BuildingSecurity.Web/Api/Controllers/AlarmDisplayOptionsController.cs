/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using BuildingSecurity.Globalization;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers
{
    /// <summary>
    /// The <see cref="AlarmDisplayOptionsController"/> provides the ability for a
    /// client application to retrieve the list of <see cref="AlarmDisplayRange"/>
    /// that together define the Alarm Display Options
    /// in a negotiated format, particularly JSON or XML, over the standard http protocol.
    /// </summary>
    [RequiredPermission(PermissionNames.CanViewAlarmDisplayOptions)]
    public class AlarmDisplayOptionsController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public AlarmDisplayOptionsController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient)
            : base(sessionStore)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        /// <summary>
        /// Retrieve the Alarm Display Options.
        /// </summary>
        /// <returns>A list of <see cref="AlarmDisplayRange"/>. The complete list defines the Alarm Display Options.</returns>
        public IEnumerable<AlarmDisplayRange> Get(string id = null)
        {
            if(id != null && string.Compare(id, "default", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return DisplayOptionDefaults;
            }

            return GetAlarmDisplayOptions(BuildingSecurityUser);
        }

        /// <summary>
        /// Save updates to the Alarm Display Options.
        /// </summary>
        /// <param name="displayOptions">A collection of <see cref="AlarmDisplayRange"/> containing the updates to be saved.</param>
        /// <returns>A success message.</returns>
        [RequiredPermission(PermissionNames.CanEditAlarmDisplayOptions)]
        public string Post(AlarmDisplayOptions displayOptions)
        {
            if (displayOptions == null || !ModelState.IsValid || !AlarmDisplayRange.Validate(displayOptions.DisplayRanges))
            {
                throw new HttpResponseException(HttpResponses.BadRequestMessage);
            }

            SaveAlarmDisplayOptions(BuildingSecurityUser, displayOptions.DisplayRanges.ToArray());
            return Resources.AlarmDisplayOptionsSaveCompletedMessage;
        }

        private IEnumerable<AlarmDisplayRange> GetAlarmDisplayOptions(IUser user)
        {
            Collection<AlarmDisplayRange> result;
            if (_buildingSecurityClient.TryReadApplicationPreference(user.BuildingSecurityCookie, ApplicationSettings.AlarmDisplayOptions, out result))
            {
                return result;
            }

            return DisplayOptionDefaults;
        }


        private void SaveAlarmDisplayOptions(IUser user, AlarmDisplayRange[] alarmDisplayOptions)
        {
            _buildingSecurityClient.SaveApplicationPreference(
                user.BuildingSecurityCookie, ApplicationSettings.AlarmDisplayOptions,
                alarmDisplayOptions);
        }

        private static IEnumerable<AlarmDisplayRange> DisplayOptionDefaults
        {
            get
            {
                return new[]
                       {
                          new AlarmDisplayRange{Id = 0, Color = "#E3150E", LowerLimit = 0, UpperLimit = 0, AudioAlertId = new Guid("8191A7DE-CCA4-447F-AC2E-C3101438AEE3")},
                          new AlarmDisplayRange{Id = 1, Color = "#FE973F", LowerLimit = 1, UpperLimit = 2, AudioAlertId = new Guid("B76FFE24-C2B2-46F9-AE01-BCEC1C568D14")},
                          new AlarmDisplayRange{Id = 2, Color = "#F3F150", LowerLimit = 3, UpperLimit = 5, AudioAlertId = new Guid("64755EEE-379A-461F-B57B-F44F9DC61E40")},
                          new AlarmDisplayRange{Id = 3, Color = "", LowerLimit = 6, UpperLimit = 100, AudioAlertId = new Guid("64755EEE-379A-461F-B57B-F44F9DC61E40")},
                          new AlarmDisplayRange{Id = 4, Color = "", LowerLimit = 101, UpperLimit = 255, AudioAlertId = new Guid("64755EEE-379A-461F-B57B-F44F9DC61E40")}
                       };
            }
        }
    }
}
