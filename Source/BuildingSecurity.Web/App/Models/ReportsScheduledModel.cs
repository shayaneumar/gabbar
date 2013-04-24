/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using BuildingSecurity.Reporting.ReportingService;

namespace BuildingSecurity.Web.App.Models
{
    public class ReportsScheduledModel
    {
        public ReportsScheduledModel(IEnumerable<Subscription> scheduledReports)
        {
            ScheduledReports = scheduledReports.OrderBy(r => r.Report).ThenBy(r => r.Description);
        }

        public IEnumerable<Subscription> ScheduledReports { get; private set; }
    }
}
