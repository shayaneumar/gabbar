/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity
{
    [DataContract]
    public class UserPreferences : IUserPreferences
    {
        public UserPreferences()
        {
        }

        public UserPreferences(string selectedTimeZone)
        {
            SelectedTimeZone = selectedTimeZone;
        }

        [DataMember]
        public string SelectedTimeZone { get; private set; }

        [IgnoreDataMember]
        public TimeZoneInfo SelectedTimeZoneInfo
        {
            get
            {
                if(string.IsNullOrWhiteSpace(SelectedTimeZone))return TimeZoneInfo.Local;
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(SelectedTimeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.Local;
                }
            }
        }
    }
}
