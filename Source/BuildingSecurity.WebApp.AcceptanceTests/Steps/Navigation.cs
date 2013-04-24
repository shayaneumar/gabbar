/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class Navigation
    {
        private readonly string _baseSite = EnvironmentConfiguration.WebUiAddress;
        private static readonly Dictionary<string, string> PageNameToUrl = new Dictionary<string, string>
                                                               {
                                                                   {"alarm manager", "/AlarmManager"},
                                                                   {"sign in", "/"},
                                                                   {"reports", "/Reports"},
                                                                   {"alarm activity report","/Reports/Runner?reportId=/JCIARS/DefaultReports/AlarmActivity/History/Standard"},
                                                                   {"schedule report","/Reports?listAction=Schedule"},
                                                                   {"manage scheduled reports","/Reports/Scheduled"},
                                                                   {"help","/Help"},
                                                                   {"alarm display options","/AlarmDisplayOptions"},
                                                                   {"report server configuration","/ReportServerConfiguration"},
                                                                   {"sign out","/Account/LogOff"},
                                                                   {"unauthorized","/Unauthorized"},
                                                                   {"case management", "/Cases"},
                                                                   {"create case", "/Cases/New"},
                                                               };
       [Given("I am on the (.*)")]
        public void GivenIAmOnSomePage(string pagename)
       {
           pagename = NormalizePageName(pagename);
           Assert.IsTrue(PageNameToUrl.ContainsKey(pagename),pagename+" was not a known page.");
           WebBrowser.CurrentBrowser.NavigateTo(_baseSite + PageNameToUrl[pagename]);
           ThenIShouldArriveOn(pagename);
       }

        private static string NormalizePageName(string pageName)
        {
            var normalizedName = pageName.Trim().ToLowerInvariant();
            const string pageSuffix =" page";
            if (normalizedName.EndsWith(pageSuffix))
            {
                normalizedName = normalizedName.Substring(0, normalizedName.Length - pageSuffix.Length).TrimEnd();
            }
            return normalizedName;

        }

        [Then("I should arrive on the (.*)")]
        public void ThenIShouldArriveOn(string pagename)
        {
            pagename = NormalizePageName(pagename);
            WebBrowser.CurrentBrowser.WaitForUrl(PageNameToUrl[pagename.ToLower()], true, 10000);
        }

        [When("I navigate to the (.*) page")]
        public void WhenINavigateTo(string pagename)
        {
            pagename = NormalizePageName(pagename);
            Assert.IsTrue(PageNameToUrl.ContainsKey(pagename), pagename + " was not a known page.");
            WebBrowser.NavigateTo(_baseSite + PageNameToUrl[pagename]);
        }
    }
}
