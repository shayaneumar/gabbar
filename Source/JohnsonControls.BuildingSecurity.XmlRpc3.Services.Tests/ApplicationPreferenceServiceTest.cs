/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
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
    public class ApplicationPreferenceServiceTest
    {
        private Configuration _config;
        private ITypedSessionManagement _sessionManagementService;

        #region Additional test attributes
        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void TestInitialize()
        {
            _config = new Configuration();
            _sessionManagementService = new SessionManagementService(_config.PegasysUrl);
        }
        #endregion

        public void Login()
        {
            P2000LoginReply p2000LoginReply = _sessionManagementService.P2000Login(_config.ValidSuperUserName, _config.ValidSuperUserPassword);
            _config.SessionGuid = p2000LoginReply.SessionInfo.SessionGuid;
        }

        public void Logout()
        {
            _sessionManagementService.P2000Logout(_config.ValidSuperUserName, _config.SessionGuid);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore] // TODO: Move integration tests into a different project
        public void ApplicationPreferenceSaveTest()
        {
            // Arrange
            var target = new ApplicationPreferenceService(_config.PegasysUrl);
            var exceptionHappened = false;
            // Act
            Login();
            try
            {
                //string xmlText = string.Format("<Data>{0}</Data>", @"Testing 1");
                const string xmlText = "Test 1";
                target.ApplicationPreferenceSave(_config.ValidSuperUserName, _config.SessionGuid, "key", PreferenceType.Application, xmlText);
            }
            catch (Exception)
            {
                exceptionHappened = true;
            }
            finally
            {
                Logout(); 
            }

            // Assert
            Assert.IsTrue(!exceptionHappened); 
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore] // TODO: Move integration tests into a different project
        public void ApplicationPreferenceReadTest()
        {
            // Arrange
            var target = new ApplicationPreferenceService(_config.PegasysUrl);
            var exceptionHappened = false;
            string result=null;
            // Act
            Login();
            try
            {
                result = target.ApplicationPreferenceRead(_config.ValidSuperUserName, _config.SessionGuid, "key", PreferenceType.Application);
            }
            catch (Exception)
            {
                exceptionHappened = true;
            }
            finally
            {
                Logout();
            }

            // Assert
            Assert.IsFalse(exceptionHappened);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore] // TODO: Move integration tests into a different project
        public void ApplicationPreferenceDeleteTest()
        {
            // Arrange
            var target = new ApplicationPreferenceService(_config.PegasysUrl);
            var exceptionHappened = false;
            // Act
            Login();
            try
            {
                target.ApplicationPreferenceDelete(_config.ValidSuperUserName, _config.SessionGuid, "key", PreferenceType.Application);
            }
            catch (Exception)
            {
                exceptionHappened = true;
            }
            finally
            {
                Logout();
            }

            // Assert
            Assert.IsFalse(exceptionHappened);
        }
    }
}
