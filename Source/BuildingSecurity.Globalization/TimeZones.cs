/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/


using System.Resources;

namespace BuildingSecurity.Globalization
{
    /// <summary>
    /// Provides a resource manager for looking up localized time zone descriptions.
    /// </summary>
    public static class TimeZones
    {
        public static ResourceManager ResourceManager
        {
            get { return new ResourceManager("BuildingSecurity.Globalization.TimeZones", typeof(TimeZones).Assembly); }
        }
    }
}
