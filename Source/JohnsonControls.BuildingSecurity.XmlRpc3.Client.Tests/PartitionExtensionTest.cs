/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    /// <summary>
    ///This is a test class for PartitionResponseExtensionTest and is intended
    ///to contain all PartitionResponseExtensionTest Unit Tests
    ///</summary>
    [TestClass]
    public class PartitionExtensionTest
    {
        /// <summary>
        ///A test for ConvertToPartition
        ///</summary>
        [TestMethod]
        public void ConvertPartitionTest()
        {
            // Arrange
            Guid guid = Guid.NewGuid();

            var rpcpartition = new Services.Partition
            {
                Key = guid.ToString(),
                Name = "Partition Name"
            };

            var expected = new Partition(
                "Partition Name",
                guid
            );

            //Act
            var actual = rpcpartition.ConvertToPartition();

            //Assert
            DtoAssert.AreEqual(expected, actual);      //test against expected object
        }

    }
}
