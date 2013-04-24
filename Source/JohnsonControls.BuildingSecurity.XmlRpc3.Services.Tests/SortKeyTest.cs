/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class SortKeyTest
    {
        private readonly XmlSerializer<SortKey> _sortKeySerializer = new XmlSerializer<SortKey>();

        /// <summary>
        /// Test to verify the default constructor is setting properties correctly.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorTest()
        {
            // Assert no exception
            ActionAssert.DoesNotThrow<Exception>(() => new SortKey());
        }

        /// <summary>
        /// Test to verify that members set by the constructor are set properly.
        /// </summary>
        [TestMethod]
        public void ConstructorAllMembersTest()
        {
            // Arrange
            var expected = new SortKey { SequenceNumber = "1", Name = "NameTest", StartKey = "StartKey", LastKey = "LastKey" };

            // Act
            var actual = new SortKey("1", "NameTest", "StartKey", "LastKey");

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod]
        public void NameTest()
        {
            // Arrange
            const string expected = "ExpectedName";

            // Act
            var actual = new SortKey { Name = "ExpectedName"};

            // Assert
            DtoAssert.AreEqual(expected, actual.Name);
        }

        /// <summary>
        /// Tests that all members are deserialized properly.
        /// </summary>
        [TestMethod]
        public void WithAllElementsPresent()
        {
            // Arrange
            var expected = new SortKey("1", "SortName", "StartKey", "LastKey");
            const string xml = @"<SortKey SequenceNumber=""1"">
                                <Name>SortName</Name>
                                <StartKey>StartKey</StartKey>
                                <LastKey>LastKey</LastKey>
                            </SortKey>";

            // Act
            var actual = _sortKeySerializer.Deserialize(xml);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests proper deserialization with all members missing
        /// from the SortKey Element.
        /// </summary>
        [TestMethod]
        public void WithAllSortKeyElementsMissing()
        {
            // Arrange
            var expected = new SortKey();
            const string xml = @"<SortKey>
                                 </SortKey>";

            // Act
            var actual = _sortKeySerializer.Deserialize(xml);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

    }
}
