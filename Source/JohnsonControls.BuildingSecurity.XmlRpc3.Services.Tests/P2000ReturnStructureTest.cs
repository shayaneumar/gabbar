/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using CookComputing.XmlRpc;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnsonControls.XmlRpc;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    [TestClass]
    public class P2000ReturnStructureTest
    {
        [TestClass]
        public class XmlRpcDeserializer
        {
            
            [TestMethod]
            public void WithXmlDocPropertyPresentShouldDeserializeProperly()
            {
                // Arrange
                var xmlSerializer = new XmlRpcSerializer();
                const string reply = @"<?xml version='1.0'?>
                        <methodResponse><params><param>
                        <value><struct><member><name>XmlDoc</name><value>this is a string</value></member></struct></value>
                        </param></params></methodResponse>";
                var expected = new P2000ReturnStructure
                                    {
                                        XmlDoc = "this is a string"
                                    };

                // Act
                var actual = xmlSerializer.DeserializeStringResponse<P2000ReturnStructure>(reply);

                // Assert
                DtoAssert.AreEqual(expected, actual);

            }
        }
    }
}
