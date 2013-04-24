/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Tags
{
    [Binding]
    public class AsNewUser
    {
        [BeforeScenario("asNewUser")]
        public void BeforeScenario()
        {
            WebBrowser.CleanUp();
        }
    }
}
