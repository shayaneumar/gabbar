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
    public class P2000LoginReplyWrapperTest
    {
        [TestClass]
        public class XmlDeserializer
        {
            [TestMethod]
            public void WithValidP2000ReplyShouldDeserialize()
            {
                // Arrange
                const string message =
                    "<P2000Reply><P2000LoginReply><SessionInfo/><UserDetails/></P2000LoginReply></P2000Reply>";
                var expected = new P2000LoginReplyWrapper
                                                      {
                                                          P2000LoginReply = new P2000LoginReply
                                                                                {
                                                                                    SessionInfo = new SessionInfo(),
                                                                                    UserDetails = new UserDetails()
                                                                                }
                                                      };
                var serializer = new XmlSerializer<P2000LoginReplyWrapper>();

                // Act
                var actual = serializer.Deserialize(message);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}