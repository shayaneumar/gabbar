/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Reply object returned from <see cref="ITypedAlarmService.AlarmDetails(string,string,string,int,Services.SortOrder)"/>.
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class AlarmDetailsReply
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="AlarmDetailsReply"/> class.
        /// All properties except AlarmHistories will have their C# default value.
        /// AlarmHistories will default to an empty list to match the XML deserializer behavior
        /// when deserializing and empty AlarmDetailsReply element.
        /// </summary>
        public AlarmDetailsReply()
        {
            AlarmHistories = new AlarmHistory[0];
        }

        /// <summary>Object containing the GUID (Alarm.ItemGuid) of the Alarm to be returned</summary>
        public AlarmDetailsFilter AlarmDetailsFilter { get; set; }

        /// <summary>Number of Alarm History Records Per Page to be returned.</summary>
        public string RecordsPerPage { get; set; }

        /// <summary>Class containing attributes (array of SortKeys, and Order) required for Sorting and Paging</summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>Array of AlarmHistory objects containing the details of updates to the Alarm.</summary>
        public AlarmHistory[] AlarmHistories { get; set; }
    }
}