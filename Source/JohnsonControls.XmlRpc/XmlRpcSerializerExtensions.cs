/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.IO;
using CookComputing.XmlRpc;

namespace JohnsonControls.XmlRpc
{
    public static class XmlRpcSerializerExtensions
    {
        /// <summary>
        /// Deserializes the string response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static T DeserializeStringResponse<T>(this XmlRpcSerializer serializer, string response)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            using (var reader = new StringReader(response))
            {
                var responseObject = serializer.DeserializeResponse(reader, typeof (T));
                return (T) responseObject.retVal;
            }
        }
    }
}
