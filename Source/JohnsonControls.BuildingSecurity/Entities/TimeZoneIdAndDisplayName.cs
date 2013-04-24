/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity
{
    public class TimeZoneIdAndDisplayName
    {
        public string Id { get; private set; }

        public string DisplayName { get; private set; }

        public TimeZoneIdAndDisplayName(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }
}
