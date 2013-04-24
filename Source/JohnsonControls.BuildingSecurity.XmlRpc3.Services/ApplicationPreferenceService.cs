/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Service that allows the Saving, Reading, and Deleting of user preferences in the P2000 server.
    /// </summary>
    public class ApplicationPreferenceService : ITypedApplicationPreference
    {
        private readonly P2000XmlRpcProxy<IApplicationPreference> _applicationPreferenceProxy;
        private const uint SettingStorageSize = 512998;
        private const uint MaxKeySize = 49;
        private static readonly IDictionary<PreferenceType,string> PreferenceTypeMap = new Dictionary<PreferenceType, string>
                                                                                  {
                                                                                      {PreferenceType.User, "1"},
                                                                                      {PreferenceType.Application, "4"},
                                                                                  };
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionManagementService"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        public ApplicationPreferenceService(Uri serviceUrl)
        {
            _applicationPreferenceProxy = new P2000XmlRpcProxy<IApplicationPreference>(serviceUrl);
        }

        public void ApplicationPreferenceSave(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType, string xmlApplicationPreferenceDoc)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (applicationKey == null) throw new ArgumentNullException("applicationKey");
            if (xmlApplicationPreferenceDoc == null) throw new ArgumentNullException("xmlApplicationPreferenceDoc");
            if (applicationKey.Length >= MaxKeySize) throw new ArgumentException("Too large", "applicationKey");
            if (xmlApplicationPreferenceDoc.Length > SettingStorageSize) throw new ArgumentException("Too large", "xmlApplicationPreferenceDoc");//The Length property returns the number of Char objects in this instance, not the number of Unicode characters

            _applicationPreferenceProxy.Invoke(proxy => proxy.ApplicationPreferenceSave(userName, sessionGuid, applicationKey, PreferenceTypeMap[preferenceType], xmlApplicationPreferenceDoc));
        }

        public string ApplicationPreferenceRead(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (applicationKey == null) throw new ArgumentNullException("applicationKey");
            if (applicationKey.Length >= MaxKeySize) throw new ArgumentException("Too large", "applicationKey");

            try
            {
                return _applicationPreferenceProxy.Invoke(proxy => proxy.ApplicationPreferenceRead(userName, sessionGuid, applicationKey, PreferenceTypeMap[preferenceType])).XmlDoc;
            }
            catch (ServiceOperationException e)
            {
                if (e.FaultCode != XmlRpcFaultCodes.ApplicationReferenceNotFound) throw;
                return null;
            }
        }

        public void ApplicationPreferenceDelete(string userName, string sessionGuid, string applicationKey, PreferenceType preferenceType)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (applicationKey == null) throw new ArgumentNullException("applicationKey");
            if (applicationKey.Length >= MaxKeySize) throw new ArgumentException("Too large", "applicationKey");

            try
            {
                _applicationPreferenceProxy.Invoke(proxy => proxy.ApplicationPreferenceDelete(userName, sessionGuid, applicationKey, PreferenceTypeMap[preferenceType]));
            }
            catch (ServiceOperationException e)
            {
                if (e.FaultCode != XmlRpcFaultCodes.ApplicationReferenceNotFound) throw;
            }
        }
    }
}
