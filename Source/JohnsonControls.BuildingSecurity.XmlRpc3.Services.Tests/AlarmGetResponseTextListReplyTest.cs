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
    [TestClass]
    public class AlarmGetResponseTextListReplyTest
    {
        private const string FullXmlMessage =
            @"<AlarmGetResponseTextListReply>
                <AlarmResponseTextFilter>
                    <Partition>
                        <CV>Super User</CV>
                    </Partition>
                </AlarmResponseTextFilter>
                <Paging>
                    <Page>0</Page>
                    <RecordsPerPage>50</RecordsPerPage>
                    <RecordCount>1</RecordCount>
                </Paging>
                <SortOrder>
                    <SortKeys>
                        <SortKey SequenceNumber=""1"">
                            <Name>AlarmResponseText</Name>
                            <StartKey>StartKey</StartKey>
                            <LastKey>LastKey</LastKey>
                        </SortKey>
                    </SortKeys>
                    <Order>ASC</Order>
                </SortOrder>
                <ResponseTexts>
                    <ResponseText>
                        <PartitionName>Super User</PartitionName>
                        <Public>1</Public>
                        <AlarmResponseName>Call Police</AlarmResponseName >
                        <AlarmResponseText>Calling 911 and making a log of my call.</AlarmResponseText>
                    </ResponseText >
                    <ResponseText>
                        <PartitionName>Super User</PartitionName>
                        <Public>1</Public>
                        <AlarmResponseName>Propped Door</AlarmResponseName >
                        <AlarmResponseText>Told source of alarm to stop propping the door open.</AlarmResponseText>
                    </ResponseText >
                </ResponseTexts>
            </AlarmGetResponseTextListReply>";

        [TestClass]
        public class Deserialization
        {
            private readonly XmlSerializer<AlarmGetResponseTextListReply> _replySerializer = 
                new XmlSerializer<AlarmGetResponseTextListReply>();
            [TestMethod]
            public void WithEmptyReply()
            {
                // Arrange 
                const string xml = "<AlarmGetResponseTextListReply></AlarmGetResponseTextListReply>";
                var expected = new AlarmGetResponseTextListReply();

                // Act
                var actual = _replySerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void WithFullPayload()
            {
                // Arrange
                var expected = new AlarmGetResponseTextListReply(new AlarmResponseTextFilter("Super User"),
                                                                 new Paging(0, 50, 1),
                                                                 new SortOrder(new[] { new SortKey("1", "AlarmResponseText", "StartKey", "LastKey") }, "ASC"),
                                                                 new ResponseText("Super User", 1, "Call Police",
                                                                                  "Calling 911 and making a log of my call."),
                                                                 new ResponseText("Super User", 1, "Propped Door",
                                                                                  "Told source of alarm to stop propping the door open."));

                // Act
                var actual = _replySerializer.Deserialize(FullXmlMessage);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}
