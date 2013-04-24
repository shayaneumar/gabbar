/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App.Models
{
    public class ActiveAlarmsModel
    {
        private const string IsMultipleSelectEnabledKey = "active:alarms:multiselect:enabled";




        public string UserTimeZone { get; set; }

        public ActiveAlarmsModel() {}

        public ActiveAlarmsModel(string userTimeZone)
        {
            UserTimeZone = userTimeZone;
        }

        public static IEnumerable<TimeZoneIdAndDisplayName> TimeZones
        {
            get
            {
                var resourceManager = Globalization.TimeZones.ResourceManager;
                var timezones =  TimeZoneInfo.GetSystemTimeZones();

                return (from timeZone in timezones
                        let displayName = resourceManager.GetString(timeZone.Id) ?? timeZone.DisplayName
                        select new TimeZoneIdAndDisplayName(timeZone.Id, displayName));
            }
        }

        public static bool IsMultipleSelectEnabled
        {
            get
            {
                bool enabled;
                string enabledValue = ConfigurationManager.AppSettings[IsMultipleSelectEnabledKey];

                if(bool.TryParse(enabledValue, out enabled))
                {
                    return enabled;
                }

                return false;
            }
        }
    }
}
