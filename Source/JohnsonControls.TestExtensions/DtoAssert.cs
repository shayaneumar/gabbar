/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace JohnsonControls.TestExtensions
{
    public static class DtoAssert
    {
        /// <summary>
        /// Assert that 2 values are equal. Equality will be 
        /// </summary>
        /// <typeparam name="T">The type of <param name="expected"/> and  <param name="actual"/></typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void AreEqual<T>(T expected, T actual)
        {
            //CompareObjects carries state with it so it can't be a static field
            var reflectionComparer = new CompareObjects
                                         {
                                             CompareChildren = true,
                                             CompareFields = true,
                                             CompareProperties = true,
                                             AttributesToIgnore = new List<Type>
                                                                      {
                                                                          typeof (ExcludeFromEqualityAttribute)
                                                                      }
                                         };
            if (reflectionComparer.Compare(expected, actual)) return;

            throw new AssertFailedException(reflectionComparer.DifferencesString);
        }
    }
}
