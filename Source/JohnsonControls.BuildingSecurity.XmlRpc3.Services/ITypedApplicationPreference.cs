/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public interface ITypedApplicationPreference
    {
        void ApplicationPreferenceSave(string userName, string sessionGuid,  string applicationKey, PreferenceType preferenceType,string xmlApplicationPreferenceDoc);

        string ApplicationPreferenceRead(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType);

        void ApplicationPreferenceDelete(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType);
    }
}
