/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls
{
    [TestClass]
    public class ByteExtensionsTests
    {
        [TestMethod]
        public void ToHexString_WithNullElements_ShouldThrowException()
        {
            //Assert
            ActionAssert.Throws<ArgumentNullException>(() => ByteExtensions.ToHexString(null), "bytes");
        }

        [TestMethod]
        public void ToHexString_WithEmptyByte_ShouldConvertEmptyByteArrayToHexString()
        {
            //Assert
            Assert.AreEqual("", new byte[0].ToHexString(), "Empty array of bytes did not result in an empty string.");
        }

        [TestMethod]
        public void ToHexString_WithByteArray_ShouldConvertByteArrayToHexString()
        {
            //Arrange
            var bytes = new byte[] { 10, 12, 22 };

            //Assert
            Assert.AreEqual(expected: "0A0C16", actual: bytes.ToHexString(), ignoreCase: true, message: "Failed to convert byte array into respective hexadecimal codes string.");
        }
    }
}
