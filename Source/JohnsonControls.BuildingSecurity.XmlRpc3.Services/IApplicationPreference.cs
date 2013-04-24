/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using CookComputing.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public interface IApplicationPreference
    {
        [XmlRpcMethod("ApplicationPreferenceSave")]
        void ApplicationPreferenceSave(string userName, string sessionGuid, string applicationKey,string preferenceType, string xmlApplicationPreferenceDoc);

        [XmlRpcMethod("ApplicationPreferenceRead")]
        P2000ReturnStructure ApplicationPreferenceRead(string userName, string sessionGuid, string applicationKey, string preferenceType);

        [XmlRpcMethod("ApplicationPreferenceDelete")]
        void ApplicationPreferenceDelete(string userName, string sessionGuid, string applicationKey, string preferenceType);
    }
}
