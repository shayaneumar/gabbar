using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    
    
    /// <summary>
    ///This is a test class for P2000AlarmDetailsReplyWrapperTest and is intended
    ///to contain all P2000AlarmDetailsReplyWrapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class P2000AlarmDetailsReplyWrapperTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///A test for P2000AlarmDetailsReplyWrapper Constructor
        ///</summary>
        [TestMethod]
        public void P2000AlarmDetailsReplyWrapperConstructorTest()
        {
            const string xml = @"
              <P2000Reply>
                  <AlarmDetailsReply>
                      <AlarmDetailsFilter>
                          <AlarmGuid>
                          <CV>235337E0-B192-4259-82AE-C0D10C64C831</CV>
                          </AlarmGuid>
                      </AlarmDetailsFilter>
                      <RecordsPerPage>50</RecordsPerPage>
                      <SortOrder>
                          <SortKeys>
                              <SortKey SequenceNumber=""1"">
                                <Name>AlarmTimeStamp</Name>
                              </SortKey>
                          </SortKeys>
                          <Order>ASC</Order> 
                      </SortOrder>
                      <AlarmHistories>
                          <AlarmHistory>
                              <AlarmSiteName>MKESite</AlarmSiteName>
                              <OperatorSiteName>MKESite</OperatorSiteName>
                              <PartitionName>Super User</PartitionName>
                              <Public>0</Public>
                              <AlarmHistoryGuid>D02E9777-19C0-4B52-85A7-802ACBA2137B</AlarmHistoryGuid>
                              <AlarmState>4</AlarmState>
                              <AlarmStateName>Pending</AlarmStateName>
                              <AlarmTimestamp>2012-06-08 09:15:35.000</AlarmTimestamp>
                              <ConditionState>1</ConditionState>
                              <ConditionStateName>Set</ConditionStateName>
                              <ConditionTimestamp>2012-06-08 09:15:35.000</ConditionTimestamp>
                              <OperatorName />
                              <Description>Term Down MKE-Reader2, MKE-Reader2, 191-MKE</Description>
                              <Category>P2000</Category>
                              <EscalationLevel>0</EscalationLevel>
                              <ItemName>Term Down MKE-Reader2</ItemName>
                              <AlarmResponseText />
                          </AlarmHistory>
                          <AlarmHistory>
                              <AlarmSiteName>MKESite</AlarmSiteName>
                              <OperatorSiteName>MKESite</OperatorSiteName>
                              <PartitionName>Super User</PartitionName>
                              <Public>0</Public>
                              <AlarmHistoryGuid>73D1E12B-8ECD-4C06-9C64-0A0C3541A34D</AlarmHistoryGuid>
                              <AlarmState>4</AlarmState>
                              <AlarmStateName>Pending</AlarmStateName>
                              <AlarmTimestamp>2012-06-08 09:15:36.000</AlarmTimestamp>
                              <ConditionState>0</ConditionState>
                              <ConditionStateName>Secure</ConditionStateName>
                              <ConditionTimestamp>2012-06-08 09:15:36.000</ConditionTimestamp>
                              <OperatorName />
                              <Description>Term Down MKE-Reader2, MKE-Reader2, 191-MKE</Description>
                              <Category>P2000</Category>
                              <EscalationLevel>0</EscalationLevel>
                              <ItemName>Term Down MKE-Reader2</ItemName>
                              <AlarmResponseText />
                          </AlarmHistory>
                          <AlarmHistory>
                              <AlarmSiteName>MKESite</AlarmSiteName>
                              <OperatorSiteName>MKESite</OperatorSiteName>
                              <PartitionName>Super User</PartitionName>
                              <Public>0</Public>
                              <AlarmHistoryGuid>A3D4C1F9-2198-4AA0-BB62-8DDACA4F6BE0</AlarmHistoryGuid>
                              <AlarmState>3</AlarmState>
                              <AlarmStateName>Acked</AlarmStateName>
                              <AlarmTimestamp>2012-06-14 16:46:53.000</AlarmTimestamp>
                              <ConditionState>0</ConditionState>
                              <ConditionStateName>Secure</ConditionStateName>
                              <ConditionTimestamp>2012-06-08 09:15:36.000</ConditionTimestamp>
                              <OperatorName>ckellav</OperatorName>
                              <Description>Term Down MKE-Reader2, MKE-Reader2, 191-MKE</Description>
                              <Category>P2000</Category>
                              <EscalationLevel>0</EscalationLevel>
                              <ItemName>Term Down MKE-Reader2</ItemName>
                              <AlarmResponseText />
                          </AlarmHistory>
                      </AlarmHistories>
                  </AlarmDetailsReply>
              </P2000Reply>
            ";

            P2000AlarmDetailsReplyWrapper expected = (new XmlSerializer<P2000AlarmDetailsReplyWrapper>()).Deserialize(xml);

            var actual = new P2000AlarmDetailsReplyWrapper
            {
                AlarmDetailsReply =
                    new AlarmDetailsReply
                        {
                            AlarmDetailsFilter =
                                new AlarmDetailsFilter(
                                "235337E0-B192-4259-82AE-C0D10C64C831"),
                            RecordsPerPage = "50",
                            SortOrder =
                                new SortOrder(
                                new[]
                                    {
                                        new SortKey("1", "AlarmTimeStamp",
                                                    null, null)
                                    }, "ASC"),
                            AlarmHistories = new AlarmHistory[3]
                        }
            };

            actual.AlarmDetailsReply.AlarmHistories[0] = new AlarmHistory
            {
                ConditionState = "1",
                ConditionStateName = "Set",
                ConditionTimestamp = "2012-06-08 09:15:35.000",
                Description = "Term Down MKE-Reader2, MKE-Reader2, 191-MKE",
                AlarmState = "4",
                AlarmStateName = "Pending",
                AlarmTimestamp = "2012-06-08 09:15:35.000",
                OperatorName = "",
                OperatorSiteName = "MKESite",
                EscalationLevel = "0",
                AlarmResponseText = "",
                PartitionName = "Super User",
                IsPublic = "0",
                ItemName = "Term Down MKE-Reader2",
                Category = "P2000",
                AlarmHistoryGuid = "D02E9777-19C0-4B52-85A7-802ACBA2137B",
                AlarmSiteName = "MKESite"
             };

            actual.AlarmDetailsReply.AlarmHistories[1] = new AlarmHistory
            {
                ConditionState = "0",
                ConditionStateName = "Secure",
                ConditionTimestamp = "2012-06-08 09:15:36.000",
                Description = "Term Down MKE-Reader2, MKE-Reader2, 191-MKE",
                AlarmState = "4",
                AlarmStateName = "Pending",
                AlarmTimestamp = "2012-06-08 09:15:36.000",
                OperatorName = "",
                OperatorSiteName = "MKESite",
                EscalationLevel = "0",
                AlarmResponseText = "",
                PartitionName = "Super User",
                IsPublic = "0",
                ItemName = "Term Down MKE-Reader2",
                Category = "P2000",
                AlarmHistoryGuid = "73D1E12B-8ECD-4C06-9C64-0A0C3541A34D",
                AlarmSiteName = "MKESite"
            };

            actual.AlarmDetailsReply.AlarmHistories[2] = new AlarmHistory
            {
                ConditionState = "0",
                ConditionStateName = "Secure",
                ConditionTimestamp = "2012-06-08 09:15:36.000",
                Description = "Term Down MKE-Reader2, MKE-Reader2, 191-MKE",
                AlarmState = "3",
                AlarmStateName = "Acked",
                AlarmTimestamp = "2012-06-14 16:46:53.000",
                OperatorName = "ckellav",
                OperatorSiteName = "MKESite",
                EscalationLevel = "0",
                AlarmResponseText = "",
                PartitionName = "Super User",
                IsPublic = "0",
                ItemName = "Term Down MKE-Reader2",
                Category = "P2000",
                AlarmHistoryGuid = "A3D4C1F9-2198-4AA0-BB62-8DDACA4F6BE0",
                AlarmSiteName = "MKESite"
            };

            DtoAssert.AreEqual(expected, actual);
        }
    }
}
