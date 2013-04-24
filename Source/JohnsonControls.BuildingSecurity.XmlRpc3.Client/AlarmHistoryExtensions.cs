/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using JohnsonControls.BuildingSecurity.XmlRpc3.Globalization;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    /// <summary>
    /// Extensions for the <see cref="AlarmHistory"/> class.
    /// </summary>
    public static class AlarmHistoryExtensions
    {
        /// <summary>
        /// Transform a <see cref="AlarmHistory"/> to a <see cref="HistoryEntry"/>.
        /// </summary>
        /// <param name="alarmHistory">The <see cref="AlarmHistory"/> that should be transformed.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>
        /// A <see cref="HistoryEntry"/> created from the contents of the passed in <see cref="AlarmHistory"/>.
        /// </returns>
        public static HistoryEntry ConvertToActionEntry(this AlarmHistory alarmHistory, TimeZoneInfo timeZone)
        {
            if (alarmHistory == null) throw new ArgumentNullException("alarmHistory");

            return new HistoryEntry(
                alarmHistory.AlarmHistoryGuid,
                TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(alarmHistory.AlarmTimestamp, CultureInfo.InvariantCulture), timeZone),
                alarmHistory.OperatorName,
                Translator.GetString(CategoryType.AlarmStates, Convert.ToInt32(alarmHistory.AlarmState, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture),
                Translator.GetString(CategoryType.ConditionStates, Convert.ToInt32(alarmHistory.ConditionState, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture),
                alarmHistory.AlarmResponseText
            );
        }
    }
}
