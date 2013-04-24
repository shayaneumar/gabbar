/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.IO;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests
{
    [Binding]
    public class Events
    {
        private static readonly Guid RunId = Guid.NewGuid();

        [BeforeStep]
        public void BeforeStep()
        {
        }

        [AfterStep]
        public void AfterStep()
        {
            if (ScenarioContext.Current.TestError != null)//Take a screenshot if there was an issue
            {
                var logPath = ".\\SpecFlowTestLog\\" + RunId+"\\";
                Directory.CreateDirectory(logPath);
                WebBrowser.TakeScreenShot(logPath+DateTime.Now.ToString("HH.mm.ss.ffff")+".png");
            }
        }

        [BeforeScenarioBlock]
        public void BeforeScenarioBlock()
        {
        }

        [AfterScenarioBlock]
        public void AfterScenarioBlock()
        {
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
        }

        [AfterScenario]
        public void AfterScenario()
        {
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {
        }

        [AfterFeature]
        public static void AfterFeature()
        {
            WebBrowser.Close();
            WebBrowser.CloseBrowserManager();
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
        }
    }
}
