/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BuildingSecurity.WebApp.AcceptanceTests.Util;
using JohnsonControls.BuildingSecurity;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.ContextHelpers
{
    public static class CaseContext
    {
        /// <summary>
        /// Gets the cases what were in the system (or simulator) prior to entering the WHEN block
        /// of the current scenario.
        /// </summary>
        public static IDictionary<string, Case> Before
        {
            get
            {
                IDictionary<string, Case> result;
                if (ScenarioContext.Current.TryGetValue("Before.Cases", out result))
                {
                    return result;
                }
                throw new InvalidOperationException(@"Cases have not yet been initialized. Possible causes are, the WHEN block has not started." +
                                                    @" CaseContext.CaseEvents.BeforeWhen must execute prior getting this property.");
            }
            private set { ScenarioContext.Current["Before.Cases"] = value; }
        }

        /// <summary>
        /// Gets the cases what were in the system (or simulator) after exiting the WHEN block
        /// of the current scenario.
        /// </summary>
        public static IDictionary<string, Case> After
        {
            get
            {
                IDictionary<string, Case> result;
                if (ScenarioContext.Current.TryGetValue("After.Cases", out result))
                {
                    return result;
                }
                throw new InvalidOperationException(@"Cases have not yet been initialized. Possible causes are, the WHEN block has not finished." +
                                    @" CaseContext.CaseEvents.AfterWhen must execute prior getting this property.");
            }
            private set { ScenarioContext.Current["After.Cases"] = value; }
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
                    Before = CaseClient.GetCaseListFor(@"_systemuser").ToDictionary(x => x.Id, x => x);
                }
            }

            [AfterScenarioBlock]
            public void AfterWhen()
            {
                if (ScenarioContext.Current.CurrentScenarioBlock == ScenarioBlock.When)
                {
                    After = CaseClient.GetCaseListFor(@"_systemuser").ToDictionary(x => x.Id, x => x);
                }
            }
        }
    }
}
