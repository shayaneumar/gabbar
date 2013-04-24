/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    public class InMemoryUserRepository
    {
        private static readonly List<string> AllPermissions = new List<string>
                {
                    PermissionNames.CanViewWebUserInterface,
                    PermissionNames.CanViewAlarmManager,
                    PermissionNames.CanViewAlarmDisplayOptions,
                    PermissionNames.CanEditAlarmDisplayOptions,
                    PermissionNames.CanViewReports,
                    PermissionNames.CanViewReportsServerSettings,
                    PermissionNames.CanEditReportsServerSettings,
                    PermissionNames.CanRunReports,
                    PermissionNames.CanScheduleReports,
                    PermissionNames.CanViewScheduledReports,
                    PermissionNames.CanEditScheduledReports,
                    PermissionNames.CanDeleteScheduledReports,
                    PermissionNames.CanEditSystemSettings,
                    PermissionNames.CanRunOrScheduleReports,
                    PermissionNames.CanControlSimulation,
                    PermissionNames.CanViewCaseManager,
                };

        private static readonly Dictionary<string, IEnumerable<string>> Users = new Dictionary<string, IEnumerable<string>> {
            { @"NOALARMVIEW", AllPermissions.Except(new[] { PermissionNames.CanEditAlarmDisplayOptions, PermissionNames.CanViewAlarmDisplayOptions,PermissionNames.CanViewAlarmManager }) },
            { @"NOALARMCONFIG", AllPermissions.Except(new[] { PermissionNames.CanEditAlarmDisplayOptions,PermissionNames.CanViewAlarmDisplayOptions }) },
            { @"NOREPORTCONFIG", AllPermissions.Except(new[] { PermissionNames.CanEditReportsServerSettings,PermissionNames.CanViewReportsServerSettings }) },
            { @"NOSCHEDULEREPORT", AllPermissions.Except(new[] { PermissionNames.CanDeleteScheduledReports, PermissionNames.CanEditScheduledReports,PermissionNames.CanScheduleReports, PermissionNames.CanViewScheduledReports }) },
            { @"NORUNREPORT", AllPermissions.Except(new[] { PermissionNames.CanRunReports }) },
            { @"EDITSCHEDLE", AllPermissions.Except(new[] { PermissionNames.CanScheduleReports, PermissionNames.CanDeleteScheduledReports }) },
            { @"VIEWSCHEDULE", AllPermissions.Except(new[] { PermissionNames.CanScheduleReports, PermissionNames.CanDeleteScheduledReports, PermissionNames.CanEditScheduledReports }) },
            { @"ADDSCHEDULE", AllPermissions.Except(new[] { PermissionNames.CanDeleteScheduledReports }) },
            { @"NOWEB", Enumerable.Empty<string>()},
            { @"ALARM", AllPermissions.Where(x=>x.IndexOf("alarm", StringComparison.InvariantCultureIgnoreCase) >=0) },
            { @"REPORT", AllPermissions.Where(x=>x.IndexOf("report", StringComparison.InvariantCultureIgnoreCase) >=0) },
            { @"SETUP", new []{PermissionNames.CanViewWebUserInterface} },
            { @"NOCASEMANVIEW", AllPermissions.Except(new [] {PermissionNames.CanViewCaseManager})},
        };

        public IEnumerable<string> GetPermissionsOf(string username)
        {
            IEnumerable<string> result;
            return Users.TryGetValue(username.ToUpperInvariant(), out result) ? result : AllPermissions.ToList();
        }
    }
}
