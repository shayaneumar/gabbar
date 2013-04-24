/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web.Http;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.TimeZone;

namespace BuildingSecurity.Web.Api.Controllers
{
    [RequiredPermission(PermissionNames.CanViewAlarmManager)]
    public class UserPreferencesController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public UserPreferencesController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient) : base(sessionStore)
        {
            if (buildingSecurityClient == null) throw new ArgumentNullException("buildingSecurityClient");
            _buildingSecurityClient = buildingSecurityClient;
        }

        [RequiredPermission(PermissionNames.CanEditAlarmDisplayOptions)]
        public void Post(UserPreferencesInput userPreferencesInput)
        {
            if (userPreferencesInput == null) throw new HttpResponseException(HttpResponses.BadRequestMessage);

            switch (userPreferencesInput.PreferenceKey)
            {
                case PreferenceKey.SelectedTimeZone:
                    if (userPreferencesInput.PreferenceValue.IsValidTimeZone())
                    {
                         _buildingSecurityClient.SaveUserPreference(BuildingSecurityUser.BuildingSecurityCookie, UserSettings.UserTimeZone, new UserPreferences(userPreferencesInput.PreferenceValue));
                    }
                    else
                    {
                        throw new HttpResponseException(HttpResponses.BadRequestMessage);
                    }
                    break;

                default:
                    throw new HttpResponseException(HttpResponses.BadRequestMessage);
            }
        }
    }
}
