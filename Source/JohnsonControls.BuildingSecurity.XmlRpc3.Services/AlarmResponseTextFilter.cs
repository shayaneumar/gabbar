/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Globalization;
using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Specifies filter criteria for <see cref="ITypedAlarmService.AlarmGetResponseTextList(string,string,SortOrder,Paging)"/>.
    /// </summary>
    /// <remarks>
    /// This class allows you to specify a <see cref="PartitionFilter"/>.
    /// </remarks>
    public class AlarmResponseTextFilter
    {
        /// <summary>
        /// Creates a default instance of <see cref="AlarmResponseTextFilter"/>
        /// with a null <see cref="PartitionFilter"/> value.
        /// </summary>
        public AlarmResponseTextFilter()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AlarmResponseTextFilter"/> with
        /// the specified partition filter.
        /// </summary>
        /// <param name="partitionFilter"></param>
        public AlarmResponseTextFilter(PartitionFilter partitionFilter)
        {
            PartitionFilter = partitionFilter;
        }

        /// <summary>
        /// Creates an instance of <see cref="AlarmResponseTextFilter"/> with
        /// a partition filter for the specified partition name.
        /// </summary>
        /// <param name="partitionName"></param>
        public AlarmResponseTextFilter(string partitionName) : this(new PartitionFilter {CV=partitionName})
        {
        }

        /// <summary>
        /// Gets or sets the partition filter criteria.
        /// </summary>
        [XmlElement("Partition")]
        public PartitionFilter PartitionFilter { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "PartitionFilter: {0}", PartitionFilter);
        }
    }
}
