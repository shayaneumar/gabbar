/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using BuildingSecurity.Globalization;

namespace BuildingSecurity.Web
{
    public static class CopyrightFactory
    {
        public static string Current
        {
            get
            {
                DateTime now = DateTime.Now;
                if(now.Year > 2012)
                {
                    return string.Format(CultureInfo.CurrentCulture, Resources.CopyrightClaimFormat, now.Year);
                }

                return Resources.CopyrightClaim2012;
            }
        }
    }
}
