/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Diagnostics;
using BuildingSecurity.Globalization;

namespace BuildingSecurity.Reporting
{
    public enum ReportDestination
    {
        FileShare,
        Email
    }

    public static class ReportDestinationTypeExtensions
    {
        public static string Label(this ReportDestination outputType)
        {
            switch (outputType)
            {
                case ReportDestination.FileShare:
                    return Resources.ReportDestinationFileShareLabel;

                case ReportDestination.Email:
                    return Resources.ReportDestinationEmailLabel;

                default:
                    Debugger.Break(); // If you end up here, this switch statement may need updating
                    break;
            }

            return Resources.ReportDestinationFileShareLabel;
        }
    }
}
