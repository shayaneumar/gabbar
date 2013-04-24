/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
namespace BuildingSecurity.Reporting
{
    /// <summary>
    /// Handles QuotaExceededException thrown by WCF Service
    /// System.ServiceModel.CommunicationException occurred
    /// Message=The maximum message size quota for incoming messages (?????) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element.
    /// </summary>
    [Serializable]
    public class ReportingMessageQuotaExceededException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReportingMessageQuotaExceededException"/>
        /// </summary>
        public ReportingMessageQuotaExceededException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingMessageQuotaExceededException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public ReportingMessageQuotaExceededException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingMessageQuotaExceededException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ReportingMessageQuotaExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
