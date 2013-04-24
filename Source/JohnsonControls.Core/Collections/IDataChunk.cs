/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using JetBrains.Annotations;

namespace JohnsonControls.Collections
{
    /// <summary>
    /// Represents a discrete portion of a larger collection of data.
    /// </summary>
    /// <typeparam name="T">The type of data contained in this chunk.</typeparam>
    public interface IDataChunk<out T>
    {
        /// <summary>
        /// A subset of a collection.
        /// </summary>
        [NotNull]
        IEnumerable<T> Data { get; }

        /// <summary>
        /// The first element in the current chunk
        /// </summary>
        [CanBeNull]
        T FirstElement { get; }

        /// <summary>
        /// The last element in the current chunk
        /// </summary>
        [CanBeNull]
        T LastElement { get; }

        /// <summary>
        /// Is the last chunk of data
        /// </summary>
        bool IsEnd { get; }
    }
}
