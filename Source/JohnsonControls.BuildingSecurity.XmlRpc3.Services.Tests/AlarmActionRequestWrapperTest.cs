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
    ///This is a test class for AlarmActionRequestWrapperTest and is intended
    ///to contain all AlarmActionRequestWrapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class AlarmActionRequestWrapperTest
    {
        private const string FullXmlMessage =
            @"<?xml version=""1.0"" encoding=""utf-16""?>
<P2000Request>
  <AlarmActionRequest>
    <AlarmActionFilter>
      <AlarmGuid>
        <CV>E95A9F3E-A107-4277-AB2E-2A9C6E5525C8</CV>
      </AlarmGuid>
    </AlarmActionFilter>
    <Command>2</Command>
    <Parameters>
      <ConditionSequenceNumber>5</ConditionSequenceNumber>
      <AlarmResponseText>response</AlarmResponseText>
    </Parameters>
  </AlarmActionRequest>
</P2000Request>";

        [TestClass]
        public class Deserialization
        {
            private readonly XmlSerializer<AlarmActionRequestWrapper> _replySerializer =
                new XmlSerializer<AlarmActionRequestWrapper>();

            [TestMethod]
            public void WithEmptyReply()
            {
                // Arrange 
                const string message = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<P2000Request />";
                var expected = new AlarmActionRequestWrapper();

                // Act
                var actual = _replySerializer.Deserialize(message);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void WithFullPayload()
            {
                // Arrange
                var expected = new AlarmActionRequestWrapper
                {
                    AlarmActionRequest =
                        new AlarmActionRequest
                        {
                            AlarmActionFilter =
                                new AlarmActionFilter { AlarmGuid = new MultipleCVAlarmGuidFilter { CurrentValues = new[] { "E95A9F3E-A107-4277-AB2E-2A9C6E5525C8" } } },
                            Command = "2",
                            Parameters =
                                new Parameters
                                {
                                    ConditionSequenceNumber = "5",
                                    AlarmResponseText = "response"
                                }
                        }
                };

                // Act
                var actual = _replySerializer.Deserialize(FullXmlMessage);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}
