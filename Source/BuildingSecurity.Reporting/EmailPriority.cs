/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Diagnostics;

namespace BuildingSecurity.Reporting
{
    public enum EmailPriority
    {
        Low,
        Normal,
        High
    }

    public static class EmailPriorityExtensions
    {
        private const string LowString = "LOW";
        private const string NormalString = "NORMAL";
        private const string HighString = "HIGH";

        public static string SsrsText(this EmailPriority outputType)
        {
            switch (outputType)
            {
                case EmailPriority.Low:
                    return LowString;

                case EmailPriority.Normal:
                    return NormalString;

                case EmailPriority.High:
                    return HighString;

                default:
                    Debugger.Break(); // If you end up here, this switch statement may need updating
                    break;
            }
            return NormalString;
        }
    }
}