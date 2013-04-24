/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TestExtensions
{
    [TestClass]
    public class CustomExceptionAssertTests
    {
        [TestMethod]
        public void MissingMessageConstructor_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<NotFullyImplementedException_Message>);
        }

        [TestMethod]
        public void MissingMessageAndInnerExceptionConstructor_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<NotFullyImplementedException_MessageInnerException>);
        }

        [TestMethod]
        public void MissingSerailzationConstructor_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<NotFullyImplementedException_SerializationInfoStreamingContext>);
        }

        [TestMethod]
        public void BadMessageConstructor_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<BadlyImplementedException_Message>);
        }

        [TestMethod]
        public void BadMessageAndInnerExceptionConstructorWithBadMessagePassing_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<BadlyImplementedException_MessageEmptyInnerException>);
        }

        [TestMethod]
        public void BadMessageAndInnerExceptionConstructorWithBadExceptionPassing_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<BadlyImplementedException_MessageInnerExceptionNull>);
        }

        [TestMethod]
        public void BadSerailzationConstructor_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<BadlyImplementedException_SerializationInfoStreamingContext>);
        }

        [TestMethod]
        public void MissingSerializableAttribute_Fails()
        {
            ActionAssert.Throws<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<MissingAttributeException_Serializable>);
        }

        [TestMethod]
        public void Exception_Passes()
        {
            ActionAssert.DoesNotThrow<AssertFailedException>(CustomExceptionAssert.HasStandardConstructors<Exception>);
        }

        //The following are some exceptions which should fail a test.
        // ReSharper disable UnusedParameter.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable InconsistentNaming
        [Serializable]
        private class BadlyImplementedException_Message : Exception
        {
            public BadlyImplementedException_Message(string message)
            {
            }

            public BadlyImplementedException_Message(string message, Exception innerException):base(message, innerException)
            {
            }

            protected BadlyImplementedException_Message(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
            {
            }
        }

        [Serializable]
        private class BadlyImplementedException_MessageInnerExceptionNull : Exception
        {
            public BadlyImplementedException_MessageInnerExceptionNull(string message):base(message)
            {
            }

            public BadlyImplementedException_MessageInnerExceptionNull(string message, Exception innerException): base(message, null)
            {
            }

            protected BadlyImplementedException_MessageInnerExceptionNull(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
            {
            }
        }

        [Serializable]
        private class BadlyImplementedException_SerializationInfoStreamingContext : Exception
        {
            public BadlyImplementedException_SerializationInfoStreamingContext(string message):base(message)
            {
            }

            public BadlyImplementedException_SerializationInfoStreamingContext(string message, Exception innerException):base(message, innerException)
            {
            }

            protected BadlyImplementedException_SerializationInfoStreamingContext(SerializationInfo serializationInfo, StreamingContext streamingContext)
            {
            }
        }

        [Serializable]
        private class BadlyImplementedException_MessageEmptyInnerException : Exception
        {
            public BadlyImplementedException_MessageEmptyInnerException(string message)
                : base(message)
            {
            }

            public BadlyImplementedException_MessageEmptyInnerException(string message, Exception innerException)
                : base("", innerException)
            {
            }

            protected BadlyImplementedException_MessageEmptyInnerException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo, streamingContext)
            {
            }
        }

        [Serializable]
        private class NotFullyImplementedException_Message : Exception
        {
            public NotFullyImplementedException_Message(string message, Exception innerException):base(message, innerException)
            {
            }

            protected NotFullyImplementedException_Message(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo,streamingContext)
            {
            }
        }

        [Serializable]
        private class NotFullyImplementedException_MessageInnerException : Exception
        {
            public NotFullyImplementedException_MessageInnerException(string message):base(message)
            {
            }

            protected NotFullyImplementedException_MessageInnerException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo,streamingContext)
            {
            }
        }

        [Serializable]
        private class NotFullyImplementedException_SerializationInfoStreamingContext : Exception
        {
            public NotFullyImplementedException_SerializationInfoStreamingContext(string message):base(message)
            {
            }

            public NotFullyImplementedException_SerializationInfoStreamingContext(string message, Exception innerException):base(message, innerException)
            {
            }
        }

        private class MissingAttributeException_Serializable : Exception
        {
            public MissingAttributeException_Serializable(string message)
                : base(message)
            {
            }

            public MissingAttributeException_Serializable(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            protected MissingAttributeException_Serializable(SerializationInfo serializationInfo, StreamingContext streamingContext)
                : base(serializationInfo, streamingContext)
            {
            }
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedParameter.Local
    }
}
