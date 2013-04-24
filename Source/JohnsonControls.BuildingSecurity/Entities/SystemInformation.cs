/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    public class SystemInformation
    {
        public SystemInformation(Guid enterpriseSiteId, string enterpriseSiteName, int realTimeServicePort, int sessionHeartbeatInterval, string localeName)
        {
            EnterpriseSiteId = enterpriseSiteId;
            EnterpriseSiteName = enterpriseSiteName;
            RealTimeServicePort = realTimeServicePort;
            SessionHeartbeatInterval = sessionHeartbeatInterval;
            LocaleName = localeName;
        }

        public Guid EnterpriseSiteId { get; private set; }
        public string EnterpriseSiteName { get; private set; }
        public int RealTimeServicePort { get; private set; }
        public int SessionHeartbeatInterval { get; private set; }
        public string LocaleName { get; private set; }
    }
}
