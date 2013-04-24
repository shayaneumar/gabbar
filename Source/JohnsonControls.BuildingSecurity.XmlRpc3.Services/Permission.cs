/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Globalization;
using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public class Permission
    {
        private const string WebUserInterfaceResourceIdentifier = "30001";
        private const string AlarmManagerResourceIdentifier = "30002";
        private const string AlarmManagerSettingsResourceIdentifier = "30003";
        private const string ReportsResourceIdentifier = "30006";
        private const string ReportIntegrationSettingsResourceIdentifier = "30007";
        private const string ScheduleReportsResourceIdentifier = "30008";
        private const string RunReportResourceIdentifier = "30009";
        private const string SiteSettingsResourceIdentifier = "30010";

        private const int ViewPermissionMinimum = 256;
        private const int EditPermissionMinimum = 512;
        private const int AddPermissionMinimum = 1024;
        private const int DeletePermissionMinimum = 2048;

        public string ResourceKey { get; set; }

        public string PermissionLevel { get; set; }

        private bool _permissionLevelSet;
        private int _permissionLevel;

        /// <summary>
        /// Indicates if this represents a view permission of a resource.
        /// </summary>
        private bool IsViewPermission
        {
            get
            {
                if (!_permissionLevelSet)
                {
                    _permissionLevel = int.Parse(PermissionLevel, CultureInfo.InvariantCulture);
                    _permissionLevelSet = true;
                }

                return _permissionLevel >= ViewPermissionMinimum;
            }
        }

        /// <summary>
        /// Indicates if this represents a edit permission of a resource.
        /// </summary>
        private bool IsEditPermission
        {
            get
            {
                if (!_permissionLevelSet)
                {
                    _permissionLevel = int.Parse(PermissionLevel, CultureInfo.InvariantCulture);
                    _permissionLevelSet = true;
                }

                return _permissionLevel >= EditPermissionMinimum;
            }
        }

        /// <summary>
        /// Indicates if this represents a add permission of a resource.
        /// </summary>
        private bool IsAddPermission
        {
            get
            {
                if (!_permissionLevelSet)
                {
                    _permissionLevel = int.Parse(PermissionLevel, CultureInfo.InvariantCulture);
                    _permissionLevelSet = true;
                }

                return _permissionLevel >= AddPermissionMinimum;
            }
        }

        /// <summary>
        /// Indicates if this represents a delete permission of a resource.
        /// </summary>
        private bool IsDeletePermission
        {
            get
            {
                if (!_permissionLevelSet)
                {
                    _permissionLevel = int.Parse(PermissionLevel, CultureInfo.InvariantCulture);
                    _permissionLevelSet = true;
                }

                return _permissionLevel >= DeletePermissionMinimum;
            }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view the web application user interface.  Without this permission
        /// a user cannot log in successfully.
        /// </summary>
        [XmlIgnore]
        public bool IsViewWebUserInterfacePermission
        {
            get { return ResourceKey == WebUserInterfaceResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view the alarm manager.
        /// </summary>
        [XmlIgnore]
        public bool IsViewAlarmManagerPermission
        {
            get { return ResourceKey == AlarmManagerResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view the alarm manager settings.
        /// </summary>
        [XmlIgnore]
        public bool IsViewAlarmManagerSettingsPermission
        {
            get { return ResourceKey == AlarmManagerSettingsResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// edit the alarm manager settings.
        /// </summary>
        [XmlIgnore]
        public bool IsEditAlarmManagerSettingsPermission
        {
            get { return ResourceKey == AlarmManagerSettingsResourceIdentifier && IsEditPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view reports.
        /// </summary>
        [XmlIgnore]
        public bool IsViewReportsPermission
        {
            get { return ResourceKey == ReportsResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view the reports server settings.
        /// </summary>
        [XmlIgnore]
        public bool IsViewReportServerSettingsPermission
        {
            get { return ResourceKey == ReportIntegrationSettingsResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// edit the report server settings.
        /// </summary>
        [XmlIgnore]
        public bool IsEditReportServerSettingsPermission
        {
            get { return ResourceKey == ReportIntegrationSettingsResourceIdentifier && IsEditPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view the report scheduler page.
        /// </summary>
        [XmlIgnore]
        public bool IsViewScheduleReportsPermission
        {
            get { return ResourceKey == ScheduleReportsResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// edit the report scheduler page.
        /// </summary>
        [XmlIgnore]
        public bool IsEditScheduleReportsPermission
        {
            get { return ResourceKey == ScheduleReportsResourceIdentifier && IsEditPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// add a newly scheduled report.
        /// </summary>
        [XmlIgnore]
        public bool IsAddScheduleReportsPermission
        {
            get { return ResourceKey == ScheduleReportsResourceIdentifier && IsAddPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// delete an existing scheduled report.
        /// </summary>
        [XmlIgnore]
        public bool IsDeleteScheduleReportsPermission
        {
            get { return ResourceKey == ScheduleReportsResourceIdentifier && IsDeletePermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view an on-demand report.
        /// </summary>
        [XmlIgnore]
        public bool IsViewRunReportsPermission
        {
            get { return ResourceKey == RunReportResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// view the site settings.
        /// </summary>
        [XmlIgnore]
        public bool IsViewSiteSettingsPermission
        {
            get { return ResourceKey == SiteSettingsResourceIdentifier && IsViewPermission; }
        }

        /// <summary>
        /// Indicates if this instance represents a permission to
        /// edit the site settings.
        /// </summary>
        [XmlIgnore]
        public bool IsEditSiteSettingsPermission
        {
            get { return ResourceKey == SiteSettingsResourceIdentifier && IsEditPermission; }
        }

    }
}