/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System.Runtime.Serialization;

namespace JohnsonControls.Serialization
{
    public interface IDataSerializer<T>
    {
        string Serialize(T obj);

        /// <exception cref="SerializationException">Thrown if object can not be serialized</exception>
        T Deserialize(string serializedValue);
    }
}
