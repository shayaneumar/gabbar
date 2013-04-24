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
    /// <summary>
    ///This is a test class for AlarmGetResponseTextListRequestTest and is intended
    ///to contain all AlarmGetResponseTextListRequestTest Unit Tests
    ///</summary>
    [TestClass]
    public class AlarmGetResponseTextListRequestTest
    {
        private readonly XmlSerializer<AlarmGetResponseTextListRequest> _serializer =
            new XmlSerializer<AlarmGetResponseTextListRequest>();

        /// <summary>
        ///A test for AlarmGetResponseTextListRequest Default Constructor
        ///</summary>
        [TestMethod]
        public void DefaultConstructorTest()
        {
            // Assert no exception thrown
            ActionAssert.DoesNotThrow<Exception>(() => new AlarmGetResponseTextListRequest());
        }

        /// <summary>
        ///A test for AlarmGetResponseTextListRequest Constructor that all members are set
        /// properly.
        ///</summary>
        [TestMethod]
        public void AllMemberConstructorTest()
        {
            // Arrange
            var expected = new AlarmGetResponseTextListRequest
                               {
                                   Filter = new AlarmResponseTextFilter("partition"),
                                   Paging = new Paging(5, 50, 500),
                                   SortOrder = new SortOrder(new[] {new SortKey("1", "field1", null, null)})
                               };

            // Act
            var actual = new AlarmGetResponseTextListRequest(new AlarmResponseTextFilter("partition"),
                                                             new Paging(5, 50, 500), new SortOrder(new[] { new SortKey("1", "field1", null, null) }));

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test verifying the AlarmResponseTextFilter object can be set
        /// and retrieved properly
        ///</summary>
        [TestMethod]
        public void FilterTest()
        {
            // Arrange
            var expected = new AlarmResponseTextFilter("partition");
            var requestObj = new AlarmGetResponseTextListRequest { Filter = new AlarmResponseTextFilter("partition") };

            // Act
            var actual = requestObj.Filter;

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test verifying the Paging object can be set
        /// and retrieved properly
        ///</summary>
        [TestMethod]
        public void PagingTest()
        {
            // Arrange
            var expected = new Paging(5, 50, 500);
            var requestObj = new AlarmGetResponseTextListRequest { Paging = new Paging(5, 50, 500) };
            
            // Act
            var actual = requestObj.Paging;

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test verifying the Sort Order object can be set
        /// and retrieved properly
        ///</summary>
        [TestMethod]
        public void SortOrderTest()
        {
            // Arrange
            var expected = new SortOrder(new[] { new SortKey("1", "Field1", null, null) });
            var requestObj = new AlarmGetResponseTextListRequest { SortOrder = new SortOrder(new[] { new SortKey("1", "Field1", null, null) }) };

            // Act
            var actual = requestObj.SortOrder;

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WithAllElementsPresent()
        {
            // Arrange
            var expected = new AlarmGetResponseTextListRequest(new AlarmResponseTextFilter("Partition1"),
                                                               new Paging(5, 50, 500), new SortOrder(new[] { new SortKey("1", "Column1", null, null) }));
            const string requestObj = @"
                    <AlarmGetResponseTextListRequest>
                        <AlarmResponseTextFilter>
                            <Partition>
                                <CV>Partition1</CV>
                            </Partition>
                        </AlarmResponseTextFilter>
                        <Paging>
                            <Page>5</Page>
                            <RecordsPerPage>50</RecordsPerPage>
                            <RecordCount>500</RecordCount>
                        </Paging>
                        <SortOrder>
                            <SortKeys>
                                <SortKey SequenceNumber=""1"">
                                    <Name>Column1</Name>
                                </SortKey>
                            </SortKeys>
                            <Order>DESC</Order>
                        </SortOrder>
                    </AlarmGetResponseTextListRequest>";

            // Act
            var actual = _serializer.Deserialize(requestObj);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests deserialization with no members present.
        /// </summary>
        [TestMethod]
        public void WithEmptyAlarmGetResponseTextListRequestElementPresent()
        {
            // Arrange
            var expected = new AlarmGetResponseTextListRequest();
            const string xml = @"<AlarmGetResponseTextListRequest>
                                 </AlarmGetResponseTextListRequest>";

            //
            var actual = _serializer.Deserialize(xml);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }
    }
}
