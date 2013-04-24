/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Collections
{
    [TestClass]
    public class DataChunkTests
    {
        [TestMethod]
        public void DataChunk_WithTwoItems_SetsFirstItem()
        {
            //Arrange
            var input = new[] { "Johnson", "Controls" };
            //Act;
            var actual = new DataChunk<string>(input, isEnd: true);
            //Assert
            Assert.AreEqual(input.First(), actual.FirstElement, "Failed to set first item of enumeration.");
        }

        [TestMethod]
        public void DataChunk_WithTwoItems_SetsLastItem()
        {
            //Arrange
            var input = new[] { "Johnson", "Controls" };
            //Act
            var actual = new DataChunk<string>(input, isEnd: true);
            //Assert
            Assert.AreEqual(input.Last(), actual.LastElement, "Failed to set last item of enumeration.");
        }

        [TestMethod]
        public void DataChunk_WhenEndOfData_SetsIsEndToTrue()
        {
            //Arrange
            //Act
            var actual = new DataChunk<string>(new[] { @"don't care" }, isEnd: true);
            //Assert
            Assert.IsTrue(actual.IsEnd, "Failed to set IsEnd property of enumeration.");
        }

        [TestMethod]
        public void DataChunk_WithTwoItems_ShouldMatchEnnumerationCount()
        {
            //Arrange
            //Act
            var actual = new DataChunk<string>(new[] { "1", "2" }, isEnd: true);
            //Assert
            Assert.AreEqual(2, actual.Count, "Failed to match count of enumeration.");
        }

        [TestMethod]
        public void DataChunk_WithEmptyEnumearation_ShouldReturnCountAsZero()
        {
            //Arrange
            //Act
            var actual = new DataChunk<string>(Enumerable.Empty<string>(), isEnd: true);
            //Assert
            Assert.AreEqual(0, actual.Count, "DataChunk was constructed with an empty array, but had a count that was not 0");
        }

        [TestMethod]
        public void DataChunk_WithNullEnumearation_ShouldThrowException()
        {
            //Act+Assert
            ActionAssert.Throws<ArgumentNullException>(()=>new DataChunk<string>(data:null, isEnd: true), "data");
        }
    }
}
