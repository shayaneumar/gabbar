/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace JohnsonControls
{
    [TestClass]
    public class EnumerableExtensionsTests
    {

        #region GetHashCode Tests
        // Most of the GetDeepHashCode tests don't make any assertions. They just make sure
        // that no exceptions are thrown. I wanted to make sure that null elements, value
        // elements, null list, and empty list were all handled without exceptions.

        [TestMethod]
        public void TestGetHashCodeWithNullElements()
        {
            // should not throw an exception even with null elements.
            IEnumerable<object> objects = new object[] { null, null, null };
            objects.GetDeepHashCode();
        }

        [TestMethod]
        public void TestGetHashCodeWithValueElements()
        {
            // should handle value types fine
            IEnumerable<int> ints = new[] { 7, 3, 5, 2 };
            ints.GetDeepHashCode();
        }

        [TestMethod]
        public void TestGetHashCodeWithNullList()
        {
            ActionAssert.Throws<ArgumentNullException>(() => EnumerableExtensions.GetDeepHashCode<object>(null), "items");
        }

        [TestMethod]
        public void TestGetHashCodeWithEmptyList()
        {
            // should handle empty list without exception
            new object[0].GetDeepHashCode();
        }
        #endregion

        #region ToString Tests
        [TestMethod]
        public void ToString_EmptyList()
        {
            Assert.AreEqual("[]", new int[0].ConvertToString());
        }

        [TestMethod]
        public void ToString_Singleton()
        {
            Assert.AreEqual("[5]", new[] { 5 }.ConvertToString());
        }

        [TestMethod]
        public void ToString_Multiple()
        {
            Assert.AreEqual("[5, 11, 13]", new[] { 5, 11, 13 }.ConvertToString());
        }

        [TestMethod]
        public void ToString_NullCollection_ThrowsException()
        {
            ActionAssert.Throws<ArgumentNullException>(() => EnumerableExtensions.ConvertToString<string>(null), "collection");
        }
        #endregion
    }
}

