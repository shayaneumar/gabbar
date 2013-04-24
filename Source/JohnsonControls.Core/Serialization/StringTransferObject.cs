/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System.Runtime.Serialization;

namespace JohnsonControls.Serialization
{
    /// <summary>
    /// This class allows a single string to be used as a DTO. Without the 
    /// DataContract wrapper, a string object can not be serialized by itself.
    /// </summary>
    [DataContract]
    public class StringTransferObject
    {
        public StringTransferObject(string value)
        {
            Value = value;
        }

        [DataMember]
        public string Value { get; private set; }
    }
}
