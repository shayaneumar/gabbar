/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls
{
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Returns true if passed in object has a property or field named <see cref="name"/>
        /// and of type <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the expected data</typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="valueHandler"></param>
        /// <returns></returns>
        public static void TryGetValue<T>(this object obj, string name, Action<T> valueHandler)
        {
            var t = obj.GetType();
            var field = t.GetField(name);
            if (field != null && field.FieldType is T)
            {
                valueHandler((T) field.GetValue(obj));
            }

            var property = t.GetProperty(name, typeof (T));
            if (property != null)
            {
                valueHandler((T)property.GetValue(obj));
            }
        }
    }
}
