/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Diagnostics
{
    [TestClass]
    public class TraceAdapterTest
    {
        private StringBuilder _traceLog;
        private TextWriterTraceListener _customTraceWriter;

        [TestInitialize]
        public void TestSetup()
        {
            _traceLog = new StringBuilder(1000);
            _customTraceWriter = new TextWriterTraceListener(new StringWriter(_traceLog));
            Trace.AutoFlush = true;
            Trace.Listeners.Add(_customTraceWriter);//Hook into trace
        }

        [TestCleanup]
        public void TestTearDown()
        {
            Trace.Listeners.Remove(_customTraceWriter);
            _customTraceWriter.Dispose();
        }

        [TestMethod]
        public void MessageGetsLoggedToTrace()
        {
            //Arrange
            const string testMessage = "TestMessage-1";
            var tLog = new TraceAdapter();

            //Act
            tLog.WriteMessage(testMessage, LogLevel.Information);

            //Assert
            StringAssert.Contains(_traceLog.ToString(), testMessage);
        }
    }
}
