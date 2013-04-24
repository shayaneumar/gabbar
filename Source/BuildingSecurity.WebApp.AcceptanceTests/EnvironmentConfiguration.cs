/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Configuration;
using ArtOfTest.WebAii.Core;

namespace BuildingSecurity.WebApp.AcceptanceTests
{
    public static class EnvironmentConfiguration
    {
        static EnvironmentConfiguration()
        {
            var configuredAddress = ConfigurationManager.AppSettings["WebUiAddress"];
            var configuredBrowser = ConfigurationManager.AppSettings["Browser"];

            BrowserType browser;
            if(!Enum.TryParse(configuredBrowser, true, out browser))
            {
                browser = BrowserType.Chrome;
            }

            Uri address;
            if (!Uri.TryCreate(configuredAddress, UriKind.Absolute, out address))
            {
                address = new Uri("http://localhost:18223");
            }

            WebUiAddress = address.ToString().TrimEnd('/');
            WebServer = address.Host;
            Browser = browser;
        }


        public static string WebUiAddress { get; private set; }
        public static string WebServer { get; private set; }
        public static BrowserType Browser { get; private set; }
    }
}
