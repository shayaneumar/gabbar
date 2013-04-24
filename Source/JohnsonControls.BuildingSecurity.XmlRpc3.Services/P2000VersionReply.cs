/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public class P2000VersionReply
    {
        public string LastUpdated { get; set; }
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string BuildNumber { get; set; }
        public string RevisionNumber { get; set; }
    }
}
