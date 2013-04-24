/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{

    public class P2000LoginReply
    {
        public SessionInfo SessionInfo { get; set; }
        //TODO: Follow the suggestion of demeter and move things like partitions up onto this object.
        //And/or consider returning a different object type from ITypedSessionManagement.P2000Login(...)
        public UserDetails UserDetails { get; set; }

        [XmlIgnore]
        public bool CanViewWebUserInterface { get { return UserDetails != null && UserDetails.CanViewWebUserInterface; } }

        [XmlIgnore]
        public bool CanViewSiteSettings { get { return UserDetails != null && UserDetails.CanViewSiteSettings; } }

        [XmlIgnore]
        public bool CanEditSiteSettings { get { return UserDetails != null && UserDetails.CanEditSiteSettings; } }

        /// <summary>
        /// Indicates if the user that this reply applies to has permission to view Alarm Manager.
        /// </summary>
        [XmlIgnore]
        public bool CanViewAlarmManager { get { return UserDetails != null && UserDetails.CanViewAlarmManager; } }

        /// <summary>
        /// Indicates if the user that this reply applies to has permission to view reports.
        /// </summary>
        [XmlIgnore]
        public bool CanViewAlarmManagerSettings { get { return UserDetails != null && UserDetails.CanViewAlarmManagerSettings; } }

        [XmlIgnore]
        public bool CanEditAlarmManagerSettings { get { return UserDetails != null && UserDetails.CanEditAlarmManagerSettings; } }

        [XmlIgnore]
        public bool CanViewReportsServerSettings { get { return UserDetails != null && UserDetails.CanViewReportsServerSettings; } }

        [XmlIgnore]
        public bool CanEditReportsServerSettings { get { return UserDetails != null && UserDetails.CanEditReportsServerSettings; } }

        [XmlIgnore]
        public bool CanViewReports { get { return UserDetails != null && UserDetails.CanViewReports; } }

        [XmlIgnore]
        public bool CanViewRunReports { get { return UserDetails != null && UserDetails.CanViewRunReports; } }

        [XmlIgnore]
        public bool CanViewScheduleReports { get { return UserDetails != null && UserDetails.CanViewScheduleReports; } }

        [XmlIgnore]
        public bool CanEditScheduleReports { get { return UserDetails != null && UserDetails.CanEditScheduleReports; } }

        [XmlIgnore]
        public bool CanAddScheduleReports { get { return UserDetails != null && UserDetails.CanAddScheduleReports; } }

        [XmlIgnore]
        public bool CanDeleteScheduleReports { get { return UserDetails != null && UserDetails.CanDeleteScheduleReports; } }
    }
}