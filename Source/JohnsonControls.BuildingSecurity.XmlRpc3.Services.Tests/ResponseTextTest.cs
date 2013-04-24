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
    public class ResponseTextTest
    {
        [TestClass]
        public class Deserialization
        {
            private readonly XmlSerializer<ResponseText> _responseSerializer = new XmlSerializer<ResponseText>();

            [TestMethod]
            public void WithEmptyResponseTextElement()
            {
                // Arrange
                const string xml = "<ResponseText></ResponseText>";
                var expected = new ResponseText();

                // Act
                var actual = _responseSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void WithFullSpecifiedResponseTextEntry()
            {
                // Arrange
                const string xml =
                    @"<ResponseText>
                        <Public>1</Public>
                        <AlarmResponseText>Notify local police office</AlarmResponseText>
                        <PartitionName>Local</PartitionName>
                        <AlarmResponseName>Call Police</AlarmResponseName>
                      </ResponseText>";

                var expected = new ResponseText("Local", 1, "Call Police", "Notify local police office");

                // Act
                var actual = _responseSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}
