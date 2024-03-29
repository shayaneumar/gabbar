﻿/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Reporting
{
    public interface IReportingClientFactory
    {
        IReportingClient CreateClient(IUser user);
        IReportingClient CreateClient(ReportServerConfiguration reportServerConfiguration);
    }
}
