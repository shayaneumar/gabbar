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
    ///This is a test class for P2000AlarmGetResponseTextListRequest and is intended
    ///to contain all P2000AlarmGetResponseTextListRequest Unit Tests
    ///</summary>
    [TestClass]
    public class P2000AlarmGetResponseTextListRequestTest
    {
        private readonly XmlSerializer<P2000AlarmGetResponseTextListRequest> _requestSerializer = new XmlSerializer<P2000AlarmGetResponseTextListRequest>();

        /// <summary>
        /// Verify the Default constructor can be called and that it returns a non-Null object.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorTest()
        {
            // Assert no exception thrown
            ActionAssert.DoesNotThrow<Exception>(() => new P2000AlarmGetResponseTextListRequest());
        }

        /// <summary>
        /// Test to verify that members set by the constructor are set properly.
        /// </summary>
        [TestMethod]
        public void ConstructorAllMembersTest()
        {
            // Arrange
            var expected = new AlarmGetResponseTextListRequest(new AlarmResponseTextFilter("Partition1"),
                                                                            new Paging(5, 50, 500),
                                                                            new SortOrder(new[] {new SortKey("1", "SortKeyName", "StartKey", "LastKey")}, "ASC"));

            var getResponseTextObject = new P2000AlarmGetResponseTextListRequest(expected);

            // Act
            var actual = getResponseTextObject.AlarmGetResponseTextListRequest;

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetResponseTextList which is a container for the 
        /// request filter parameters.
        ///</summary>
        [TestMethod]
        public void GetResponseTextList()
        {
            // Arrange
            var expected = new AlarmGetResponseTextListRequest(null, null, new SortOrder(new[] { new SortKey("1", "Field1", null, null) }));
            var requestObj = new P2000AlarmGetResponseTextListRequest(new AlarmGetResponseTextListRequest(null, null, new SortOrder(new[] { new SortKey("1", "Field1", null, null) })));

            // Act
            var actual = requestObj.AlarmGetResponseTextListRequest;

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for deserializing GetResponseTextList
        ///</summary>
        [TestMethod]
        public void DeserializeWithAllElementsPresent()
        {
            // Arrange
            var expected = new P2000AlarmGetResponseTextListRequest(new AlarmGetResponseTextListRequest(new AlarmResponseTextFilter("Partition1"), new Paging(5, 50, 500), new SortOrder(new[] { new SortKey("1", "Column1", null, null) })));
            const string requestObj = @"
                <P2000Request>
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
                    </AlarmGetResponseTextListRequest>
                </P2000Request>";

            // Act
            var actual = _requestSerializer.Deserialize(requestObj);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests deserialization with no members present.
        /// </summary>
        [TestMethod]
        public void DeserializeWithEmptyP2000GetResponseTextListRequestElementPresent()
        {
            // Arrange
            var expected = new P2000AlarmGetResponseTextListRequest();
            const string xml = @"<P2000Request>
                                 </P2000Request>";

            // Act
            var actual = _requestSerializer.Deserialize(xml);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }


    }
}
