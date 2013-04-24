/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Reporting
{
    public enum IncludeLink
    {
        Yes,
        No
    }

    public static class IncludeLinkExtensions
    {
        private const string TrueString = "true";
        private const string FalseString = "false";

        public static string SsrsText(this IncludeLink outputType)
        {
            switch (outputType)
            {
                case IncludeLink.Yes:
                    return TrueString;

                default:
                    return FalseString;
            }
        }
    }
}