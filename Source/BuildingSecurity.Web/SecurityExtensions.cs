/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using JohnsonControls.Diagnostics;

namespace BuildingSecurity.Web
{
    public static class SecurityExtensions
    {
        public static string EnsureHttpAbsoluteUrl(this string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (!url.IsValidHttpAbsoluteUrl())
            {
                Log.Error("Potential cross site scripting attack detected. Attempting to thwart attack.");
                return "";
            }

            return url;
        }

        public static bool IsValidHttpAbsoluteUrl(this string maybeUrl)
        {
            Uri uri;
            if (Uri.TryCreate(maybeUrl, UriKind.Absolute, out uri))
            {
                if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) || uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
