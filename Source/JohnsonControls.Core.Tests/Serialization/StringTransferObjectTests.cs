/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Serialization
{
    [TestClass]
    public class StringTransferObjectTests
    {
        [TestMethod]
        public void Construct_WithValidInputString_ValueShouldMatchInputString()
        {
            //Arrange
            var objStringTransfer = new StringTransferObject("input");

            //Act
            string value = objStringTransfer.Value;

            //Assert
            Assert.AreEqual("input", value, "Failed to transfer input string to new string object.");
        }
    }
}
