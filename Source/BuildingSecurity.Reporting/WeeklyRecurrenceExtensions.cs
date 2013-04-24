/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using BuildingSecurity.Reporting.ReportingService;
using System.Globalization;

namespace BuildingSecurity.Reporting
{
    public static class WeeklyRecurrenceExtensions
    {
        public static string DayOfWeek(this WeeklyRecurrence weeklyRecurrence)
        {
            string dayOfWeek = "";

            // TODO: Consider appending Day(s) of Week, instead of only returning the last
            if (weeklyRecurrence.DaysOfWeek.Sunday)     dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Sunday,      CultureInfo.CurrentCulture);
            if (weeklyRecurrence.DaysOfWeek.Monday)     dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Monday,      CultureInfo.CurrentCulture);
            if (weeklyRecurrence.DaysOfWeek.Tuesday)    dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Tuesday,     CultureInfo.CurrentCulture);
            if (weeklyRecurrence.DaysOfWeek.Wednesday)  dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Wednesday,   CultureInfo.CurrentCulture);
            if (weeklyRecurrence.DaysOfWeek.Thursday)   dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Thursday,    CultureInfo.CurrentCulture);
            if (weeklyRecurrence.DaysOfWeek.Friday)     dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Friday,      CultureInfo.CurrentCulture);
            if (weeklyRecurrence.DaysOfWeek.Saturday)   dayOfWeek = Weekdays.GetName(Reporting.WeeklyRecurrencePattern.Saturday,    CultureInfo.CurrentCulture);

            return dayOfWeek;
        }

        public static WeeklyRecurrencePattern WeeklyRecurrencePattern(this WeeklyRecurrence weeklyRecurrance)
        {
            if (weeklyRecurrance.DaysOfWeek.Sunday)     return Reporting.WeeklyRecurrencePattern.Sunday;
            if (weeklyRecurrance.DaysOfWeek.Monday)     return Reporting.WeeklyRecurrencePattern.Monday;
            if (weeklyRecurrance.DaysOfWeek.Tuesday)    return Reporting.WeeklyRecurrencePattern.Tuesday;
            if (weeklyRecurrance.DaysOfWeek.Wednesday)  return Reporting.WeeklyRecurrencePattern.Wednesday;
            if (weeklyRecurrance.DaysOfWeek.Thursday)   return Reporting.WeeklyRecurrencePattern.Thursday;
            if (weeklyRecurrance.DaysOfWeek.Friday)     return Reporting.WeeklyRecurrencePattern.Friday;
            if (weeklyRecurrance.DaysOfWeek.Saturday)   return Reporting.WeeklyRecurrencePattern.Saturday;

            return Reporting.WeeklyRecurrencePattern.Sunday;
        }

        public static ReportFrequency ReportFrequency(this WeeklyRecurrence weeklyRecurrance)
        {
            if (weeklyRecurrance.DaysOfWeek.Sunday &&
                weeklyRecurrance.DaysOfWeek.Monday &&
                weeklyRecurrance.DaysOfWeek.Tuesday &&
                weeklyRecurrance.DaysOfWeek.Wednesday &&
                weeklyRecurrance.DaysOfWeek.Thursday &&
                weeklyRecurrance.DaysOfWeek.Friday &&
                weeklyRecurrance.DaysOfWeek.Saturday)
            {
                return Reporting.ReportFrequency.Daily;
            }

            return Reporting.ReportFrequency.Weekly;
        }
    }
}
