/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    public class Partition
    {
        /// <summary>
        /// Creates an immutable Partition object
        /// </summary>
        /// <param name="partitionName">The name of the partition the event occurred on</param>
        /// <param name="partitionIdentifier">The Id of the partition the event occurred on</param>
        public Partition(string partitionName, Guid partitionIdentifier)
        {
            Name = partitionName;
            Identifier = partitionIdentifier;
        }
        
        /// <summary>
        /// The name of the partition.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The unique identifier for the partition.
        /// </summary>
        public Guid Identifier { get; private set; }
    }
}
