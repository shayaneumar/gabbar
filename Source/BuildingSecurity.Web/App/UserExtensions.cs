/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App
{
    public static class UserExtensions
    {
        public static void PopulateViewBag(this IUser user, dynamic viewBag)
        {
            if(user != null)
            {
                viewBag.FullName = string.IsNullOrWhiteSpace(user.FullName) ? user.Name : user.FullName;
                viewBag.CanViewAlarmManager = user.HasPermission(PermissionNames.CanViewAlarmManager);
                viewBag.AlarmManagerTarget = "_self";

                viewBag.CanViewReports = user.HasPermission(PermissionNames.CanViewReports);
                viewBag.ReportsTarget = viewBag.CanViewAlarmManager ? "reports" : "_self";

                viewBag.CanViewSystemSetup = user.HasPermission(PermissionNames.CanEditSystemSettings);
                viewBag.SystemSetupTarget = (viewBag.CanViewAlarmManager || viewBag.CanViewReports) ? "systemSetup" : "_self";

                viewBag.VersionP2000 = ConstructVersionNumber(user.VersionP2000);
            }
            else
            {
                viewBag.FullName = string.Empty;
                viewBag.CanViewAlarmManager = false;
                viewBag.CanViewReports = false;
                viewBag.CanViewSystemSetup = false;
            }
        }

        private static string ConstructVersionNumber(Version version)
        {
            if (version != null)
            {
                return version.MajorVersion + "." + version.MinorVersion + "." +
                       version.BuildNumber + "." + version.RevisionNumber;
            }

            return null;
        }
    }
}
