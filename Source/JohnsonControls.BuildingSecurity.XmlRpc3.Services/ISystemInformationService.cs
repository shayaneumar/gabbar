/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using CookComputing.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Service used to retrieve system specific information from the P2000
    /// </summary>
    public interface ISystemInformationService
    {
        [XmlRpcMethod("P2000GetSystemInfo")]
        P2000ReturnStructure P2000GetSystemInfo(string userName, string sessionGuid);

        [XmlRpcMethod("P2000GetVersionEx")]
        P2000ReturnStructure P2000GetVersionEx();
    }
}
