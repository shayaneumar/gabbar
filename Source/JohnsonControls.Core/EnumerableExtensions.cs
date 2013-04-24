/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace JohnsonControls
{
    /// <summary>
    /// A static class for defining extension methods for IEnumberable.
    /// </summary>
    public static class EnumerableExtensions
    {

        /// <summary>
        /// Creates a string representation of the specified collection.
        /// The format used is a comma separated list of values inside
        /// of brackets. T should override ToString to get a useful result.
        /// </summary>
        /// <exception cref="ArgumentNullException">If collection is null</exception>
        [NotNull]
        [Pure]
        public static string ConvertToString<T>([NotNull]this IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            var builder = new StringBuilder();
            builder.Append("[");

            builder.Append(string.Join(", ", collection));

            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// Returns the hash code for the specified collection. The hash code
        /// is computed from the hash codes of each item in the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="items">A collection of elements</param>
        /// <returns>The hash code of the collection</returns>
        /// <exception cref="ArgumentNullException">If items is null</exception>
        [Pure]
        public static int GetDeepHashCode<T>([NotNull]this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            unchecked
            {
                return items.Aggregate(17,
                                       (current, item) =>
                                       current + ((19*current) + (Equals(item, null) ? 0 : item.GetHashCode())));
            }
        }
    }
}
