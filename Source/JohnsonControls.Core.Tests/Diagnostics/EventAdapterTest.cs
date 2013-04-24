/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Diagnostics
{
    [TestClass]
    public class EventAdapterTest
    {
        [TestMethod]
        public void EventAdapter_WithNullElements_ShouldThrowException()
        {
            ActionAssert.Throws<ArgumentException>(() => new EventAdapter(null), "sourceName");
        }
    }
}
