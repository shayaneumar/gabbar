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
    ///This is a test class for AlarmActionReplyWrapperTest and is intended
    ///to contain all AlarmActionReplyWrapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class AlarmActionReplyWrapperTest
    {
        private const string FullXmlMessage =
             @"<?xml version='1.0' encoding='utf-16' ?> 
                    <P2000Reply>
                    <AlarmActionReply>
                    <AlarmActionFilter>
                    <AlarmGuid>
                    <CV>E95A9F3E-A107-4277-AB2E-2A9C6E5525C8</CV> 
                    </AlarmGuid>
                    </AlarmActionFilter>
                    <Command>2</Command> 
                    <Parameters>
                    <ConditionSequenceNumber>191</ConditionSequenceNumber> 
                    <AlarmResponseText>response</AlarmResponseText> 
                    </Parameters>
                    <AlarmActionResponses>
                    <AlarmActionResponse>
                    <AlarmGuid>E95A9F3E-A107-4277-AB2E-2A9C6E5525C8</AlarmGuid> 
                    <AlarmActionStatus>1</AlarmActionStatus> 
                    </AlarmActionResponse>
                    </AlarmActionResponses>
                    </AlarmActionReply>
                    </P2000Reply>";

        [TestClass]
        public class Deserialization
        {
            private readonly XmlSerializer<AlarmActionReplyWrapper> _serializer = new XmlSerializer<AlarmActionReplyWrapper>();

            [TestMethod]
            public void WithEmptyReply()
            {
                // Arrange 
                const string response = "<P2000Reply></P2000Reply>";
                var expected = new AlarmActionReplyWrapper();

                // Act
                var actual = _serializer.Deserialize(response);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void WithFullPayload()
            {
                // Arrange
                var expected = new AlarmActionReplyWrapper
                                   {
                                       AlarmActionReply = new AlarmActionReply
                                                              {
                                                                  AlarmActionFilter =
                                                                      new AlarmActionFilter
                                                                          {
                                                                              AlarmGuid =
                                                                                  new MultipleCVAlarmGuidFilter
                                                                                      {
                                                                                          CurrentValues =
                                                                                              new[]
                                                                                                  {
                                                                                                      "E95A9F3E-A107-4277-AB2E-2A9C6E5525C8"
                                                                                                  }
                                                                                      }
                                                                          },
                                                                  Command = "2",
                                                                  Parameters =
                                                                      new Parameters
                                                                          {
                                                                              ConditionSequenceNumber = "191",
                                                                              AlarmResponseText = "response"
                                                                          },
                                                                  AlarmActionResponses = new[] { new AlarmActionResponse { AlarmActionStatus = "1", AlarmGuid = "E95A9F3E-A107-4277-AB2E-2A9C6E5525C8" } }
                                                                          
                                                              }
                                   };

                // Act
                var actual = _serializer.Deserialize(FullXmlMessage);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}
