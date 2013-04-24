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
    public class BuildingSecurityException : Exception
    {
        public BuildingSecurityException()
        {
        }

        public BuildingSecurityException(string message)
            :base(message)
        {
        }

        public BuildingSecurityException(string message, Exception innerException)
            :base(message, innerException)
        {
        }

        protected BuildingSecurityException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
