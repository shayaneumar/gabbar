/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using BuildingSecurity.Reporting.ReportingServices2010;
using JohnsonControls.BuildingSecurity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingSecurity.Reporting.Tests
{
    /// <summary>
    ///This is a test class for ReportingTest and is intended
    ///to contain all ReportingTest Unit Tests
    ///</summary>
    [TestClass]
    public class ReportingTest
    {
        private readonly IReportingClient _reportingClient;

        public ReportingTest()
        {
            _reportingClient = new ReportingClient(new ReportServerConfiguration("http://10.10.93.87/ReportServer", "P20003", "administrator", "").CloneWithNewPassword("control01!"), new BasicHttpBinding("ReportingServicesSoap"));
        }

        /// <summary>
        /// Integration test to inspect GetReportList
        ///</summary>
        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore]
        public void GetReportsTest()
        {
            // Act
            var results = _reportingClient.GetReports("/");

            // Assert
            Assert.IsInstanceOfType(results, typeof(IEnumerable));
        }

        /// <summary>
        /// Integration test to inspect GetReportList
        ///</summary>
        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore]
        public void GetParametersTest()
        {
            // Act
            var results = _reportingClient.GetParameters("/JCIARS/DefaultReports/AuditTrail/History/Standard", "History", new ParameterValue[0]);

            // Assert
            Assert.IsInstanceOfType(results, typeof(IEnumerable));
        }

        /// <summary>
        /// Integration test to inspect GetParameters
        ///</summary>
        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore]
        public void GetParametersWithValuesTest()
        {
            // Arrange
            var parameterValues = new List<ParameterValue>
                                      {
                                          new ParameterValue("AuditTypeSelectSingle", "Audit Type"),
                                          new ParameterValue("LanguageSelectSingle", "Language")
                                      };

            // Act
            var results = _reportingClient.GetParameters("/JCIARS/DefaultReports/AuditTrail/History/Standard", "History", parameterValues.ToArray());

            // Assert
            Assert.IsInstanceOfType(results, typeof(IEnumerable));
        }
    }
}
