/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace BuildingSecurity.Reporting
{
    public static class Weekdays
    {
        public static string GetName(WeeklyRecurrencePattern dayOfWeek, CultureInfo cultureInfo)
        {
            Dictionary<WeeklyRecurrencePattern, string> names = GetNames(cultureInfo);

            if (!names.ContainsKey(dayOfWeek)) throw new ArgumentOutOfRangeException("dayOfWeek");

            return names[dayOfWeek];
        }

        public static Dictionary<WeeklyRecurrencePattern, string> GetNames(CultureInfo cultureInfo)
        {
            if (!Names.ContainsKey(cultureInfo))
            {
                Names.Add(cultureInfo, new Dictionary<WeeklyRecurrencePattern, string>
                {
                    {WeeklyRecurrencePattern.Sunday,    cultureInfo.DateTimeFormat.DayNames[0]},
                    {WeeklyRecurrencePattern.Monday,    cultureInfo.DateTimeFormat.DayNames[1]},
                    {WeeklyRecurrencePattern.Tuesday,   cultureInfo.DateTimeFormat.DayNames[2]},
                    {WeeklyRecurrencePattern.Wednesday, cultureInfo.DateTimeFormat.DayNames[3]},
                    {WeeklyRecurrencePattern.Thursday,  cultureInfo.DateTimeFormat.DayNames[4]},
                    {WeeklyRecurrencePattern.Friday,    cultureInfo.DateTimeFormat.DayNames[5]},
                    {WeeklyRecurrencePattern.Saturday,  cultureInfo.DateTimeFormat.DayNames[6]}
                });
            }

            return Names[cultureInfo];
        }

        private static readonly Dictionary<CultureInfo, Dictionary<WeeklyRecurrencePattern, string>> Names = new Dictionary<CultureInfo, Dictionary<WeeklyRecurrencePattern, string>>();
    }
}
