/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Collections;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.ContextHelpers
{
    public static class ViewedCaseContext
    {
        public static Case Before
        {
            get
            {
                var beforeId = BeforeId;
                return beforeId == null ? null : CaseContext.Before.ValueOrDefault(beforeId);
            }
        }

        public static Case After
        {
            get
            {
                var afterId = AfterId;
                return afterId == null ? null : CaseContext.After.ValueOrDefault(afterId);
            }
        }

        private static string BeforeId
        {
            get
            {
                return ScenarioContext.Current.ValueOrDefault("Before.IdofViewedCase") as string;
            }
            set
            {
                ScenarioContext.Current["Before.IdofViewedCase"] = value;
            }
        }

        private static string AfterId
        {
            get
            {
                return ScenarioContext.Current.ValueOrDefault("After.IdofViewedCase") as string;
            }
            set
            {
                ScenarioContext.Current["After.IdofViewedCase"] = value;
            }
        }

        private static string GetCaseIdFromUrl(string url)
        {
            var caseIdInUrl = new Regex("/cases/(?<caseid>.+(?=/)|.+)", RegexOptions.IgnoreCase);
            return caseIdInUrl.Match(url).Groups["caseid"].Value;
        }

        [Binding]
        [Scope(Tag = "track_cases")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For use by specflow runtime only", error: true)]
        public class CaseEvents
        {
            [BeforeScenarioBlock]
            public void BeforeWhen()
            {
                if (ScenarioContext.Current.CurrentScenarioBlock == ScenarioBlock.When)
                {
                    BeforeId = GetCaseIdFromUrl(WebBrowser.CurrentBrowser.Url);
                }
            }

            [AfterScenarioBlock]
            public void AfterWhen()
            {
                if (ScenarioContext.Current.CurrentScenarioBlock == ScenarioBlock.When)
                {
                    AfterId = GetCaseIdFromUrl(WebBrowser.CurrentBrowser.Url);
                }
            }
        }
    }
}
