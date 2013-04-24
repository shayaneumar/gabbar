/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Linq;
using System.Runtime.Serialization;

namespace JohnsonControls.Serialization.Xml
{
    public class DataSerializerFactory : IDataSerializerFactory
    {
        public IDataSerializer<T> GetSerializer<T>()
        {
            var isDataContract = typeof (T).GetCustomAttributes(typeof(DataContractAttribute),true).Any();
            if (isDataContract)//Default to data contract serializer
            {
                return new DataSerializer<T>();
            }

            return new XmlSerializer<T>();
        }
    }
}
