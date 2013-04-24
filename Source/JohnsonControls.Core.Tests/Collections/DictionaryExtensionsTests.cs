/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Collections
{
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void ValueOrDefault_KeyDoesNotExist_ReturnsDefault()
        {
            //Arrange
            var data = new Dictionary<string, int>();

            //Act
            var result = data.ValueOrDefault("key");

            //Assert
            Assert.AreEqual(default(int), result, "Expected the default value to be returned because key is not present in the supplied dictionary. But instead got '{0}'", result);
        }

        [TestMethod]
        public void ValueOrDefault_KeyExists_ReturnsValue()
        {
            //Arrange
            var data = new Dictionary<string, int> {{"foo", 10}};

            //Act
            var result = data.ValueOrDefault("foo");

            //Assert
            Assert.AreEqual(10,result,"Failed to return value associated with the supplied key, even though key was present in dictionary. Instead got '{0}'", result);
        }

        [TestMethod]
        public void ValueOrDefault_NullKey_ThrowsException()
        {
            //Arrange
            var data = new Dictionary<string, int>();

            //Act
            //Assert
            ActionAssert.Throws<ArgumentNullException>(()=>data.ValueOrDefault(null), @"key");
        }

        [TestMethod]
        public void ValueOrDefault_NullDictionary_ThrowsException()
        {
            //Arrange
            IDictionary<string, string> data = null;

            //Act+Assert
            ActionAssert.Throws<ArgumentNullException>(() => data.ValueOrDefault("Not null key"), @"dictionary");
        }
    }
}
