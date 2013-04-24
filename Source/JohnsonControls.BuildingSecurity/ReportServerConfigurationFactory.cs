/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    public class ReportServerConfigurationFactory : IReportServerConfigurationFactory
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public ReportServerConfigurationFactory(IBuildingSecurityClient buildingSecurityClient)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        public ReportServerConfiguration GetConfiguration(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            ReportServerConfiguration result;
            if (_buildingSecurityClient.TryReadApplicationPreference(user.BuildingSecurityCookie, ApplicationSettings.ReportServerConfiguration, out result))
            {
                return result;
            }

            return new ReportServerConfiguration("", "", "", "");
        }
    }
}