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
    public class XmlSerializerTests
    {
        [TestMethod]
        public void XmlSerialization_ObjToXml()
        {
            //Arrange
            var serializer = new XmlSerializer<XmlTestClass>();
            var obj = new XmlTestClass { Value = "DemoValue" };
            
            //Act
            string xmlObj = serializer.Serialize(obj);

            //Assert

            //there is several different ways to serialize, the obj to an xml attribute, but this will detect most of them.
            StringAssert.Contains(xmlObj, "<string ");
            StringAssert.Contains(xmlObj, "value=\"DemoValue\"");
        }

        [TestMethod]
        public void XmlSerialization_XmlToObj()
        {
            //Arrange
            var serializer = new XmlSerializer<XmlTestClass>();

            //Act
            XmlTestClass obj = serializer.Deserialize("<string value=\"TestValue\"/>");

            //Assert
            Assert.AreEqual("TestValue", obj.Value, "Deserialize failed");
        }

        [TestMethod]
        public void XmlSerialization_InvalidXmlThowsSerializationException()
        {
            //Arrange
            var serializer = new XmlSerializer<XmlTestClass>();

            //Act+Assert
            ActionAssert.Throws<SerializationException>(() => serializer.Deserialize("<int val=\"1\"/>"));
        }

        [TestMethod]
        public void XmlSerialization_RoundTrip()
        {
            //Arrange
            var serializer = new XmlSerializer<XmlTestClass>();
            var obj = new XmlTestClass { Value = "DemoValue" };

            //Act
            string xmlObj = serializer.Serialize(obj);
            XmlTestClass obj2 = serializer.Deserialize(xmlObj);

            //Assert
            Assert.AreEqual(obj.Value, obj2.Value, "Round trip with XmlSerializer failed");
        }

        [TestMethod]
        public void XmlSerialization_InstanceValid()
        {
            //Arrange
            var expected = new XmlSerializer<XmlTestClass>();

            //Act
            var actual = XmlSerializer<XmlTestClass>.Instance;

            //Assert
            DtoAssert.AreEqual(expected, actual);
        }
    }
}
