/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    public static class PartitionExtensions
    {
        /// <summary>
        /// Does the transformation from RPC PartitionResponse to a BuildingSecurity domain Partition.
        /// </summary>
        /// <param name="source">P2000 PartitionResponse object to be converted to a BuildingSecurity domain object</param>
        /// <returns>BuildingSecurity domain object for a Partition, transformed from the specified P2000 PartitionResponse source</returns>
        public static Partition ConvertToPartition(this Services.Partition source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new Partition(source.Name, new Guid(source.Key));
        }
    }
}