﻿/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace JohnsonControls.XmlRpc
{
    [Serializable]
    public class ServiceTimedOutException : ServiceOperationException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ServiceTimedOutException"/>
        /// </summary>
        public ServiceTimedOutException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTimedOutException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public ServiceTimedOutException(string message)
            :base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTimedOutException class with a specified error message and a reference
        /// to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ServiceTimedOutException(string message, Exception innerException)
            :base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTimedOutException class with serialized data.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="streamingContext">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param>
        protected ServiceTimedOutException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}