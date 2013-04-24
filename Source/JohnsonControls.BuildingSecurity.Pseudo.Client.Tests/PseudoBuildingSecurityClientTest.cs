/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using JohnsonControls.BuildingSecurity.XmlRpc3.Client;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    [TestClass]
    public class TheRetrieveCasesMethod
    {
        private static IBuildingSecurityClientCookie CreateCookie(string userName = null, string fullName = null, string sessionId = null,
            IEnumerable<Partition> partitions = null, bool canViewAlarms = false, bool canViewReports = false)
        {
            var permission = new Dictionary<string, bool>
                                 {
                                     {PermissionNames.CanViewAlarmManager, canViewAlarms},
                                     {PermissionNames.CanViewReports, canViewReports}
                                 }.ToDictionary(x => x.Key.ToUpperInvariant(), x => x.Value);
            return new BuildingSecurityClientCookie(userName: userName ?? "userName", fullName: fullName ?? "fullName", sessionId: sessionId ?? Guid.NewGuid().ToString(),
                               partitionList: partitions ?? Enumerable.Empty<Partition>(), permissions: permission);
        }

        private static PseudoBuildingSecurityClient GetBuildingSecurityClient(InMemoryCaseRepository caseRepository = null)
        {
            return new PseudoBuildingSecurityClient(caseRepository??new InMemoryCaseRepository());
        }

        [TestMethod]
        public void ShouldReturnCases()
        {
            //Arrange
            var expected = new[]
                {
                    new Case(Guid.NewGuid().ToString(), "Case 2", "User 1", DateTimeOffset.Now, "User 1", Enumerable.Empty<CaseNote>(), CaseStatus.Open),
                    new Case(Guid.NewGuid().ToString(), "Case 1", "User 2",  DateTimeOffset.Now, "User 2", Enumerable.Empty<CaseNote>(), CaseStatus.Open)
                };
            var target = GetBuildingSecurityClient(caseRepository: new InMemoryCaseRepository(expected));

            //Act
            var actual = target.RetrieveCases(CreateCookie()).Data;

            //Assert
            DtoAssert.AreEqual(expected.OrderBy(c => c.Title).ToList(), actual.OrderBy(c => c.Title).ToList());
        }

        [TestMethod]
        public void UpdateCase_WithNewTitle_ShouldUpdateCase()
        {
            //Arrange
            var startingCases = new[]
                {
                    new Case(id:"id", title: "needs to be updated", createdBy: "User 1", createdDateTime: DateTimeOffset.Now, owner: "User 1", notes: Enumerable.Empty<CaseNote>(), status: CaseStatus.Open)
                };

            var target = GetBuildingSecurityClient(caseRepository: new InMemoryCaseRepository(startingCases));

            //Act
            var result = target.UpdateCase(CreateCookie(),"id",new {Title="UpdatedTitle"});

            //Assert
            Assert.AreEqual("UpdatedTitle",result.Title);
        }

        [TestMethod]
        public void UpdateCase_WithNewOwner_ShouldUpdateCase()
        {
            //Arrange
            var startingCases = new[]
                {
                    new Case(id:"id", title: "title", createdBy: "needs to be updated", createdDateTime: DateTimeOffset.Now, owner: "User 1", notes: Enumerable.Empty<CaseNote>(), status: CaseStatus.Open)
                };

            var target = GetBuildingSecurityClient(caseRepository: new InMemoryCaseRepository(startingCases));

            //Act
            var result = target.UpdateCase(CreateCookie(), "id", new { Owner = "NewOwner" });

            //Assert
            Assert.AreEqual("NewOwner", result.Owner);
        }

        [TestMethod]
        public void UpdateCase_WithNoNewInformation_ShouldReturnTheCurrentCase()
        {
            //Arrange
            var currentCase = new Case(id: "id", title: "title", createdBy: "needs to be updated",
                                       createdDateTime: DateTimeOffset.Now, owner: "User 1",
                                       notes: Enumerable.Empty<CaseNote>(), status: CaseStatus.Open);
            var target = GetBuildingSecurityClient(caseRepository: new InMemoryCaseRepository(new []{currentCase}));

            //Act
            var result = target.UpdateCase(CreateCookie(), "id", new {});

            //Assert
            DtoAssert.AreEqual(currentCase, result);
        }

        [TestMethod]
        public void UpdateCase_ReOpenStatus_ShouldOpenCase()
        {
            //Arrange
            var startingCase = new Case(id: "id", title: "title", createdBy: "User 1",
                                        createdDateTime: DateTimeOffset.Now, owner: "User 1",
                                        notes: Enumerable.Empty<CaseNote>(), status: CaseStatus.Closed);

            var target = GetBuildingSecurityClient(caseRepository: new InMemoryCaseRepository(new[] { startingCase }));

            //Act
            var result = target.UpdateCase(CreateCookie(), "id", new { Status = "open" });

            //Assert
            Assert.AreEqual(CaseStatus.Open, result.StatusEnum);
        }
    }
}
