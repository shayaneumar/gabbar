/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.TestExtensions;
using JohnsonControls.XmlRpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    
    
    /// <summary>
    ///This is a test class for SessionManagementServiceTest and is intended
    ///to contain all SessionManagementServiceTest Unit Tests
    ///</summary>
    [TestClass]
    public class SessionManagementServiceTest
    {
        private Configuration _config;

        [TestInitialize]
        public void TestInitialize()
        {
            _config = new Configuration();
        }

        /// <summary>
        ///A test for LogOn
        ///</summary>
        [TestMethod]
        [TestCategory("IntegrationTest")]
        [Ignore] // TODO: Move integration tests into a different project
        public void LogOnTest()
        {
            // Arrange
            ITypedSessionManagement target = new SessionManagementService(_config.PegasysUrl);

            // Act
            var p2000Reply = target.P2000Login(_config.ValidSuperUserName, _config.ValidSuperUserPassword);

            // Assert - nothing to do, just checking to make sure no exception thrown

            // Cleanup the Session
            target.P2000Logout(_config.ValidSuperUserName, p2000Reply.SessionInfo.SessionGuid);
        }

        [TestMethod]
        [Ignore]
        [TestCategory("IntegrationTest")] // TODO: Move integration tests into a different project
        public void Login_VerifyP2000ReplyTest()
        {
            // Arrange
            ITypedSessionManagement target = new SessionManagementService(_config.PegasysUrl);

            // Act
            var p2000Reply = target.P2000Login(_config.ValidSuperUserName, _config.ValidSuperUserPassword);

            // Assert
            Assert.IsTrue(p2000Reply.UserDetails.Partitions.Length > 0);

            // Cleanup the Session
            target.P2000Logout(_config.ValidSuperUserName, p2000Reply.SessionInfo.SessionGuid);
        }


        [TestClass]
        public class P2000SessionHeartBeat
        {
            private Configuration _config;

            [TestInitialize]
            public void TestInitialize()
            {
                _config = new Configuration();
            }

            [TestMethod]
            [TestCategory(TestType.Integration)]
            [Ignore]
            public void InvalidUserInvalidSession_ThrowsException()
            {
                // Arrange
                var hbService = new SessionManagementService(_config.PegasysUrl);

                // Act + Assert
                ActionAssert.Throws<ServiceOperationException>(() => hbService.P2000SessionHeartbeat("not_a_real_user", "not_a_real_sessionid"));
            }

            [TestMethod]
            public void UsernameNull_ArgumentExceptionIsThrown()
            {
                // Arrange
                var hbService = new SessionManagementService(_config.PegasysUrl);

                // Act + Assert
                ActionAssert.Throws<ArgumentException>(() => hbService.P2000SessionHeartbeat(null, "notwhitespace"), "userName");
            }

            [TestMethod]
            public void SessionGuidNull_ArgumentExceptionIsThrown()
            {
                // Arrange
                var hbService = new SessionManagementService(_config.PegasysUrl);

                // Act + Assert
                ActionAssert.Throws<ArgumentException>(() => hbService.P2000SessionHeartbeat("user", null), "sessionGuid");
            }

            [TestMethod]
            public void SessionGuidWhitespace_ArgumentExceptionIsThrown()
            {
                // Arrange
                var hbService = new SessionManagementService(_config.PegasysUrl);

                // Act + Assert
                ActionAssert.Throws<ArgumentException>(() => hbService.P2000SessionHeartbeat("userName", "  "), "sessionGuid");
            }

            [TestMethod]
            public void UsernameWhitespace_ArgumentExceptionIsThrown()
            {
                // Arrange
                var hbService = new SessionManagementService(_config.PegasysUrl);

                // Act + Assert
                ActionAssert.Throws<ArgumentException>(() => hbService.P2000SessionHeartbeat(" ", "sessionGuid"), "userName");
            }
        }


    }
}
