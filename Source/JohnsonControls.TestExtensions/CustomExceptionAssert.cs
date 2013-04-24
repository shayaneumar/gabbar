/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TestExtensions
{
    public class CustomExceptionAssert
    {
        public static void HasStandardConstructors<T>() where T : Exception
        {
            const string customMessage = @"C123";
            const string innerExceptionMessage = @"innerC123";
            var innerException = new Exception(innerExceptionMessage);
            var typeName = typeof(T).Name;
            var messageConstructor = typeof(T).GetConstructor(new[] { typeof(string) });
            var innerExceptionConstructor = typeof(T).GetConstructor(new[] {typeof(string), typeof(Exception)});

            if (messageConstructor == null)
                throw new AssertFailedException("Did not find a constructor matching " + typeName+"(string)");
            if (innerExceptionConstructor == null)
                throw new AssertFailedException("Did not find a constructor matching " + typeName + "(string, Exception)");

            //Test the constructor taking only a message
            var mcResult = (Exception)messageConstructor.Invoke(new object[] { customMessage });

            if (!customMessage.Equals(mcResult.Message, StringComparison.OrdinalIgnoreCase))
                throw new AssertFailedException(typeName + "(string) created an exception with different message than what was passed.");

            //Test the constructor taking an inner exception
            var inExResult = (Exception)innerExceptionConstructor.Invoke(new object[] { customMessage, innerException });

            if (!customMessage.Equals(inExResult.Message, StringComparison.OrdinalIgnoreCase))
                throw new AssertFailedException(typeName + "(string, Exception) created an exception with different message than what was passed.");

            if (!innerException.Equals(inExResult.InnerException))
                throw new AssertFailedException(typeName + "(string, Exception) created an exception with different inner exception than what was passed.");

            //Test the serialization constructor
            try
            {
                var formatter = new BinaryFormatter();
                Exception strResult;
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, inExResult);

                    //Read the exception back out
                    ms.Seek(0, 0);
                    strResult = (Exception)formatter.Deserialize(ms);
                }
                //Equals on the exception likely won't work, so comparing the 'ToString()' is the next best thing
                if (!inExResult.ToString().Equals(strResult.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new AssertFailedException(typeName + " failed serialization round trip");
                }
            }
            catch (SerializationException)
            {
                throw new AssertFailedException(typeName + " failed to serialize. SerializableAttribute maybe missing");
            }
        }
    }
}
