/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity
{
    public class Version
    {
        public Version(string lastUpdated, string majorVersion, string minorVersion, string buildNumber, string revisionNumber)
        {
            LastUpdated = lastUpdated;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            BuildNumber = buildNumber;
            RevisionNumber = revisionNumber;
        }

        public string LastUpdated { get; private set; }
        public string MajorVersion { get; private set; }
        public string MinorVersion { get; private set; }
        public string BuildNumber { get; private set; }
        public string RevisionNumber { get; private set; }
    }
}
