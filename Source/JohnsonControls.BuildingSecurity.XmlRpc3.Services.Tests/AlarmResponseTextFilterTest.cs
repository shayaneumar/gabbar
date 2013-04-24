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
    public class AlarmResponseTextFilterTest
    {
        [TestClass]
        public class Deserialization
        {
            private readonly XmlSerializer<AlarmResponseTextFilter> _filterSerializer =
                new XmlSerializer<AlarmResponseTextFilter>();

            [TestMethod]
            public void WithPartitionElement()
            {
                // Arrange
                var expected = new AlarmResponseTextFilter("Super User");

                const string xml =
                    @"<AlarmResponseTextFilter>
                        <Partition>
                          <CV>Super User</CV>
                        </Partition>
                      </AlarmResponseTextFilter>";

                // Act
                var actual = _filterSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void WithNoPartitionElement()
            {
                // Arrange
                var expected = new AlarmResponseTextFilter();

                const string xml = @"<AlarmResponseTextFilter></AlarmResponseTextFilter>";

                // Act
                var actual = _filterSerializer.Deserialize(xml);

                // Assert
                DtoAssert.AreEqual(expected, actual);
            }
        }
    }
}
