/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
namespace JohnsonControls.BuildingSecurity
{
    public static class PermissionNames
    {
        public const string CanViewWebUserInterface =       "CanViewWebUserInterface";
        public const string CanViewAlarmManager =           "CanViewAlarmMonitor";

        public const string CanViewAlarmDisplayOptions =    "CanViewAlarmDisplayOptions";
        public const string CanEditAlarmDisplayOptions =    "CanEditAlarmDisplayOptions";

        public const string CanViewReports =                "CanViewReports";
        public const string CanViewReportsServerSettings =  "CanViewReportsServerSettings";
        public const string CanEditReportsServerSettings =  "CanEditReportsServerSettings";
        public const string CanRunReports =                 "CanRunReports";

        public const string CanScheduleReports =            "CanScheduleReports";
        public const string CanViewScheduledReports =       "CanViewScheduledReports";
        public const string CanEditScheduledReports =       "CanEditScheduledReports";
        public const string CanDeleteScheduledReports =     "CanDeleteScheduledReports";

        public const string CanEditSystemSettings =         "CanEditSystemSettings";

        public const string CanRunOrScheduleReports =       "CanRunReports, CanScheduleReports";

        public const string CanViewCaseManager =            "CanViewCaseManager";

        public const string CanControlSimulation =          "CanControlSimulation";
    }
}