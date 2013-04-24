/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Runtime.Serialization;

namespace JohnsonControls.BuildingSecurity
{
    [Serializable]
    public class BuildingSecurityAuthenticationFailedException : BuildingSecurityException
    {
        public BuildingSecurityAuthenticationFailedException()
        {
        }

        public BuildingSecurityAuthenticationFailedException(string message)
            :base(message)
        {
        }

        public BuildingSecurityAuthenticationFailedException(string message, Exception innerException)
            :base(message, innerException)
        {
        }

        protected BuildingSecurityAuthenticationFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
