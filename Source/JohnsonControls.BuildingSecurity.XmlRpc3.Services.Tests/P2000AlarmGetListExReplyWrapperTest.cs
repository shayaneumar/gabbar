using System.Net;
using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class P2000AlarmGetListExReplyWrapperTest
    {
        [TestMethod]
        public void P2000AlarmGetListExReplyWrapperConstructorTest()
        {
            const string xml = @"
                <P2000Reply>
                    <AlarmGetListExReply>
                        <AlarmFilter />
                        <SortOrder>
                            <SortKeys>
                                <SortKey SequenceNumber=""0"">
                                    <Name>AlarmTimeStamp</Name>
                                </SortKey>
                                <SortKey SequenceNumber=""1"">
                                    <Name>AlarmGuid</Name>
                                </SortKey>
                            </SortKeys>
                            <Order>ASC</Order>
                        </SortOrder>
                        <AlarmMessages>
                            <AlarmMessage>&lt;MessageBase&gt;&lt;BaseVersion&gt;301&lt;/BaseVersion&gt;&lt;MessageType&gt;3&lt;/MessageType&gt;&lt;MessageSubType&gt;2&lt;/MessageSubType&gt;&lt;SiteName&gt;MKESite&lt;/SiteName&gt;&lt;PartitionName&gt;Super User&lt;/PartitionName&gt;&lt;Public&gt;0&lt;/Public&gt;&lt;ItemName&gt;Term Down sim t2&lt;/ItemName&gt;&lt;QueryString&gt;&lt;/QueryString&gt;&lt;Category&gt;P2000&lt;/Category&gt;&lt;Escalation&gt;0&lt;/Escalation&gt;&lt;Priority&gt;0&lt;/Priority&gt;&lt;OperatorName&gt;cardkey&lt;/OperatorName&gt;&lt;HistoryFilterKey&gt;2&lt;/HistoryFilterKey&gt;&lt;HistoryFilterName&gt;Alarm&lt;/HistoryFilterName&gt;&lt;TimeStampUTC&gt;&lt;/TimeStampUTC&gt;&lt;/MessageBase&gt;&lt;MessageDecode&gt;&lt;MessageDateTime&gt;6/13/2012 3:31:06 PM&lt;/MessageDateTime&gt;&lt;MessageTypeText&gt;Alarm&lt;/MessageTypeText&gt;&lt;MessageText&gt;Acked, Set&lt;/MessageText&gt;&lt;DetailsText&gt;Term Down sim t2, sim t2, sim&lt;/DetailsText&gt;&lt;/MessageDecode&gt;&lt;MessageDetails&gt;&lt;MessageVersion&gt;301&lt;/MessageVersion&gt;&lt;AlarmGuid&gt;E95A9F3E-A107-4277-AB2E-2A9C6E5525C8&lt;/AlarmGuid&gt;&lt;AlarmID&gt;10&lt;/AlarmID&gt;&lt;AlarmType&gt;2&lt;/AlarmType&gt;&lt;AlarmOptionsGuid&gt;CB23440D-5EFB-4901-A67B-E842209BEBB6&lt;/AlarmOptionsGuid&gt;&lt;AlarmTypeName&gt;Input Point&lt;/AlarmTypeName&gt;&lt;AlarmTypeID&gt;21&lt;/AlarmTypeID&gt;&lt;AckRequired&gt;0&lt;/AckRequired&gt;&lt;ResponseRequired&gt;0&lt;/ResponseRequired&gt;&lt;InstructionText&gt;&lt;/InstructionText&gt;&lt;AlarmState&gt;3&lt;/AlarmState&gt;&lt;AlarmStateName&gt;Acked&lt;/AlarmStateName&gt;&lt;AlarmTimestamp&gt;2012-06-13T15:31:06&lt;/AlarmTimestamp&gt;&lt;ConditionState&gt;1&lt;/ConditionState&gt;&lt;ConditionStateName&gt;Set&lt;/ConditionStateName&gt;&lt;ConditionSequenceNumber&gt;173&lt;/ConditionSequenceNumber&gt;&lt;ConditionCompletionState&gt;0&lt;/ConditionCompletionState&gt;&lt;ConditionCompletionStateName&gt;Secure&lt;/ConditionCompletionStateName&gt;&lt;ConditionTimestamp&gt;2012-06-08T14:33:06&lt;/ConditionTimestamp&gt;&lt;ConditionTimestampUTC&gt;2012-06-08T14:33:06+08:00&lt;/ConditionTimestampUTC&gt;&lt;Popup&gt;1&lt;/Popup&gt;&lt;Description&gt;Term Down sim t2, sim t2, sim&lt;/Description&gt;&lt;AlarmSiteName&gt;MKESite&lt;/AlarmSiteName&gt;&lt;AlarmResponseText&gt;&lt;/AlarmResponseText&gt;&lt;CanAcknowledge&gt;0&lt;/CanAcknowledge&gt;&lt;CanRespond&gt;1&lt;/CanRespond&gt;&lt;CanComplete&gt;0&lt;/CanComplete&gt;&lt;/MessageDetails&gt;</AlarmMessage>
                            <AlarmMessage>&lt;MessageBase&gt;&lt;BaseVersion&gt;301&lt;/BaseVersion&gt;&lt;MessageType&gt;3&lt;/MessageType&gt;&lt;MessageSubType&gt;2&lt;/MessageSubType&gt;&lt;SiteName&gt;MKESite&lt;/SiteName&gt;&lt;PartitionName&gt;Super User&lt;/PartitionName&gt;&lt;Public&gt;0&lt;/Public&gt;&lt;ItemName&gt;Term Down sim t1&lt;/ItemName&gt;&lt;QueryString&gt;&lt;/QueryString&gt;&lt;Category&gt;P2000&lt;/Category&gt;&lt;Escalation&gt;0&lt;/Escalation&gt;&lt;Priority&gt;0&lt;/Priority&gt;&lt;OperatorName&gt;cardkey&lt;/OperatorName&gt;&lt;HistoryFilterKey&gt;2&lt;/HistoryFilterKey&gt;&lt;HistoryFilterName&gt;Alarm&lt;/HistoryFilterName&gt;&lt;TimeStampUTC&gt;&lt;/TimeStampUTC&gt;&lt;/MessageBase&gt;&lt;MessageDecode&gt;&lt;MessageDateTime&gt;6/13/2012 3:31:58 PM&lt;/MessageDateTime&gt;&lt;MessageTypeText&gt;Alarm&lt;/MessageTypeText&gt;&lt;MessageText&gt;Acked, Set&lt;/MessageText&gt;&lt;DetailsText&gt;Term Down sim t1, sim t1, sim&lt;/DetailsText&gt;&lt;/MessageDecode&gt;&lt;MessageDetails&gt;&lt;MessageVersion&gt;301&lt;/MessageVersion&gt;&lt;AlarmGuid&gt;38D489C4-0388-4E1C-9F06-163C0AED54B7&lt;/AlarmGuid&gt;&lt;AlarmID&gt;11&lt;/AlarmID&gt;&lt;AlarmType&gt;2&lt;/AlarmType&gt;&lt;AlarmOptionsGuid&gt;3A92B98C-9962-44FB-B9F0-AC9983601C5D&lt;/AlarmOptionsGuid&gt;&lt;AlarmTypeName&gt;Input Point&lt;/AlarmTypeName&gt;&lt;AlarmTypeID&gt;20&lt;/AlarmTypeID&gt;&lt;AckRequired&gt;0&lt;/AckRequired&gt;&lt;ResponseRequired&gt;0&lt;/ResponseRequired&gt;&lt;InstructionText&gt;&lt;/InstructionText&gt;&lt;AlarmState&gt;3&lt;/AlarmState&gt;&lt;AlarmStateName&gt;Acked&lt;/AlarmStateName&gt;&lt;AlarmTimestamp&gt;2012-06-13T15:31:58&lt;/AlarmTimestamp&gt;&lt;ConditionState&gt;1&lt;/ConditionState&gt;&lt;ConditionStateName&gt;Set&lt;/ConditionStateName&gt;&lt;ConditionSequenceNumber&gt;191&lt;/ConditionSequenceNumber&gt;&lt;ConditionCompletionState&gt;0&lt;/ConditionCompletionState&gt;&lt;ConditionCompletionStateName&gt;Secure&lt;/ConditionCompletionStateName&gt;&lt;ConditionTimestamp&gt;2012-06-08T14:32:05&lt;/ConditionTimestamp&gt;&lt;ConditionTimestampUTC&gt;2012-06-08T14:32:05+08:00&lt;/ConditionTimestampUTC&gt;&lt;Popup&gt;1&lt;/Popup&gt;&lt;Description&gt;Term Down sim t1, sim t1, sim&lt;/Description&gt;&lt;AlarmSiteName&gt;MKESite&lt;/AlarmSiteName&gt;&lt;AlarmResponseText&gt;&lt;/AlarmResponseText&gt;&lt;CanAcknowledge&gt;0&lt;/CanAcknowledge&gt;&lt;CanRespond&gt;1&lt;/CanRespond&gt;&lt;CanComplete&gt;0&lt;/CanComplete&gt;&lt;/MessageDetails&gt;</AlarmMessage>
                        </AlarmMessages>
                    </AlarmGetListExReply>
                </P2000Reply>";

            P2000AlarmGetListExReplyWrapper expected = (new XmlSerializer<P2000AlarmGetListExReplyWrapper>()).Deserialize(WebUtility.HtmlDecode(xml));

            var actual = new P2000AlarmGetListExReplyWrapper
            {
                AlarmGetListExReply = new AlarmGetListExReply
                {
                    AlarmFilter = new AlarmFilter(),
                    SortOrder = new SortOrder(new[]
                    {
                        new SortKey("0", "AlarmTimeStamp", null, null),
                        new SortKey("1", "AlarmGuid", null, null)
                    }, "ASC"),
                    AlarmMessages = new[]
                    {
                        new AlarmMessage{
                            MessageBase=new MessageBase{BaseVersion="301", MessageType="3", MessageSubtype="2", SiteName="MKESite", PartitionName="Super User", IsPublic="0", ItemName="Term Down sim t2", QueryString="", Category="P2000", Escalation="0", Priority="0", OperatorName="cardkey", HistoryFilterKey="2", HistoryFilterName="Alarm", TimestampUtc=""},
                            MessageDecode = new MessageDecode{MessageDateTime="6/13/2012 3:31:06 PM", MessageTypeText="Alarm", MessageText="Acked, Set", DetailsText="Term Down sim t2, sim t2, sim"},
                            MessageDetails = new AlarmMessageDetails{MessageVersion="301", AlarmGuid="E95A9F3E-A107-4277-AB2E-2A9C6E5525C8", AlarmId="10", AlarmType="2", AlarmOptionsGuid="CB23440D-5EFB-4901-A67B-E842209BEBB6", AlarmTypeName="Input Point", AlarmTypeId="21", AcknowledgeRequired="0", ResponseRequired="0", InstructionText="", AlarmState="3", AlarmStateName="Acked", AlarmTimestamp="2012-06-13T15:31:06", ConditionState="1", ConditionStateName="Set", ConditionSequenceNumber="173", ConditionCompletionState="0", ConditionCompletionStateName="Secure", ConditionTimestamp="2012-06-08T14:33:06", ConditionTimestampUtc="2012-06-08T14:33:06+08:00", Popup="1", Description="Term Down sim t2, sim t2, sim", AlarmSiteName="MKESite", AlarmResponseText="", CanAcknowledge="0", CanRespond="1", CanComplete="0"}},
                        new AlarmMessage{
                            MessageBase=new MessageBase{BaseVersion="301", MessageType="3", MessageSubtype="2", SiteName="MKESite", PartitionName="Super User", IsPublic="0", ItemName="Term Down sim t1", QueryString="", Category="P2000", Escalation="0", Priority="0", OperatorName="cardkey", HistoryFilterKey="2", HistoryFilterName="Alarm", TimestampUtc=""},
                            MessageDecode = new MessageDecode{MessageDateTime="6/13/2012 3:31:58 PM", MessageTypeText="Alarm", MessageText="Acked, Set", DetailsText="Term Down sim t1, sim t1, sim"},
                            MessageDetails = new AlarmMessageDetails{MessageVersion="301", AlarmGuid="38D489C4-0388-4E1C-9F06-163C0AED54B7", AlarmId="11", AlarmType="2", AlarmOptionsGuid="3A92B98C-9962-44FB-B9F0-AC9983601C5D", AlarmTypeName="Input Point", AlarmTypeId="20", AcknowledgeRequired="0", ResponseRequired="0", InstructionText="", AlarmState="3", AlarmStateName="Acked", AlarmTimestamp="2012-06-13T15:31:58", ConditionState="1", ConditionStateName="Set", ConditionSequenceNumber="191", ConditionCompletionState="0", ConditionCompletionStateName="Secure", ConditionTimestamp="2012-06-08T14:32:05", ConditionTimestampUtc="2012-06-08T14:32:05+08:00", Popup="1", Description="Term Down sim t1, sim t1, sim", AlarmSiteName="MKESite", AlarmResponseText="", CanAcknowledge="0", CanRespond="1", CanComplete="0"}}
                    }
                }
            };

            DtoAssert.AreEqual(expected, actual);
        }
    }
}
