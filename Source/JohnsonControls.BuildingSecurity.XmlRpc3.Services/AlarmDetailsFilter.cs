/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Specifies filter criteria for <see cref="ITypedAlarmService.AlarmDetails(string,string,string,int,SortOrder)"/>.
    /// </summary>
    /// <remarks>Every property on this class is mutable to facilitate serialization.</remarks>
    public class AlarmDetailsFilter
    {
        /// <summary>
        /// Creates a default instance of <see cref="AlarmDetailsFilter"/>
        /// with a null <see cref="AlarmGuid"/> value.
        /// </summary>
        public AlarmDetailsFilter()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AlarmDetailsFilter"/> with
        /// a GUID (Alarm.ItemGuid) to filter on.
        /// </summary>
        /// <param name="alarmGuid">GUID (Alarm.ItemGuid) of the Alarm to be returned</param>
        public AlarmDetailsFilter(string alarmGuid)
        {
            AlarmGuid = new AlarmGuidFilter {CV = alarmGuid};
        }

        /// <summary>
        /// CriteriaValue object containing the GUID (Alarm.ItemGuid) of the Alarm to be returned
        /// </summary>
        public AlarmGuidFilter AlarmGuid { get; set; }
    }
}