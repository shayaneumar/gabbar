/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using JohnsonControls.Runtime.Caching;
using JohnsonControls.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services.Caching
{
    public class CachingApplicationPreferences : ITypedApplicationPreference
    {
        private readonly ITypedApplicationPreference _preferences;
        private readonly ICache _cache;

        public CachingApplicationPreferences(ITypedApplicationPreference preferences, ICache cache)
        {
            _preferences = preferences;
            _cache = cache;
        }

        public void ApplicationPreferenceSave(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType, string xmlApplicationPreferenceDoc)
        {
            _cache.Invalidate(GetKey(userName, applicationKey, preferenceType));
            _preferences.ApplicationPreferenceSave(userName, sessionGuid, applicationKey, preferenceType, xmlApplicationPreferenceDoc);
        }

        private static string GetKey(string userName, string applicationKey, PreferenceType preferenceType)
        {
            if(preferenceType == PreferenceType.User)
            {
                return userName + '♦' + applicationKey;
            }
            return applicationKey;
        }

        public string ApplicationPreferenceRead(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType)
        {
            return _cache.RetrieveOrAdd(GetKey(userName, applicationKey, preferenceType),
                                        () => new StringTransferObject(_preferences.ApplicationPreferenceRead(userName, sessionGuid, applicationKey, preferenceType)),
                                        DateTimeOffset.UtcNow.AddDays(1)).Value;
        }
        public void ApplicationPreferenceDelete(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType)
        {
            _cache.Invalidate(GetKey(userName,applicationKey,preferenceType));
            _preferences.ApplicationPreferenceDelete(userName, sessionGuid, applicationKey, preferenceType);
        }
    }
}
