/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Drawing.Imaging;
using System.Threading;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests
{
    public static class WebBrowser
    {

        public static Browser CurrentBrowser {
            get
            {
                if (!FeatureContext.Current.ContainsKey("browser") || (FeatureContext.Current["browser"] as Browser == null))
                {
                    if (!FeatureContext.Current.ContainsKey("manager") || FeatureContext.Current["manager"] as Manager == null)
                    {
                        FeatureContext.Current["manager"] = new Manager(BaseTest.GetSettings());
                    }

                    var browserManager = (Manager)FeatureContext.Current["manager"];
                    browserManager.LaunchNewBrowser(EnvironmentConfiguration.Browser);
                    FeatureContext.Current["browser"] = browserManager.ActiveBrowser;
                }

                return (Browser)FeatureContext.Current["browser"];
            }
        }

        public static void TakeScreenShot(string saveTo)
        {
            var img = CurrentBrowser.Window.GetBitmap();
            img.Save(saveTo,ImageFormat.Png);
        }

        public static void CleanUp()
        {
            /*
             * HACK: at present the telerik testing framework does not delete cookies with the secure flag set.
             * We use this flag because we are an https site.
            */
            if (EnvironmentConfiguration.WebUiAddress.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                CurrentBrowser.NavigateTo(EnvironmentConfiguration.WebUiAddress + "/Account/LogOff");
            }
            CurrentBrowser.Cookies.DeleteCookie(EnvironmentConfiguration.WebServer);
        }

        public static void Close()
        {
            CurrentBrowser.Close();
        }

        public static void NavigateTo(string url)
        {
            CurrentBrowser.NavigateTo(url);
            Thread.Sleep(250);//The browser automation framework has a race condition with navigation and WaitUntilReady
            CurrentBrowser.WaitUntilReady();
        }

        public static void CloseBrowserManager()
        {
            if (!FeatureContext.Current.ContainsKey("manager") && FeatureContext.Current["manager"] as Manager != null)
            {
                ((Manager)FeatureContext.Current["manager"]).Dispose();
                FeatureContext.Current.Remove("manager");
            }
        }
    }
}
