/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace JohnsonControls.XmlRpc
{
    /// <summary>
    /// Indicates an exception occurred while calling an xml rpc.
    /// </summary>
    [Serializable]
    public class ServiceOperationException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="ServiceOperationException"/>
        /// </summary>
        public ServiceOperationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceOperationException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public ServiceOperationException(string message)
            :base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceOperationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ServiceOperationException(string message, Exception innerException)
            :base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceOperationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        /// <param name="faultCode">Fault Code returned by the P2000 v3.x</param>
        public ServiceOperationException(string message, Exception innerException, int faultCode)
            : base(message, innerException)
        {
            _faultCode = faultCode;
        }

        /// <summary>
        /// Initializes a new instance of the ServiceOperationException class with serialized data.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="streamingContext">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param>
        protected ServiceOperationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            info.AddValue("FaultCode", FaultCode);
            base.GetObjectData(info, context);
        }

        #endregion

        private readonly int _faultCode = -1;//Default to an unknown error
        /// <summary>Fault Code returned by the P2000 v3.x</summary>
        public int FaultCode { get { return _faultCode; } }

    }
}
