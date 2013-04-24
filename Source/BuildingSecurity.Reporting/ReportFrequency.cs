/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Diagnostics;
using BuildingSecurity.Globalization;

namespace BuildingSecurity.Reporting
{
    public enum ReportFrequency
    {
        Daily,
        Weekly
    }

    public static class ReportFrequencyTypeExtensions
    {
        public static string Label(this ReportFrequency outputType)
        {
            switch (outputType)
            {
                case ReportFrequency.Daily:
                    return Resources.ReportFrequencyDailyLabel;

                case ReportFrequency.Weekly:
                    return Resources.ReportFrequencyWeeklyLabel;

                default:
                    Debugger.Break(); // If you end up here, this switch statement may need updating
                    break;
            }

            return Resources.ReportFrequencyDailyLabel;
        }
    }
}
