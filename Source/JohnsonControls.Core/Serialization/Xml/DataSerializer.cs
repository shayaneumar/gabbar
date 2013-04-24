/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace JohnsonControls.Serialization.Xml
{
    /// <summary>
    /// Provides a serialization mechanism for data contracts.
    /// </summary>
    public class DataSerializer<T> : IDataSerializer<T>
    {
        private readonly DataContractSerializer _serializer;

        public DataSerializer()
        {
            _serializer = new DataContractSerializer(typeof(T));
        }

        public string Serialize(T obj)
        {
            var builder = new StringBuilder();

            using (var sw = XmlWriter.Create(new StringWriter(builder, CultureInfo.InvariantCulture)))
            {
                _serializer.WriteObject(sw,obj);
            }

            return builder.ToString();
        }

        public T Deserialize(string serializedValue)
        {
            using (var reader = XmlReader.Create(new StringReader(serializedValue)))
            {
                return (T)_serializer.ReadObject(reader);
            }
        }
    }
}
