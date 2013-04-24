/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Xml.Serialization;
using System.Linq;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public class UserDetails
    {
        public UserDetails()
        {
            Partitions = new Partition[0];
            Permissions = new Permission[0];
        }

        public string UserName { get; set; }

        public int UserType { get; set; }

        public string UserFullName { get; set; }

        public string ProfileName { get; set; }

        public string UserId { get; set; }

        public string UserGuid { get; set; }

        public Partition[] Partitions { get; set; }

        public Permission[] Permissions { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance can view web user interface.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view web user interface; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewWebUserInterface
        {
            get { return HasPermissionSet(p => p.IsViewWebUserInterfacePermission); }
        }

        /// <summary>
        /// Indicates if this user has permission to view the alarm monitor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view alarm monitor; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewAlarmManager
        {
            get { return HasPermissionSet(p => p.IsViewAlarmManagerPermission); }
        }

        /// <summary>
        /// Indicates if this user has permission to view the alarm monitor settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view alarm monitor settings; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewAlarmManagerSettings
        {
            get { return HasPermissionSet(p => p.IsViewAlarmManagerSettingsPermission); }
        }

        /// <summary>
        /// Indicates if this user has permission to edit the alarm monitor settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit alarm monitor settings; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanEditAlarmManagerSettings
        {
            get { return HasPermissionSet(p => p.IsEditAlarmManagerSettingsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can view reports.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view reports; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewReports
        {
            get { return HasPermissionSet(p => p.IsViewReportsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can view reports server settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view reports server settings; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewReportsServerSettings
        {
            get { return HasPermissionSet(p => p.IsViewReportServerSettingsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can edit reports server settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit reports server settings; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanEditReportsServerSettings
        {
            get { return HasPermissionSet(p => p.IsEditReportServerSettingsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can view site settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view site settings; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewSiteSettings
        {
            get { return HasPermissionSet(p => p.IsViewSiteSettingsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can edit site settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit site settings; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanEditSiteSettings
        {
            get { return HasPermissionSet(p => p.IsEditSiteSettingsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can run reports.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can run reports; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewRunReports
        {
            get { return HasPermissionSet(p => p.IsViewRunReportsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can view scheduled reports.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can view schedule reports; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanViewScheduleReports
        {
            get { return HasPermissionSet(p => p.IsViewScheduleReportsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can edit scheduled reports.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit schedule reports; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanEditScheduleReports
        {
            get { return HasPermissionSet(p => p.IsEditScheduleReportsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can add a new scheduled report.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can add schedule reports; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanAddScheduleReports
        {
            get { return HasPermissionSet(p => p.IsAddScheduleReportsPermission); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can delete scheduled reports.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete schedule reports; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool CanDeleteScheduleReports
        {
            get { return HasPermissionSet(p => p.IsDeleteScheduleReportsPermission); }
        }

        private bool HasPermissionSet(Func<Permission, bool> predicate)
        {
            return Permissions != null && Permissions.Any(predicate);
        }
    }
}