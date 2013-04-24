using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class UserDetailsTest
    {
        private const string AlarmManagerResourceIdentifier = "30002";
        private const string ReportsResourceIdentifier = "30006";
        private const string ViewPermission = "256";

        [TestClass]
        public class CanViewAlarmManager
        {


            [TestMethod]
            public void WithNullPermissionsExpectFalse()
            {
                // Arrange
                var userDetails = new UserDetails();

                // Act
                var isSet = userDetails.CanViewAlarmManager;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithNoPermissionsExpectFalse()
            {
                // Arrange
                var userDetails = new UserDetails {Permissions = new Permission[0]};

                // Act
                var isSet = userDetails.CanViewAlarmManager;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithExpectedPermissionsExpectTrue()
            {
                // Arrange
                var userDetails = new UserDetails
                                      {
                                          Permissions =
                                              new[]
                                                  {
                                                      new Permission
                                                          {
                                                              PermissionLevel = ViewPermission,
                                                              ResourceKey = AlarmManagerResourceIdentifier
                                                          }
                                                  }
                                      };

                // Act
                var isSet = userDetails.CanViewAlarmManager;

                // Assert
                Assert.IsTrue(isSet);
            }
        }

        [TestClass]
        public class CanViewReports
        {
            [TestMethod]
            public void WithNullPermissionsExpectFalse()
            {
                // Arrange
                var userDetails = new UserDetails();

                // Act
                var isSet = userDetails.CanViewReports;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithNoPermissionsExpectFalse()
            {
                // Arrange
                var userDetails = new UserDetails { Permissions = new Permission[0] };

                // Act
                var isSet = userDetails.CanViewReports;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithExpectedPermissionsExpectTrue()
            {
                // Arrange
                var userDetails = new UserDetails
                                      {
                                          Permissions =
                                              new[]
                                                  {
                                                      new Permission
                                                          {
                                                              PermissionLevel = ViewPermission,
                                                              ResourceKey = ReportsResourceIdentifier
                                                          }
                                                  }
                                      };

                // Act
                var isSet = userDetails.CanViewReports;

                // Assert
                Assert.IsTrue(isSet);
            }
        }

    }
}
