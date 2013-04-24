/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime
{
    [TestClass]
    public class InMemorySimulationClientTests
    {
        private static InMemorySimulationClient GetSimulationClient(InMemoryCaseRepository caseRepository = null)
        {
            return new InMemorySimulationClient(new PseudoBuildingSecurityClient(caseRepository??new InMemoryCaseRepository()), new Scheduler(TimeSpan.FromMilliseconds(1.0)));
        }

        [TestMethod]
        public void UpdateCase_WithDifferentTitle_ChanesCaseTitle()
        {
            //arrange
            var caseRepository = new InMemoryCaseRepository();
            const string caseId = "1";
            const string desiredTitle = "updated title";

            caseRepository.CreateCase(new CaseData{ Id = "1", Title = "Starting Title" });
            var client = GetSimulationClient(caseRepository);

            //act
            client.Run("{events:[{'at':'0.0:0:0.0','name':'UpdateCase','value':{'Id':'" + caseId + "','Title':'" + desiredTitle + "'}}]}");
            Thread.Sleep(200);//Run is asynchronous and as of writing this test one can not provide simulation client with a scheduler that
            //runs everything at once.

            //assert
            Assert.AreEqual(desiredTitle, caseRepository.RetrieveCase(caseId).Title, "UpdateCase failed to update case's title");
        }
    }
}
