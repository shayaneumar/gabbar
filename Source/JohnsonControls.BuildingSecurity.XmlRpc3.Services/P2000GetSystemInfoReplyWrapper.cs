/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// P2000Reply container class for P2000GetSystemInfoReply
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    [XmlRoot("P2000Reply")]
    public class P2000GetSystemInfoReplyWrapper
    {
        /// <summary>
        /// Reply object returned from
        /// <see cref="ITypedSystemInformationService.P2000GetSystemInfo(string, string)"/>.
        /// </summary>
        public P2000GetSystemInfoReply P2000GetSystemInfoReply { get; set; }
    }
}
