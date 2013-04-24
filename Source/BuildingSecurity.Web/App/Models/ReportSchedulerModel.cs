/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BuildingSecurity.Reporting;
using BuildingSecurity.Globalization;
using System;

namespace BuildingSecurity.Web.App.Models
{
    public class ReportSchedulerModel : ReportActionModel, IValidatableObject
    {
        public string ScheduleId { get; set; }

        [Display(Name = "ReportSchedulerModelDescriptionLabel", ResourceType = typeof(Resources))]
        public string Description { get; set; }

        [Required(ErrorMessageResourceName = "ReportModelReportRecurranceNotSpecifiedErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public ReportFrequency? ReportFrequency { get; set; }
        public ReportDestination ReportDestination { get; set; }
        public bool EndDateSpecified { get; set; }
        public string DeliverySettingsText { get; set; }
        public string DateRangeText { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "JavaScript needs true/false to be lowercase.")]
        public string EndDateSpecifiedValue { get { return EndDateSpecified.ToString(CultureInfo.InvariantCulture).ToLowerInvariant(); } }

        [DataType(DataType.Time)]
        [Display(Name = "ReportSchedulerModeRunTimeLabel", ResourceType = typeof(Resources))]
        public string ScheduledTime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ReportSchedulerModelStartDateLabel", ResourceType = typeof(Resources))]
        public string StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ReportSchedulerModelEndDateLabel", ResourceType = typeof(Resources))]
        public string EndDate { get; set; }

        [Display(Name = "ReportSchedulerModelWeekDayLabel", ResourceType = typeof(Resources))]
        public WeeklyRecurrencePattern Weekday { get; set; }

        [Display(Name = "ReportSchedulerModelLocationLabel", ResourceType = typeof(Resources))]
        public string Location { get; set; }

        [Display(Name = "ReportSchedulerModelUserNameLabel", ResourceType = typeof(Resources))]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ReportSchedulerModelPasswordLabel", ResourceType = typeof(Resources))]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "ReportSchedulerModelEmailAddressesLabel", ResourceType = typeof(Resources))]
        public string EmailAddresses { get; set; }

        [Display(Name = "ReportSchedulerModelSubjectLabel", ResourceType = typeof(Resources))]
        public string Subject { get; set; }

        [Display(Name = "ReportSchedulerModelCommentLabel", ResourceType = typeof(Resources))]
        public string Comment { get; set; }

        public static IEnumerable<SelectListItem> Weekdays
        {
            get
            {
                return BuildingSecurity.Reporting.Weekdays.GetNames(CultureInfo.CurrentCulture).Select(weekday => new SelectListItem { Text = weekday.Value, Value = weekday.Key.ToString() }).ToList();
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate Description is specified
            if (string.IsNullOrWhiteSpace(Description))
                yield return new ValidationResult(Resources.ReportSchedulerModelDescriptionRequiredErrorMessage);

            // Validate Scheduled Time
            if (string.IsNullOrWhiteSpace(ScheduledTime))
            {
                yield return new ValidationResult(Resources.ReportSchedulerModelScheduledTimeRequiredMessage);
            }

            DateTime scheduledTime;
            if (!DateTime.TryParse(DateTime.MinValue.ToString(
                CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + " " + ScheduledTime,
                CultureInfo.CurrentCulture, DateTimeStyles.None, out scheduledTime))
                yield return new ValidationResult(Resources.ReportSchedulerModelInvalidScheduledTimeErrorMessage);

            // Validate Start Date
            DateTime startDate;
            bool validStartDate = DateTime.TryParse(StartDate, out startDate);
            if (!validStartDate)
                yield return new ValidationResult(Resources.ReportSchedulerModelInvalidStartDateErrorMessage);

            if (EndDateSpecified)
            {
                // Validate End Date
                DateTime endDate;
                bool validEndDate = DateTime.TryParse(EndDate, out endDate);
                if (!validEndDate)
                    yield return new ValidationResult(Resources.ReportSchedulerModelInvalidEndDateErrorMessage);

                // Validate End Date >= Start Date
                if ((validStartDate) && (validEndDate) && (endDate < startDate))
                    yield return new ValidationResult(Resources.ReportSchedulerModelEndDatePrecedesStartDateErrorMessage);
            }

            switch (ReportDestination)
            {
                case ReportDestination.FileShare:
                    // Validate Location specified
                    if (string.IsNullOrWhiteSpace(Location))
                        yield return new ValidationResult(Resources.ReportSchedulerModelLocationRequiredErrorMessage);

                    // Validate Location is a valid UNC path
                    if ((Location != null) && (!IsUncPath(Location))) 
                        yield return new ValidationResult(Resources.ReportSchedulerModelLocationInvalidErrorMessage);

                    // Validate User Name specified
                    if (string.IsNullOrWhiteSpace(UserName))
                        yield return new ValidationResult(Resources.ReportSchedulerModelUserNameRequiredErrorMessage);

                    // Validate Password specified
                    if (string.IsNullOrEmpty(Password))
                        yield return new ValidationResult(Resources.ReportSchedulerModelPasswordRequiredErrorMessage);
                    break;

                case ReportDestination.Email:
                    // Validate Email Addresses specified
                    if (string.IsNullOrWhiteSpace(EmailAddresses))
                        yield return new ValidationResult(Resources.ReportSchedulerModelEmailAddressesRequiredErrorMessage);

                    // Validate Subject specified
                    if (string.IsNullOrWhiteSpace(Subject))
                        yield return new ValidationResult(Resources.ReportSchedulerModelSubjectRequiredErrorMessage);

                    break;
            }
        }

        /// <summary>
        /// Determines whether the path is a valid UNC path.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns>
        ///   <c>true</c> if UNC path is valid; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsUncPath(string path)
        {
            Uri uri;
            return Uri.TryCreate(path, UriKind.Absolute, out uri) && uri.IsUnc && uri.Segments.Count() > 1;
        }
    }
}
