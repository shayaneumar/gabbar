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
    ///This is a test class for PagingTest and is intended
    ///to contain all PagingTest Unit Tests
    ///</summary>
    [TestClass]
    public class PagingTest
    {

        /// <summary>
        /// Test to verify that members set by the constructor are set properly.
        /// </summary>
        [TestMethod]
        public void ConstructorAllMembersTest()
        {
            // Arrange
            var expected = new Paging {PageNumber = 5, RecordsPerPage = 50, RecordCount = 500};

            // Act
            var actual = new Paging(5, 50, 500);

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// TotalRecordsRequest is used to request an instance of a Paging
        /// object configured to request all records.
        ///</summary>
        [TestMethod]
        public void TotalRecordsRequestTest()
        {
            // Arrange
            var expected = new Paging
                                    {
                                        RecordsPerPage = Paging.MaxRecordsPerPage,
                                        RecordCount = Paging.AllRecordsRecordCount
                                    };

            // Act
            Paging actual = Paging.TotalRecordsRequest();

            // Assert
            DtoAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests for Publicly exposed class members.
        /// </summary>
        [TestClass]
        public class PublicMembers
        {

            /// <summary>
            /// Testing PageNumber public accessors.
            ///</summary>
            [TestMethod]
            public void PageNumberTest()
            {
                // Arrange
                const int expected = 1;

                // Act
                var actual = new Paging {PageNumber = 1};

                // Assert
                DtoAssert.AreEqual(expected, actual.PageNumber);
            }

            /// <summary>
            ///A test for RecordsPerPage
            ///</summary>
            [TestMethod]
            public void RecordsPerPageTest()
            {
                // Arrange
                const int expected = 1;

                // Act
                var actual = new Paging {RecordsPerPage = 1};

                // Assert
                DtoAssert.AreEqual(expected, actual.RecordsPerPage);
            }

            /// <summary>
            ///A test for RecordCount
            ///</summary>
            [TestMethod]
            public void TotalRecordCountTest()
            {
                // Arrange
                const int expected = 1;

                // Act
                var actual = new Paging {RecordCount = 1};

                // Assert
                DtoAssert.AreEqual(expected, actual.RecordCount);
            }
        }

        /// <summary>
        /// Tests for the serialization and deserialization of all objects
        /// marked for XML serialization.
        /// </summary>
        [TestClass]
        public class Serialization
        {
            private readonly XmlSerializer<Paging> _pagingSerializer = new XmlSerializer<Paging>();

            [TestMethod]
            public void WithEveryElementSpecified()
            {
                // Arrange
                var expected = new Paging {PageNumber = 4, RecordsPerPage = 50, RecordCount = 500};
                const string xml =
                    @"<Paging>
                            <Page>4</Page>
                            <RecordsPerPage>50</RecordsPerPage>
                            <RecordCount>500</RecordCount>
                        </Paging>";

                // Act
                var actual = _pagingSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            /// <summary>
            /// Test checking whether deserialization is successful with
            /// Partial members present.
            /// </summary>
            [TestMethod]
            public void WithRecordCountElementMissing()
            {
                // Arrange
                var expected = new Paging {PageNumber = 4, RecordsPerPage = 50};
                const string xml =
                    @"<Paging>
                                <Page>4</Page>
                                <RecordsPerPage>50</RecordsPerPage>
                                </Paging>";

                // Act
                var actual = _pagingSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            /// <summary>
            /// Tests deserialization with no members present.
            /// </summary>
            [TestMethod]
            public void WithEmptyPagingElementPresent()
            {
                // Arrange
                var expected = new Paging();
                const string xml = @"<Paging>
                            </Paging>";

                // Act
                var actual = _pagingSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

        }
    }
}
