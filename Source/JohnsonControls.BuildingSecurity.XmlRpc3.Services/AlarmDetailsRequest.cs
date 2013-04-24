/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Container object that holds all of the request filters and results configuration for
    /// <see cref="ITypedAlarmService.AlarmDetails(string,string,string,int,Services.SortOrder)"/>
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class AlarmDetailsRequest 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmDetailsRequest"/> class.
        /// </summary>
        public AlarmDetailsRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmDetailsRequest"/> class,
        /// with a filter, records / page, and sort order.
        /// </summary>
        /// <param name="alarmDetailsFilter">An AlarmDetailsFilter object containing the GUID (Alarm.ItemGuid) to filter on</param>
        /// <param name="recordsPerPage">Number of Records Per Page to be returned</param>
        /// <param name="sortOrder">A SortOrder object containing an array of SortKeys, and the sort Order</param>
        public AlarmDetailsRequest(AlarmDetailsFilter alarmDetailsFilter, string recordsPerPage, SortOrder sortOrder)
        {
            AlarmDetailsFilter = alarmDetailsFilter;
            RecordsPerPage = recordsPerPage;
            SortOrder = sortOrder;
        }

        /// <summary>An AlarmDetailsFilter object containing the GUID (Alarm.ItemGuid) to filter on</summary>
        public AlarmDetailsFilter AlarmDetailsFilter { get; set; }

        /// <summary>Number of Records Per Page to be returned</summary>
        public string RecordsPerPage { get; set; }

        /// <summary>A SortOrder object containing an array of SortKeys, and the sort Order</summary>
        public SortOrder SortOrder { get; set; }
    }
}