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
    public class P2000AlarmGetResponseTextListReplyTest
    {
        [TestClass]
        public class Deserialization
        {
            private readonly XmlSerializer<P2000AlarmGetResponseTextListReply> _replySerializer =
                new XmlSerializer<P2000AlarmGetResponseTextListReply>();

                [TestMethod]
            public void WithNoInnerReply()
            {
                // Arrange
                const string xml = "<P2000Reply></P2000Reply>";
                var expected = new P2000AlarmGetResponseTextListReply();

                // Act
                var actual = _replySerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void WithInnerReply()
            {
                // Arrange
                const string xml =
                    "<P2000Reply><AlarmGetResponseTextListReply></AlarmGetResponseTextListReply></P2000Reply>";
                var expected = new P2000AlarmGetResponseTextListReply(new AlarmGetResponseTextListReply());

                // Act
                var actual = _replySerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}
