/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Text;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls
{
    [TestClass]
    public class StringExtensionsTests
    {

        [TestMethod]
        public void SafeTrim_WithValidInputString_ShouldTrimString()
        {
            //Arrange
            const string trimstring = "  trim string   ";

            //Assert
            Assert.AreEqual("trim string", trimstring.SafeTrim(), "Failed to trim white-space from the beginning or end of a string.");
        }

        [TestMethod]
        public void Sha1_WithNullStringElement_ShouldThrowNullException()
        {
            //Arrange
            Encoding encoding = new ASCIIEncoding();

            //Assert
            ActionAssert.Throws<ArgumentNullException>(() => StringExtensions.Sha1(null, encoding), "str");
        }

        [TestMethod]
        public void Sha1_WithNullEncodingElement_ShouldThrowNullException()
        {
            //Arrange
            const string nullencodingstring = "encoding string";

            //Assert
            ActionAssert.Throws<ArgumentNullException>(() => StringExtensions.Sha1(nullencodingstring, null), "encoding");
        }

        [TestMethod]
        public void Sha1_WithValidInputStringAndEncoding_ShouldReturnHashedString()
        {
            //Arrange
            const string inputstring = "input string";
            Encoding encoding = new ASCIIEncoding();

            //Act
            var hashedString = inputstring.Sha1(encoding);

            //Assert
            Assert.AreEqual(expected: "B1A39A26EA62A5C075CD3CB5AA46492C8E1134B7", actual: hashedString, ignoreCase: true, message: "Failed to make hashed secure string from the input string.");
        }


        [TestMethod]
        public void ToSecureString_AfterSecuring_ShouldBeReadOnly()
        {
            //Arrange
            //Act
            var securedString = "some string".ToSecureString();

            //Assert
            Assert.IsTrue(securedString.IsReadOnly(), message: "Secure string should be set to readonly, but was not.");
        }

    }
}
