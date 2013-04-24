/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TimeZone
{
    [TestClass]
    public class TimeZoneExtensionsTests
    {
        [TestMethod]
        public void IsValidTimeZone_WithValidTimeZone_ShouldReturnTrue()
        {
            // Arrange
            string localId = TimeZoneInfo.Local.Id;

            // Act
            bool isValid = localId.IsValidTimeZone();

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidTimeZone_WithInvalidTimeZone_ShouldReturnFalse()
        {
            // Arrange
            const string nonsense = "blah";

            // Act
            bool isValid = nonsense.IsValidTimeZone();

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidTimeZone_WithEmptyStringTimeZone_ShouldReturnFalse()
        {
            // Act
            bool isValid = "".IsValidTimeZone();

            // Assert
            Assert.IsFalse(isValid, "\"\" is not a valid timezone, but validity check returned true.");
        }

        [TestMethod]
        public void IsValidTimeZone_WithNullTimeZone_ShouldReturnFalse()
        {
            // Arrange
            const string nullstring = null;

            // Act
            bool isValid = nullstring.IsValidTimeZone();

            // Assert
            Assert.IsFalse(isValid,"null is not a valid timezone, but validity check returned true.");
        }
    }
}
