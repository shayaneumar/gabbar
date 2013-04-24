/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    [TestClass]
    public class IdHelpersTests
    {
        [TestMethod]
        public void GetGuid_WhenIdIsNull_ReturnsRandomGuid()
        {
            //Arrange
            //Act
            var normalizedId = IdHelpers.GetGuid(null);
            var secondNormalizedId = IdHelpers.GetGuid(null);
            //Assert
            Assert.AreNotEqual(normalizedId, secondNormalizedId,"Two alarms with no id have the same normalized id. When id is null the normalized id should be random");
        }

        [TestMethod]
        public void GetId_WhenStringIsNull_ReturnRandomString()
        {
            //Arrange
            //Act
            string resultString = IdHelpers.GetId(null);
            string secondresultString = IdHelpers.GetId(null);

            //Assert
            Assert.AreNotEqual(resultString, secondresultString,"Back to back calls with null input resulted in identical outputs. Expected a random string for both calls.");
        }

        [TestMethod]
        public void GetId_WhenStringIdIsNotNull_ReturnInput()
        {
            //Arrange
            const string localString = "abcs";

            //Act
            string resultString = IdHelpers.GetId(localString);

            //Assert
            Assert.AreEqual(localString, resultString,"If input string is not null GetID should return input string, but did not.");
        }
    }
}
