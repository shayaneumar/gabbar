/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Runtime.Serialization;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Serialization.Xml
{
    [TestClass]
    public class DataSerializerTests
    {
        [TestClass]
        public class Serialize
        {
            [TestMethod]
            public void ValidObject_SerializesCorrectly()
            {
                //Arrange
                var serializer = new DataSerializer<DataContractTestClass>();
                var validObject = new DataContractTestClass {Value = "So Meta"};

                //Act
                string xml = serializer.Serialize(validObject);

                //Assert
                StringAssert.Contains(xml, "<DataContractTestClass ");
                StringAssert.Contains(xml, "<Value>So Meta</Value>");
            }
        }

        [TestClass]
        public class Deserialize
        {
            [TestMethod]
            public void ValidXml_DeserializedCorrectly()
            {
                //Arrange
                var serializer = new DataSerializer<DataContractTestClass>();

                //Act
                DataContractTestClass obj = serializer.Deserialize("<DataContractTestClass xmlns=\"http://schemas.datacontract.org/2004/07/JohnsonControls.Serialization.Xml\"><Value>TestValue</Value></DataContractTestClass>");
                //Note that DataSerializer is very picky about the xmlns being present
                //Assert
                Assert.AreEqual("TestValue",obj.Value);
            }

            [TestMethod]
            public void InvalidXml_ThowsSerializationException()
            {
                //Arrange
                var serializer = new DataSerializer<DataContractTestClass>();

                //Act+Assert
                ActionAssert.Throws<SerializationException>(() => serializer.Deserialize("<int val=\"1\"/>"));
            }
        }


        [TestMethod]
        public void DataContractSerialization_RoundTrip()
        {
            //Arrange
            var serializer = new DataSerializer<DataContractTestClass>();
            var obj = new DataContractTestClass { Value = "DemoValue" };

            //Act
            string xml = serializer.Serialize(obj);
            DataContractTestClass obj2 = serializer.Deserialize(xml);

            //Assert
            Assert.AreEqual(obj.Value, obj2.Value, "Round trip with DataSerializer failed");
        }
    }
}
