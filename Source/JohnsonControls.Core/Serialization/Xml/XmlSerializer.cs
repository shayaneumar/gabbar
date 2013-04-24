/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/


using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace JohnsonControls.Serialization.Xml
{
    /// <summary>
    /// Serializes and deserializes objects of type T to/from xml strings.
    /// </summary>
    /// <remarks>
    /// This class uses an <see cref="XmlSerializer"/> internally so objects will
    /// be serialized/deserialized according to the rules/conventions of 
    /// <see cref="XmlSerializer"/>.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class XmlSerializer<T> : IDataSerializer<T>
    {
        private readonly XmlSerializer _serializer;
        private readonly XmlSerializerNamespaces _namespaces;

        /// <summary>
        /// Creates an instance of <see cref="XmlSerializer{T}"/>.
        /// </summary>
        public XmlSerializer()
        {
            _serializer = new XmlSerializer(typeof(T));
            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add("", ""); // prevent default namespace garbage
        }

        public static readonly XmlSerializer<T> Instance = new XmlSerializer<T>();

        /// <summary>
        /// Serializes <paramref name="obj"/> to an xml string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(T obj)
        {
            var builder = new StringBuilder();

            using (var sw = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                _serializer.Serialize(sw, obj, _namespaces);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Deserializes the string <paramref name="xml"/> into an object of 
        /// type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public T Deserialize(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                try
                {
                    return (T)_serializer.Deserialize(reader);
                }
                catch(InvalidOperationException)
                {
                    throw new SerializationException();
                }
            }
        }
    }
}
