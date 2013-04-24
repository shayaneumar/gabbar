/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Reporting
{
    public enum IncludeReport
    {
        Yes,
        No
    }

    public static class IncludeReportExtensions
    {
        private const string TrueString = "true";
        private const string FalseString = "false";

        public static string SsrsText(this IncludeReport outputType)
        {
            switch (outputType)
            {
                case IncludeReport.Yes:
                    return TrueString;

                default:
                    return FalseString;
            }
        }
    }
}