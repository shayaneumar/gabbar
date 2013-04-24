/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TestExtensions
{
    public static class ActionAssert
    {
        public static void DoesNotThrow<T>(Action a) where T : Exception
        {
            try
            {
                a();
            }
            catch (T e)
            {
                throw new AssertFailedException("An " + typeof(T).Name + " was thrown, and this was not expected. "+ e.Message);
            }
        }

        public static void Throws<T>(Action a, string parameterName) where T: ArgumentException
        {
            Throws<T>(a, ex =>
            {
                if(ex.ParamName != parameterName)
                {
                    throw new AssertFailedException(string.Format(CultureInfo.InvariantCulture, "Exception was thrown, for {0} expected {1}", ex.ParamName, parameterName));
                }
            });
        }

        public static void Throws<T>(Action a) where T : Exception
        {
            Throws<T>(a, x=> { });
        }

        private static void Throws<T>(Action a, Action<T> exceptionValidator ) where T:Exception
        {
            bool exceptionThrown = false;

            try
            {
                a();
            }
            catch (T ex)
            {
                exceptionValidator(ex);
                exceptionThrown = true;
            }
            catch(Exception ex)
            {
                throw new AssertFailedException(string.Format("Expected an exception of type:{0}, but got an exception of type:{1}", typeof(T).Name, ex.GetType().Name));
            }



            if (!exceptionThrown) throw new AssertFailedException("Exception was expected and not thrown");
        }
    }
}
