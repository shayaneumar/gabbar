/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace JohnsonControls.Collections
{
    public static class DictionaryExtensions
    {
        public static TV ValueOrDefault<TK, TV>([NotNull]this IDictionary<TK, TV> dictionary, TK key)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");

            TV value;
            if (dictionary.TryGetValue(key, out value)) return value;
            return default(TV);
        }
    }
}
