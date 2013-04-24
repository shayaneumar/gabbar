/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Serialization.Xml
{
    [TestClass]
    public class DataSerializerFactoryTests
    {
        [TestMethod]
        public void DataContractObject_ReturnsDataSerializer()
        {
            //Arrange
            var factory = new DataSerializerFactory();

            //Act
            var serializer = factory.GetSerializer<DataContractTestClass>();

            //Assert
            Assert.IsNotNull(serializer as DataSerializer<DataContractTestClass>, "failed to select DataSerializer for data contract");
        }

        [TestMethod]
        public void NondataContractObject_ReturnsXmlSerializer()
        {
            //Arrange
            var factory = new DataSerializerFactory();

            //Act
            var serializer = factory.GetSerializer<XmlTestClass>();

            //Assert
            Assert.IsNotNull(serializer as XmlSerializer<XmlTestClass>, "failed to select XmlSerializer for non data contract");
        }
    }
}
