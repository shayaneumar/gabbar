/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JohnsonControls.Diagnostics
{
    /// <summary>
    ///This is a test class for LogTest and is intended
    ///to contain all LogTest Unit Tests
    ///</summary>
    [TestClass]
    public class LogTest
    {
        public LogTest()
        {
            Log.Logger = null;
        }

        private static Mock<ILogger> CreateMockLoggerThatExpectsMessage(string message, LogLevel level)
        {
            return CreateMockLoggerThatExpectsMessage(x => x == message, l => l == level);
        }

        private static Mock<ILogger> CreateMockLoggerThatExpectsMessage(Expression<Func<string,bool>> messageMatch, Expression<Func<LogLevel,bool>> levelMatch)
        {
            var logMock = new Mock<ILogger>(MockBehavior.Strict);
            logMock.Setup(l => l.WriteMessage(It.Is(messageMatch), It.Is(levelMatch)));
            Log.Logger = logMock.Object;
            return logMock;
        }

        /// <summary>
        ///A test for Verbose formatting
        ///</summary>
        [TestMethod]
        public void VerboseLogTest()
        {
            // Arrange
            Log.LogLevel = LogLevel.Verbose;
            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage("00 Message", LogLevel.Verbose);

            // Act
            Log.Verbose("{0} Message", "00");

            // Assert
            // Verify logging message matches expected.
            mockLogger.VerifyAll();
        }

        /// <summary>
        ///A test for Verbose formatting
        ///</summary>
        [TestMethod]
        public void InformationLogTest()
        {
            // Arrange
            Log.LogLevel = LogLevel.Verbose;
            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage("00 Message", LogLevel.Information);

            // Act
            Log.Information("{0} Message", "00");

            // Assert
            // Verify logging message matches expected.
            mockLogger.VerifyAll();
        }

        /// <summary>
        ///A test for Verbose formatting
        ///</summary>
        [TestMethod]
        public void WarningLogTest()
        {
            // Arrange
            Log.LogLevel = LogLevel.Verbose;
            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage("00 Message", LogLevel.Warning);

            // Act
            Log.Warning("{0} Message", "00");

            // Assert
            // Verify logging message matches expected.
            mockLogger.VerifyAll();
        }

        /// <summary>
        ///A test for Verbose formatting
        ///</summary>
        [TestMethod]
        public void ErrorLogTest()
        {
            // Arrange
            Log.LogLevel = LogLevel.Verbose;
            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage("00 Message", LogLevel.Error);
            // Act
            Log.Error("{0} Message", "00");

            // Assert
            // Verify logging message matches expected.
            mockLogger.VerifyAll();
        }

        /// <summary>
        ///A test for Verbose formatting
        ///</summary>
        [TestMethod]
        public void ErrorLogInvalidFormatTest()
        {
            // Arrange
            Log.LogLevel = LogLevel.Verbose;
            const string invalidFormattingString = "This is an invalid formatting string {}";

            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage(message => message.Contains(invalidFormattingString), l => l == LogLevel.Error);

            // Act
            Log.Error(invalidFormattingString, "00");

            // Assert
            //Verify logged message should contain the raw formatting string because the formatting string was invalid
            mockLogger.VerifyAll();
        }

        [TestMethod]
        public void NoParamsAddedForLogging()
        {
            // Arrange
            Log.LogLevel = LogLevel.Verbose;
            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage("Message", LogLevel.Error);
            // Act
            Log.Error("Message");

            // Assert
            // Verify logging message matches expected.
            mockLogger.VerifyAll();
        }

        [TestMethod]
        public void LogLevelShouldLog()
        {
            // Arrange
            Log.LogLevel = LogLevel.Information;
            Mock<ILogger> mockLogger = CreateMockLoggerThatExpectsMessage("Message", LogLevel.Information);

            // Act
            Log.Information("Message");

            // Assert
            // Verify logging message matches expected.
            mockLogger.VerifyAll();
        }

        [TestMethod]
        public void LogLevelShouldNotLog()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            Log.Logger = loggerMock.Object;
            Log.LogLevel = LogLevel.Error;

            // Act
            Log.Information("Message");

            // Assert
            // Nothing to assert. If any method on loggerMock.Object was called it would do the assert and fail
        }


        /// <summary>
        ///A test for the Logger Accessor.  Verify that by default we get back an instance of TraceAdapter.
        ///</summary>
        [TestMethod]
        public void VerifyTraceAdapterSetAsDefaultLoggerTest()
        {
            // Arrange
            var expected = new TraceAdapter().GetType();

            // Act
            var actual = Log.TraceLogger.GetType();

            // Assert
            Assert.AreSame(expected, actual);
        }

        /// <summary>
        ///A test for the Logger Accessor.  Verify that we get back to same type we entered.
        ///</summary>
        [TestMethod]
        public void VerifyLoggerGetAccessorTest()
        {
            // Arrange
            var expected = new TraceAdapter();
            Log.Logger = expected;

            // Act
            var actual = Log.Logger;

            // Assert
            Assert.AreSame(expected, actual);
        }
    }
}
