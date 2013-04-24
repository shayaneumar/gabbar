/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/


using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class PermissionTest
    {

        // Any number with 8th bit set has view permission
        private const string ViewAndEditPermission = "768"; // 0000 0011 0000 0000
        private const string EditPermission = "512"; // 0000 0010 0000 0000
        private const string ViewPermission = "256"; // 0000 0001 0000 0000
        private const string LessThanViewPermission = "255";

        [TestClass]
        public class IsViewAlarmManagerPermission
        {
            private const string AlarmManagerResourceIdentifier = "30002";


            [TestMethod]
            public void WithAlarmManagerPermissionSetExpectTrue()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = AlarmManagerResourceIdentifier, 
                                         PermissionLevel = ViewAndEditPermission 
                                     };

                // Act
                var isSet = permission.IsViewAlarmManagerPermission;

                // Assert
                Assert.IsTrue(isSet);
            }

            [TestMethod]
            public void WithAlarmManagerEditPermissionSetExpectTrue()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = AlarmManagerResourceIdentifier,
                                         PermissionLevel = EditPermission
                                     };

                // Act
                var isSet = permission.IsViewAlarmManagerPermission;

                // Assert
                Assert.IsTrue(isSet);
            }

            [TestMethod]
            public void WithLessThanViewPermissionSetExpectFalse()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = AlarmManagerResourceIdentifier,
                                         PermissionLevel = LessThanViewPermission
                                     };

                // Act
                var isSet = permission.IsViewAlarmManagerPermission;

                // Assert
                Assert.IsFalse(isSet);

            }

            [TestMethod]
            public void WithWrongResourceKeyButCorrectPermissionLevelExpectFalse()
            {
                // Arrange
                var permission = new Permission
                {
                    ResourceKey = "Made Up Resource",
                    PermissionLevel = ViewPermission
                };

                // Act
                var isSet = permission.IsViewAlarmManagerPermission;

                // Assert
                Assert.IsFalse(isSet);
            }
        }

        [TestClass]
        public class IsViewReportsPermission
        {
            private const string ReportsResourceIdentifier = "30006";

            [TestMethod]
            public void WithAlarmManagerPermissionSetExpectTrue()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = ReportsResourceIdentifier,
                                         PermissionLevel = ViewAndEditPermission
                                     };

                // Act
                var isSet = permission.IsViewReportsPermission;

                // Assert
                Assert.IsTrue(isSet);
            }

            [TestMethod]
            public void WithAlarmManagerEditPermissionExpectTrue()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = ReportsResourceIdentifier,
                                         PermissionLevel = EditPermission
                                     };

                // Act
                var isSet = permission.IsViewReportsPermission;

                // Assert
                Assert.IsTrue(isSet);
            }

            [TestMethod]
            public void WithTooLowOfAPermissionExpectFalse()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = ReportsResourceIdentifier,
                                         PermissionLevel = LessThanViewPermission
                                     };

                // Act
                var isSet = permission.IsViewReportsPermission;

                // Assert
                Assert.IsFalse(isSet);
            }

            [TestMethod]
            public void WithWrongResourceKeyButCorrectPermissionLevelExpectFalse()
            {
                // Arrange
                var permission = new Permission
                                     {
                                         ResourceKey = "Made Up Resource",
                                         PermissionLevel = ViewPermission
                                     };

                // Act
                var isSet = permission.IsViewReportsPermission;

                // Assert
                Assert.IsFalse(isSet);
            }
        }
    }
}
