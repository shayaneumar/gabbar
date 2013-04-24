/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Configuration;

namespace JohnsonControls.BuildingSecurity
{
    /*
     * This file should contain nothing but constants, however since we want to be able to
     * change these 'constants at runtime, we had to make them properties instead.
    */
    public static class ApplicationSettings
    {
         private static readonly string DefaultNamespace = (ConfigurationManager.AppSettings["P2000SettingPrefix"]??"")+"JCI.WEBUI.";

        public static string ReportServerConfiguration {get { return DefaultNamespace + "REPORTS.CONFIG"; }}
        public static string ClientLogo                {get { return DefaultNamespace + "CLIENTLOGO"; }}
        public static string TemporaryClientLogo       {get { return DefaultNamespace + "TEMPCLIENTLOGO"; }}
        public static string AlarmDisplayOptions       {get { return DefaultNamespace + "ALARM.DISPLAYOPT";}}
    }

    public static class UserSettings
    {
        private static readonly string DefaultNamespace = (ConfigurationManager.AppSettings["P2000SettingPrefix"] ?? "") + "JCI.WEBUI.";

        public static string UserTimeZone {get { return DefaultNamespace + "USER.TIMEZONE"; }}
    }
}
