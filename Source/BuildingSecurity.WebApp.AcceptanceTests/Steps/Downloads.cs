/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuildingSecurity.WebApp.AcceptanceTests.Steps
{
    [Binding]
    public class Downloads
    {

        [Then(@"a (.*) should be downloaded")]
        public void ThenAFileShouldBeDownloaded(string filetype)
        {
            Assert.Inconclusive("File downloads can not be detected yet.");
        }
    }
}
