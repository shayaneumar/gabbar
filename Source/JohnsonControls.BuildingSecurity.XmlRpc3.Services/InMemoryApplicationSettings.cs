/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SettingsDictionary : Dictionary<string, string>
    {//This class's sole purpose is to get rid fx cop warnings about nested generic types
        public SettingsDictionary()
        {}

        SettingsDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {}
    }

    public class InMemoryApplicationSettings : ITypedApplicationPreference
    {
        private readonly SettingsDictionary _applicationSettings;
        private readonly IDictionary<string, SettingsDictionary> _userSettings;
        private readonly object _lockingObject = new object();

        public InMemoryApplicationSettings(SettingsDictionary applicationSettings, IDictionary<string, SettingsDictionary> userSettings)
        {
            _applicationSettings = applicationSettings;
            _userSettings = userSettings;
        }

        public void ApplicationPreferenceSave(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType, string xmlApplicationPreferenceDoc)
        {
            lock (_lockingObject)
            {
                if (preferenceType == PreferenceType.User)
                {
                    if (!_userSettings.ContainsKey(userName))
                    {
                        _userSettings[userName] = new SettingsDictionary();
                    }
                    _userSettings[userName][applicationKey] = xmlApplicationPreferenceDoc;
                }
                if (preferenceType == PreferenceType.Application)
                {
                    _applicationSettings[applicationKey] = xmlApplicationPreferenceDoc;
                }
            }
        }

        public string ApplicationPreferenceRead(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType)
        {
            lock (_lockingObject)
            {
                if (preferenceType == PreferenceType.User )
                {
                    if (_userSettings.ContainsKey(userName) && _userSettings[userName].ContainsKey(applicationKey))
                    {
                        return _userSettings[userName][applicationKey];
                    }
                }

                if (preferenceType == PreferenceType.Application)
                {
                    if (_applicationSettings.ContainsKey(applicationKey))
                    {
                        return _applicationSettings[applicationKey];
                    }
                }

                throw new ServiceOperationException("No such setting");
            }
        }

        public void ApplicationPreferenceDelete(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType)
        {
            lock (_lockingObject)
            {
                if (preferenceType == PreferenceType.User)
                {
                    if (_userSettings.ContainsKey(userName) )
                    {
                        _userSettings[userName].Remove(applicationKey);
                    }
                }

                if (preferenceType == PreferenceType.Application)
                {
                    _applicationSettings.Remove(applicationKey);
                }
            }
        }
    }
}
