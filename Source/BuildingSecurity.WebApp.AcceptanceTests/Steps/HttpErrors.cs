/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class HttpErrors
    {
        [Then(@"I receive a permission denied error")]
        public void ThenIShouldRecieveAPermissionDeniedError()
        {
            Assert.IsTrue(WebBrowser.CurrentBrowser.PageTitle.Contains(@"403"), "Expected a 403 error, but did not get one.");
        }
    }
}
