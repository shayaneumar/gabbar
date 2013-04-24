/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Container object that holds all of the response filters, results configuration, and data from
    /// <see cref="AlarmService.AlarmGetListEx"/>
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class AlarmGetListExReply
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmGetListExReply"/> class.
        /// </summary>
        public AlarmGetListExReply()
        {
            AlarmMessages = new AlarmMessage[0];
        }

        /// <summary>An AlarmFilter object containing the GUID (Alarm.ItemGuid) used to filter on</summary>
        public AlarmFilter AlarmFilter { get; set; }

        /// <summary>Number of Records Per Page that are be returned</summary>
        public string RecordsPerPage { get; set; }

        /// <summary>A SortOrder object containing an array of SortKeys, and the sort Order</summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>Array of AlarmMessage objects containing the historical attributes of the Alarm</summary>
        public AlarmMessage[] AlarmMessages { get; set; }
    }
}