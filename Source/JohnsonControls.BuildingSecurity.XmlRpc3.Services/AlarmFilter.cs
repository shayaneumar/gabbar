/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Specifies filter criteria for <see cref="AlarmService.AlarmGetListEx"/>.
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class AlarmFilter
    {
        /// <summary>
        /// Creates a default instance of <see cref="AlarmFilter"/>
        /// with a null <see cref="AlarmGuid"/> values.
        /// </summary>
        public AlarmFilter()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AlarmFilter"/> with
        /// a GUID (Alarm.ItemGuid) used for filtering.
        /// </summary>
        /// <param name="partition">Name of the Partition to filter on</param>
        /// <param name="alarmGuid">GUID (Alarm.ItemGuid) of the Alarm to be returned</param>
        /// <param name="alarmSiteName">Name of the Site to filter on</param>
        /// <param name="alarmTypeName">Name of the Alarm Type to filter on</param>
        /// <param name="itemName">Name of the Item associated with the Alarm(s) to filter on</param>
        /// <param name="operatorName">Name of the Operator who updated the Alarm(s) to filter on</param>
        public AlarmFilter(string partition, string alarmGuid, string alarmSiteName, string alarmTypeName, string itemName, string operatorName)
        {
            if (partition != null) Partition = new PartitionFilter {CV = partition};
            if (alarmGuid != null) AlarmGuid = new AlarmGuidFilter{CV=alarmGuid};
            if (alarmSiteName != null) AlarmSiteName = new AlarmSiteNameFilter {CV = alarmSiteName};
            if (alarmTypeName != null) AlarmTypeName = new AlarmTypeNameFilter {CV = alarmTypeName};
            if (itemName != null) ItemName = new ItemNameFilter {CV = itemName};
            if (operatorName != null) OperatorName = new OperatorNameFilter {CV = operatorName};
        }

        /// <summary>CriteriaValue object containing the name of the Partition used for filtering</summary>
        public PartitionFilter Partition { get; set; }

        /// <summary>CriteriaValue object containing the GUID (Alarm.ItemGuid) of the Alarm used for filtering</summary>
        public AlarmGuidFilter AlarmGuid { get; set; }

        /// <summary>CriteriaValue object containing the Name of the Site used for filtering</summary>
        public AlarmSiteNameFilter AlarmSiteName { get; set; }

        /// <summary>CriteriaValue object containing the Name of the Alarm Type used for filtering</summary>
        public AlarmTypeNameFilter AlarmTypeName { get; set; }

        /// <summary>CriteriaValue object containing the Name of the Item associated with the Alarm(s) used for filtering</summary>
        public ItemNameFilter ItemName { get; set; }

        /// <summary>CriteriaValue object containing the Name of the Operator who updated the Alarm(s) used for filtering</summary>
        public OperatorNameFilter OperatorName { get; set; }
    }


}