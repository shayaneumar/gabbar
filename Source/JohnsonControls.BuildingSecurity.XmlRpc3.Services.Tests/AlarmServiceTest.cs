/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// TODO: Figure out or document how to run all the unit tests in this solution
// exception integration tests.
namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    ///This is a test class for AlarmServiceTest and is intended
    ///to contain all AlarmServiceTest Unit Tests
    ///</summary>
    [TestClass]
    public class AlarmServiceTest
    {
        private Configuration _config;
        private ITypedSessionManagement _sessionManagement;

        #region Additional test attributes
        [TestInitialize]
        public void TestInitialize()
        {
            _config = new Configuration();
            Login();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Logout();
        }
        #endregion

        public void Login()
        {
           _sessionManagement = new SessionManagementService(_config.PegasysUrl);
           P2000LoginReply p2000LoginReply = _sessionManagement.P2000Login(_config.ValidSuperUserName, _config.ValidSuperUserPassword);
            _config.SessionGuid = p2000LoginReply.SessionInfo.SessionGuid;
        }

        public void Logout()
        {
            _sessionManagement.P2000Logout(_config.ValidSuperUserName, _config.SessionGuid);
        }

        [TestMethod]
        [TestCategory(TestType.Integration)]
        [Ignore] // TODO: Move integration tests into a different project
        public void AlarmGetListTest()
        {
            // Arrange
            var target = new AlarmService(_config.PegasysUrl, _config.RealTimeServiceAddress);

            // Act
            var actual = target.AlarmGetListEx(_config.ValidSuperUserName, _config.SessionGuid, null, null, null, null, null, null, 50, null);

            // Assert
            Assert.IsTrue(actual.AlarmMessages.Length > 0, 
                          "Assertion Failed because Proxy method AlarmGetList Failed to return any alarms."); 
        }

        /// <summary>
        ///A test for AlarmUpdate.  This integration test is included for individual 
        /// testing and debugging but should
        /// not be executed when "running all Solution tests".   A valid alarmGuid must be found
        /// on the system of test and entered below.
        ///</summary>
        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore] // TODO: Move integration tests into a different project
        public void AlarmActionTest()
        {
            AlarmMessageDetails messageDetails1 = null;
            AlarmMessageDetails messageDetails2 = null;
            // Arrange
            ITypedAlarmService alarmService = new AlarmService(_config.PegasysUrl, _config.RealTimeServiceAddress);


            var alarmResponse = alarmService.AlarmGetListEx(_config.ValidSuperUserName,
                                                            _config.SessionGuid,
                                                            null, null, null, null, null, null, 50, new SortOrder());

            foreach (var message in alarmResponse.AlarmMessages)
            {
                if (message.MessageDetails.AlarmState == "3")
                {
                    if (messageDetails1 == null)
                        messageDetails1 = message.MessageDetails;
                    else if (messageDetails2 == null)
                    {
                        messageDetails2 = message.MessageDetails;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Act
            if (messageDetails1 != null && messageDetails2 != null)
                alarmService.AlarmAction(_config.ValidSuperUserName,
                                         _config.SessionGuid.ToString(CultureInfo.InvariantCulture),
                                         new[]
                                             {
                                                 new AlarmIdSequenceTuple(new Guid(messageDetails1.AlarmGuid),
                                                                      Int32.Parse(
                                                                          messageDetails1.ConditionSequenceNumber))
                                                 ,
                                                 new AlarmIdSequenceTuple(new Guid(messageDetails2.AlarmGuid),
                                                                      Int32.Parse(
                                                                          messageDetails2.ConditionSequenceNumber))
                                             }, 3, "Response");

            // Assert
            // Assertion Fails if AlarmUpdate throws an Exception.
        }
    }
}
