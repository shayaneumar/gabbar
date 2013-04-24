/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.TimeZone
{
    public static class TimeZoneExtensions
    {
        public static bool IsValidTimeZone(this string timezone)
        {
            if (string.IsNullOrWhiteSpace(timezone)) return false;
            try //Ensure is valid timezone
            {
                TimeZoneInfo.FindSystemTimeZoneById(timezone);
                return true;
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException) // The id is valid but the time zone info is corrupted.
            {
            }

            return false;
        }
    }
}
