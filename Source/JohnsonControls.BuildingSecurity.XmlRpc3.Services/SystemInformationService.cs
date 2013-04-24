/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.XmlRpc;
using JohnsonControls.Serialization.Xml;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Defines the methods described in the 4.2 System Information Functions section of the 3.x P2000 XML RPC documentation 
    /// </summary>
    /// <remarks>All parameters for all members are required.  Any parameter value of null is
    /// invalid and will throw an <see cref="ArgumentNullException"/></remarks>
    public class SystemInformationService : ITypedSystemInformationService
    {
        private readonly P2000XmlRpcProxy<ISystemInformationService> _systemInformationProxy;

        public SystemInformationService(Uri serviceUrl)
        {
            if (serviceUrl == null) throw new ArgumentNullException("serviceUrl");

            _systemInformationProxy = new P2000XmlRpcProxy<ISystemInformationService>(serviceUrl);
        }

        public P2000GetSystemInfoReply P2000GetSystemInfo(string userName, string sessionGuid)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");

            P2000ReturnStructure responseStructure =
                _systemInformationProxy.Invoke(proxy => proxy.P2000GetSystemInfo(userName, sessionGuid));

            return XmlSerializer<P2000GetSystemInfoReplyWrapper>.Instance.Deserialize(responseStructure.XmlDoc).P2000GetSystemInfoReply;
        }

        public P2000VersionReply P2000GetVersionEx()
        {
            P2000ReturnStructure responseStructure =
                _systemInformationProxy.Invoke(proxy => proxy.P2000GetVersionEx());

            return XmlSerializer<P2000VersionReplyWrapper>.Instance.Deserialize(responseStructure.XmlDoc).P2000VersionReply;
        }
    }
}
