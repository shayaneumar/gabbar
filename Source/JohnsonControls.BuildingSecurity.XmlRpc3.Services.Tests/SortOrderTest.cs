/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    ///This is a test class for SortOrderTest and is intended
    ///to contain all SortOrderTest Unit Tests
    ///</summary>
    [TestClass]
    public class SortOrderTest
    {
        private readonly XmlSerializer<SortOrder> _sortOrderSerializer = new XmlSerializer<SortOrder>();

        /// <summary>
        /// Tests proper deserialization with all members present.
        /// </summary>
        [TestMethod]
        public void WithAllElementsPresent()
        {
            // Arrange
            var expected = new SortOrder(new[] { new SortKey("1", "AlarmResponseText", "StartKey", "LastKey") }, "ASC");
            const string xml = @"<SortOrder>
                            <SortKeys>
                                <SortKey SequenceNumber=""1"">
                                    <Name>AlarmResponseText</Name>
                                    <StartKey>StartKey</StartKey>
                                    <LastKey>LastKey</LastKey>
                                </SortKey>
                            </SortKeys>
                            <Order>ASC</Order>
                        </SortOrder>";

            // Act
            var actual = _sortOrderSerializer.Deserialize(xml);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests deserialization with no members present.
        /// </summary>
        [TestMethod]
        public void WithEmptySortOrderElementPresent()
        {
            // Arrange
            var expected = new SortOrder();
            const string xml = @"<SortOrder>
                                 </SortOrder>";

            // Act
            var actual = _sortOrderSerializer.Deserialize(xml);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Order
        /// </summary>
        [TestMethod]
        public void OrderTest()
        {
            // Arrange
            const string expected = "DESC";

            // Act
            var actual = new SortOrder { Order = "DESC" };

            // Assert
            Assert.AreEqual(expected, actual.Order);
        }
    }
}
