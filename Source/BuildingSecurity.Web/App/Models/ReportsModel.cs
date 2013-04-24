/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using BuildingSecurity.Reporting;
using BuildingSecurity.Web.App.Controllers;
using BuildingSecurity.Globalization;

namespace BuildingSecurity.Web.App.Models
{
    public class ReportsModel
    {
        private const string RunActionName = "Runner";
        private const string ScheduleActionName = "Scheduler";

        public ReportsModel(IEnumerable<ReportInfo> cannedReports, IEnumerable<ReportInfo> customReports, ReportListAction listAction)
        {
            CannedReports = cannedReports.OrderBy(r => r.Name);
            CustomReports = customReports.OrderBy(r => r.Name);
            ReportListAction = listAction;
        }

        public IEnumerable<ReportInfo> CannedReports { get; private set; }
        public IEnumerable<ReportInfo> CustomReports { get; private set; }
        public ReportListAction ReportListAction { get; private set; }

        public string Title
        {
            get
            {
                return IsRunListAction
                           ? Resources.RunReportsIndexViewPageTitle
                           : Resources.ScheduleReportsIndexViewPageTitle;
            } 
        }

        public static string HelpLink
        {
            get
            {
                return "~/Help/index_csh.htm#1002,withnavpane=true";
            }
        }

        public string PageHeader
        {
            get
            {
                return Title;
            }
        }

        public string LinkAction
        {
            get
            {
                return IsRunListAction ? RunActionName : ScheduleActionName;
            }
        }

        private bool IsRunListAction
        {
            get { return ReportListAction == ReportListAction.Run; }
        }
    }
}
