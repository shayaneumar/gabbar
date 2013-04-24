using JohnsonControls.Serialization.Xml;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class P2000AlarmGetListExRequestWrapperTest
    {
        [TestMethod]
        public void P2000AlarmGetListExRequestWrapperConstructorTest()
        {
            const string xml = @"
                <P2000Request>
                  <AlarmGetListExRequest>
                    <AlarmFilter>
                      <Partition>
                        <CV>Super User</CV>
                      </Partition>
                      <AlarmGuid>
                        <CV>235337E0-B192-4259-82AE-C0D10C64C831</CV>
                      </AlarmGuid>
                      <AlarmSiteName>
                        <CV>Site 1</CV>
                      </AlarmSiteName>
                      <AlarmTypeName>
                        <CV>Alarm Type 1</CV>
                      </AlarmTypeName>
                      <ItemName>
                        <CV>Item 1</CV>
                      </ItemName>
                      <OperatorName>
                        <CV>Operator 1</CV>
                      </OperatorName>
                    </AlarmFilter>
                    <RecordsPerPage>50</RecordsPerPage>
                    <SortOrder>
                        <SortKeys>
                            <SortKey SequenceNumber=""1"">
                            <Name>AlarmTimeStamp</Name>
                            </SortKey>
                        </SortKeys>
                        <Order>ASC</Order> 
                    </SortOrder>
                  </AlarmGetListExRequest>
                </P2000Request>";

            P2000AlarmGetListExRequestWrapper expected = (new XmlSerializer<P2000AlarmGetListExRequestWrapper>()).Deserialize(xml);
            var actual =
                new P2000AlarmGetListExRequestWrapper(
                    new AlarmGetListExRequest(
                        new AlarmFilter("Super User", "235337E0-B192-4259-82AE-C0D10C64C831", "Site 1", "Alarm Type 1", "Item 1", "Operator 1"),
                        "50",
                        new SortOrder(new[] { new SortKey("1", "AlarmTimeStamp", null, null) }, "ASC")));

            DtoAssert.AreEqual(expected, actual);
        }
    }
}
