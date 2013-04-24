/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Runtime.Serialization;

namespace BuildingSecurity.Web.Api.Models
{
    [DataContract]
    public class UserPreferencesInput
    {
        [DataMember(Name = "preferenceKey")]
        public PreferenceKey PreferenceKey { get; private set; }

        [DataMember(Name = "preferenceValue")]
        public string PreferenceValue { get; private set; }

        public UserPreferencesInput(PreferenceKey preferenceKey, string preferenceValue)
        {
            PreferenceKey = preferenceKey;
            PreferenceValue = preferenceValue;
        }
    }
}
