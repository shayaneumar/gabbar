/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Extends the <see cref="ISystemInformationService"/> with strongly typed versions of the
    /// 3.12 services.
    /// </summary>
    /// <remarks>
    /// The pattern for the additional services defined for 3.12 was to include 
    /// an input parameter of type string and a return value of type string. This
    /// string is expected to be an XML document of the schema specified in the 3.12
    /// RPC documentation. This API is not very useful for a developer. This interface
    /// enhances the API by providing strongly typed versions of the services and handles
    /// all of the XML serialization and deserialization.
    /// </remarks>
    public interface ITypedSystemInformationService
    {
        P2000GetSystemInfoReply P2000GetSystemInfo(string userName, string sessionGuid);

        P2000VersionReply P2000GetVersionEx();
    }
}
