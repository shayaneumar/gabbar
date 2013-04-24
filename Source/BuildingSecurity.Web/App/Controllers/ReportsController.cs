/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using BuildingSecurity.Globalization;
using BuildingSecurity.Reporting;
using BuildingSecurity.Reporting.ReportingService;
using BuildingSecurity.Web.App.Models;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Diagnostics;
using ItemParameter = BuildingSecurity.Reporting.ItemParameter;
using ParameterValue = BuildingSecurity.Reporting.ParameterValue;

namespace BuildingSecurity.Web.App.Controllers
{
    public enum ReportListAction
    {
        Run,
        Schedule
    }

    public class PersistedParameterInfo
    {
        public PersistedParameterInfo()
        {
        }

        public PersistedParameterInfo(string name, ParameterType parameterType, bool nullable, string defaultValue)
        {
            Name = name;
            ParameterType = parameterType;
            Nullable = nullable;
            DefaultValue = defaultValue;
        }

        public string Name { get; set; }
        public ParameterType ParameterType { get; set; }
        public bool Nullable { get; set; }
        public string DefaultValue { get; set; }
    }

    internal static class NameValueCollectionExtensions
    {
        public static IEnumerable<ParameterValue> ToReportParameters(this NameValueCollection formValues)
        {
            var reportParameters = Json.Decode<PersistedParameterInfo[]>(formValues["Report.Parameters"]);
            return formValues.AllKeys.Join(reportParameters, key => key, parameter => parameter.Name,
                (key, parameter) => new ParameterValue(key, Value(formValues[key], parameter.ParameterType, parameter.Nullable, parameter.DefaultValue)));
        }

        private static string Value(string submittedValue, ParameterType parameterType, bool nullable, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(submittedValue))
            {
                return nullable ? null : defaultValue;
            }

            if (parameterType == ParameterType.Boolean)
            {
                return submittedValue.ToUpperInvariant().Contains("TRUE").ToString(CultureInfo.InvariantCulture);
            }

            return submittedValue;
        }
    }

    [RequiredPermission(PermissionNames.CanViewReports)]
    public class ReportsController : Controller
    {
        private const int CustomReportIdPartIndex = 2;
        private const int DataSourcePartIndex = 4;
        private const char ReportIdPartSeperatorChar = '/';
        private const string ReportIdPartSeperatorString = "/";
        private const string CustomReportId = "CustomReports";
        private const string DefaultReportDataSouce = "History";
        private const String LanguageParameterName = "LanguageSelectSingle";
        private const String DateTimeRangeParameterName = "DateTimeRangeTypeSelectSingle";
        private const string YesterdayDateTimeRangeValue = "326F59D4-AB14-42CD-8ADC-AAD4F15CC70A";
        private const string LastSevenDaysDateTimeRangeValue = "429371A2-1EC8-49F2-BCDE-1A51D031E03D";
        private const string DynamicParameters = "DynamicParameters";

        private readonly string _customReportsPath;
        private readonly string _cannedReportsPath;
        private readonly IReportingClientFactory _reportingClientFactory;

        public ReportsController(IReportingClientFactory reportingClientFactory)
        {
            _reportingClientFactory = reportingClientFactory;

            //TODO: Inject reporting paths with ninject
            _cannedReportsPath = ConfigurationManager.AppSettings["ReportServerCannedReportsPath"];
            _customReportsPath = ConfigurationManager.AppSettings["ReportServerCustomReportsPath"];
        }

        #region Standard / Custom Report Lists

        [RequiredPermission(PermissionNames.CanRunOrScheduleReports)]
        public ActionResult Index(IUser user, ReportListAction listAction = ReportListAction.Run)
        {
            return CallAndTransformErrors(() => ProxyIndex(user, listAction), user);
        }

        private ActionResult ProxyIndex(IUser user, ReportListAction listAction)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                var model = new ReportsModel(CannedReports, GetCustomReports(reportingClient), listAction);
                return View("Index", model);
            }
        }

        #endregion

        #region Reports Runner

        [RequiredPermission(PermissionNames.CanRunReports)]
        public ActionResult Runner(IUser user, string reportId, string dataSource)
        {
            return CallAndTransformErrors(() => ProxyRunner(reportId, dataSource, user), user);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [RequiredPermission(PermissionNames.CanRunReports)]
        public ActionResult Runner(IUser user, ReportActionModel model)
        {
            return CallAndTransformErrors(() => ProxyRunner(model, user), user);
        }

        private ActionResult ProxyRunner(string reportId, string dataSource, IUser user)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                if (dataSource == null) dataSource = DefaultReportDataSouce;

                String reportUiCulture;
                Report report = RetrieveReportForReportId(reportingClient, reportId, dataSource, out reportUiCulture);

                if (report == null)
                {
                    return GetMissingReportView(reportId);
                }

                // Show the user the view to run the report
                return View("Runner", new ReportActionModel
                {
                    ReportId = report.Id,
                    ReportUiCulture = reportUiCulture,
                    ReportName = report.Name,
                    Parameters = new ReportParameters(report.Parameters, false),
                    ParametersView = DynamicParameters,
                    DataSource = dataSource,
                    DataSources = DataSourcesForReport(reportingClient, report),
                    ReportOutputType = ReportOutputType.Pdf
                });
            }
        }

        private static void ValidateDates(List<ParameterValue> parameterValues)
        {
            ParameterValue fromField = parameterValues.FirstOrDefault(pv => pv.Name == "DateTimeFrom");
            ParameterValue toField = parameterValues.FirstOrDefault(pv => pv.Name == "DateTimeTo");

            DateTime toDateTime = DateTime.MinValue;
            DateTime fromDateTime = DateTime.MinValue;
            bool couldParseFrom = fromField != null && DateTime.TryParse(fromField.Value, out fromDateTime);
            bool couldParseTo = toField != null && DateTime.TryParse(toField.Value, out toDateTime);

            //If the field is present it must be parsable or empty.
            if (!couldParseFrom && fromField != null && !string.IsNullOrWhiteSpace(fromField.Value))
                throw new ReportingDateException(ReportingDateException.ErrorCodeEnum.FromDateFormat);
            if (!couldParseTo && toField != null && !string.IsNullOrWhiteSpace(toField.Value))
                throw new ReportingDateException(ReportingDateException.ErrorCodeEnum.ToDateFormat);

            if (couldParseFrom && !IsReasonableDate(fromDateTime))
                throw new ReportingDateException(ReportingDateException.ErrorCodeEnum.FromDateRange);
            if (couldParseTo && !IsReasonableDate(toDateTime))
                throw new ReportingDateException(ReportingDateException.ErrorCodeEnum.ToDateRange);

            if (couldParseFrom && couldParseTo && toDateTime < fromDateTime)
                throw new ReportingDateException(ReportingDateException.ErrorCodeEnum.Sequence);
        }

        private static bool IsReasonableDate(DateTime date)
        {
            return date < new DateTime(2100, 1, 1) && date > new DateTime(1900, 1, 1);
        }

        private ActionResult ProxyRunner(ReportActionModel model, IUser user)
        {
            if (model != null && ModelState.IsValid)
            {
                using (var reportingClient = _reportingClientFactory.CreateClient(user))
                {
                    string errorMessage;
                    List<ParameterValue> parameterValues = null;
                    bool useParameterValues;

                    try
                    {
                        //This debug assert is needed prevent resharper from flagging model.ReportOutputType.Value as a possible exception
                        //this assert is guaranteed by the model as ReportOutputType is marked required
                        Debug.Assert(model.ReportOutputType != null, "ReportOutputType should never be null here.");
                        ReportOutputType reportOutputType = model.ReportOutputType.Value;

                        parameterValues = Request.Form.ToReportParameters().ToList();

                        ValidateDates(parameterValues);

                        var byteArray = reportingClient.GenerateReport(RequestedReport(model),
                                                                       parameterValues,
                                                                       reportOutputType);
                        return new FileContentResult(byteArray, reportOutputType.ContentType())
                        {
                            FileDownloadName = string.Format(CultureInfo.CurrentCulture,
                                                             reportOutputType.FileNameFormat(),
                                                             model.ReportName)
                        };
                    }
                    catch (TimeoutException)
                    {
                        useParameterValues = false;
                        errorMessage = Resources.ReportTimeOutMessage;
                    }
                    catch (ReportingMessageQuotaExceededException)
                    {
                        useParameterValues = true;
                        errorMessage = Resources.ReportMessageQuotaExceededExceptionMessage;
                    }
                    catch (ReportingParameterValueException exception)
                    {
                        switch (exception.ErrorCode)
                        {
                            case ReportingParameterValueException.ErrorCodeEnum.MissingRequiredValue:
                                useParameterValues = true;
                                errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.ReportParameterRequiredError, exception.ParameterPrompt);
                                break;
                            default:
                                useParameterValues = false;
                                errorMessage = Resources.ReportParameterInvalidError;
                                break;
                        }
                    }
                    catch (HttpRequestValidationException)
                    {
                        useParameterValues = false;
                        errorMessage = Resources.SchedulerReportUnacceptableValueMessage;
                    }
                    catch (ReportingDateException exception)
                    {
                        useParameterValues = true;
                        switch (exception.ErrorCode)
                        {
                            case ReportingDateException.ErrorCodeEnum.FromDateFormat: errorMessage = Resources.ReportDateInvalidFromFormatMessage; break;
                            case ReportingDateException.ErrorCodeEnum.ToDateFormat: errorMessage = Resources.ReportDateInvalidToFormatMessage; break;
                            case ReportingDateException.ErrorCodeEnum.FromDateRange: errorMessage = Resources.ReportDateInvalidFromRangeMessage; break;
                            case ReportingDateException.ErrorCodeEnum.ToDateRange: errorMessage = Resources.ReportDateInvalidToRangeMessage; break;
                            case ReportingDateException.ErrorCodeEnum.Sequence: errorMessage = Resources.ReportDateInvalidSequenceMessage; break;
                            default: errorMessage = Resources.ReportDateRangeInvalidMessage; break;
                        }
                    }
                    catch (ReportingExcelRenderingException)
                    {
                        useParameterValues = true;
                        errorMessage = Resources.ReportExceedingExcelWorksheeSizeMessage;
                    }

                    model.Parameters = new ReportParameters(reportingClient.GetParameters(model.ReportId, model.DataSource, ((useParameterValues) ? parameterValues : null)), false, parameterValues);

                    String reportUiCulture;
                    Report report = RetrieveReportForReportId(reportingClient, model.ReportId, model.DataSource, out reportUiCulture);
                    if (report == null)
                    {
                        return GetMissingReportView(model.ReportId);
                    }

                    model.DataSources = DataSourcesForReport(reportingClient, report);
                    model.ParametersView = DynamicParameters;
                    SetMessage(model, MessageType.Error, errorMessage);

                    return View("Runner", model);
                }
            }

            return View("Runner", model);
        }

        private ViewResult GetMissingReportView(string reportId)
        {
            // Show the user that we encountered an error
            return View("MissingReport", new MissingReportModel
            {
                ReportId = reportId,
                Error = Resources.ReportsViewInvalidReportErrorMessage,
                Title = Resources.ReportRunnerViewPageTitle
            });
        }
        #endregion

        #region Reports Scheduler
        [RequiredPermission(PermissionNames.CanScheduleReports)]
        public ActionResult Scheduler(IUser user, string reportId, string messageType, string messageText)
        {
            return CallAndTransformErrors(() => ProxyScheduler(reportId, user, messageType, messageText), user);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [RequiredPermission(PermissionNames.CanScheduleReports)]
        public ActionResult Scheduler(IUser user, ReportSchedulerModel model)
        {
            return CallAndTransformErrors(() => ProxyScheduler(model, user), user);
        }

        private ActionResult ProxyScheduler(string reportId, IUser user, string messageType, string messageText)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                string reportUiCulture;
                Report report = RetrieveReportForReportId(reportingClient, reportId, DefaultReportDataSouce, out reportUiCulture);
                if (report == null)
                {
                    // Show the user that we encountered an error
                    return View("MissingReport", new MissingReportModel
                    {
                        ReportId = reportId,
                        Error = Resources.ReportsViewInvalidReportErrorMessage,
                        Title = Resources.ReportSchedulerViewPageTitle
                    });
                }

                // Show the user the view to schedule the report
                DateTime now = DateTime.Now;
                var reportSchedulerModel = new ReportSchedulerModel
                {
                    ReportId = report.Id,
                    ReportUiCulture = reportUiCulture,
                    ReportName = report.Name,
                    Parameters = new ReportParameters(report.Parameters, true),
                    ParametersView = DynamicParameters,
                    ScheduledTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0).ToShortTimeString(),
                    StartDate = now.ToShortDateString(),
                    ReportOutputType = ReportOutputType.Excel,
                    ReportFrequency = ReportFrequency.Daily
                };

                SetMessage(reportSchedulerModel, messageType, messageText);
                return View("Scheduler", reportSchedulerModel);
            }
        }

        private ActionResult ProxyScheduler(ReportSchedulerModel model, IUser user)
        {
            if (model != null)
            {
                using (var reportingClient = _reportingClientFactory.CreateClient(user))
                {
                    string errorMessage;
                    List<ParameterValue> parameterValues = null;

                    try
                    {
                        parameterValues = Request.Form.ToReportParameters().ToList();

                        if (ModelState.IsValid)
                        {
                            Debug.Assert(model.ReportFrequency != null, "model.ReportFrequency != null");
                            string scheduleId = reportingClient.ScheduleReport(
                                model.ReportId, model.Description,
                                parameterValues.Concat(DateTimeRangeParameterArrayForReportFrequency(model.ReportFrequency.Value)),
                                ScheduleDefinitionFromReportSchedulerModel(model),
                                DeliverySettingsFromReportSchedulerModel(model));

                            return RedirectToAction("ScheduledReport", new { scheduleId, reportId = model.ReportId });
                        }

                        errorMessage = GetModelStateErrorMessages();
                    }
                    catch (ReportingLocationException)
                    {
                        errorMessage = Resources.SchedulerReportLocationInvalidErrorMessage;
                    }
                    catch (ReportingUnexpectedException ex)
                    {
                        Log.Error("{0}\n{1}", ex.Message, ex.StackTrace);
                        errorMessage = Resources.SchedulerReportUnexpectedErrorMessage;
                    }
                    catch (ReportingParameterValueException exception)
                    {
                        switch (exception.ErrorCode)
                        {
                            case ReportingParameterValueException.ErrorCodeEnum.MissingRequiredValue:
                                errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.ReportParameterRequiredError, exception.ParameterPrompt);
                                break;
                            default:
                                errorMessage = Resources.ReportParameterInvalidError;
                                break;
                        }
                    }
                    catch (HttpRequestValidationException)
                    {
                        errorMessage = Resources.SchedulerReportUnacceptableValueMessage;
                    }
                   
                    try
                    {
                        model.Parameters = new ReportParameters(reportingClient.GetParameters(model.ReportId, DefaultReportDataSouce, parameterValues), true);
                    }
                    catch (HttpRequestValidationException)
                    {
                        model.Parameters = new ReportParameters(reportingClient.GetParameters(model.ReportId, DefaultReportDataSouce, null), true, parameterValues);
                        errorMessage = Resources.SchedulerReportUnacceptableValueMessage;
                    }

                    model.ParametersView = DynamicParameters;
                    SetMessage(model, MessageType.Error, errorMessage);

                    return View("Scheduler", model);
                }
            }

            return View("Index");
        }

        #endregion

        #region Scheduled Report
        [RequiredPermission(PermissionNames.CanViewScheduledReports)]
        public ActionResult ScheduledReport(IUser user, string scheduleId, string reportId)
        {
            return CallAndTransformErrors(() => ProxyScheduledReport(scheduleId, reportId, user), user);
        }

        private ActionResult ProxyScheduledReport(string scheduleId, string reportId, IUser user)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                SubscriptionDetails subscriptionDetails = reportingClient.GetSubscription(scheduleId);
                if (subscriptionDetails == null)
                {
                    // TODO: Implement MissingSchedule (or MissingItem and specify the type as a property in the Model)
                    // Show the user that we encountered an error
                    return View("MissingReport", new MissingReportModel
                    {
                        ReportId = scheduleId,
                        Error = Resources.ReportsViewInvalidReportErrorMessage,
                        Title = Resources.ReportScheduledViewPageTitle
                    });
                }

                string reportUiCulture;
                Report report = RetrieveReportForReportId(reportingClient, reportId, DefaultReportDataSouce, out reportUiCulture);
                if (report == null)
                {
                    // Show the user that we encountered an error
                    return View("MissingReport", new MissingReportModel
                    {
                        ReportId = reportId,
                        Error = Resources.ReportsViewInvalidReportErrorMessage,
                        Title = Resources.ReportScheduledViewPageTitle
                    });
                }

                var model = CreateReportSchedulerModel(reportingClient, reportUiCulture, report, subscriptionDetails);

                model.DeliverySettingsText = subscriptionDetails.DeliverySettings.ToString();
                model.DateRangeText = GetDateRangeText(subscriptionDetails);

                // Show the user the view to run the report
                return View("ScheduledReport", model);
            }
        }

        #endregion

        #region Scheduled Reports List
        [RequiredPermission(PermissionNames.CanViewScheduledReports)]
        public ActionResult Scheduled(IUser user)
        {
            return CallAndTransformErrors(() => ProxyScheduled(user), user);
        }

        private ActionResult ProxyScheduled(IUser user)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                var subscriptions = reportingClient.GetSubscriptionList("/JCIARS");
                List<Subscription> updatedSubscriptions = (from subscription in subscriptions
                                                           let reportInfo = CannedReports.FirstOrDefault(r => r.Id == subscription.Path)
                                                           select new Subscription
                                                                      {
                                                                          SubscriptionID = subscription.SubscriptionID,
                                                                          Path = subscription.Path,
                                                                          Description = subscription.Description,
                                                                          Report = reportInfo == null ? GetCustomReportName(subscription.Path) : reportInfo.Name,
                                                                          Status = subscription.Status
                                                                      }).ToList();

                var model = new ReportsScheduledModel(updatedSubscriptions);

                return View("Scheduled", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiredPermission(PermissionNames.CanDeleteScheduledReports)]
        public ActionResult DeleteScheduled(IUser user, string scheduleId)
        {
            return CallAndTransformErrors(() => ProxyDeleteScheduled(scheduleId, user), user);
        }

        private ActionResult ProxyDeleteScheduled(string scheduleId, IUser user)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                reportingClient.DeleteSubscription(scheduleId);

                return RedirectToAction("Scheduled");
            }
        }

        #endregion

        #region Modify Scheduled Report

        [RequiredPermission(PermissionNames.CanEditScheduledReports)]
        public ActionResult ModifyScheduled(IUser user, string scheduleId, string reportId, string messageType, string messageText)
        {
            return CallAndTransformErrors(() => ProxyModifyScheduled(scheduleId, reportId, user, messageType, messageText), user);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [RequiredPermission(PermissionNames.CanEditScheduledReports)]
        public ActionResult ModifyScheduled(IUser user, ReportSchedulerModel model)
        {
            return CallAndTransformErrors(() => ProxyModifyScheduled(model, user), user);
        }

        private ActionResult ProxyModifyScheduled(string scheduleId, string reportId, IUser user, string messageType, string messageText)
        {
            using (var reportingClient = _reportingClientFactory.CreateClient(user))
            {
                SubscriptionDetails subscriptionDetails = reportingClient.GetSubscription(scheduleId);
                if (subscriptionDetails == null)
                {
                    // Show the user that we encountered an error
                    return View("MissingReport", new MissingReportModel
                    {
                        ReportId = scheduleId,
                        Error = Resources.ReportsViewInvalidReportErrorMessage,
                        Title = Resources.ReportSchedulerViewPageTitle
                    });
                }

                string reportUiCulture;
                Report report = RetrieveReportForReportId(reportingClient, reportId, DefaultReportDataSouce, out reportUiCulture);
                if (report == null)
                {
                    // Show the user that we encountered an error
                    return View("MissingReport", new MissingReportModel
                    {
                        ReportId = subscriptionDetails.ScheduleId,
                        Error = Resources.ReportsViewInvalidReportErrorMessage,
                        Title = Resources.ReportSchedulerViewPageTitle
                    });
                }

                var model = CreateReportSchedulerModel(reportingClient, reportUiCulture, report, subscriptionDetails);
                SetMessage(model, messageType, messageText);

                // Show the user the view to run the report
                return View("ModifyScheduled", model);
            }
        }

        private ActionResult ProxyModifyScheduled(ReportSchedulerModel model, IUser user)
        {
            if (model != null)
            {
                using (var reportingClient = _reportingClientFactory.CreateClient(user))
                {
                    string errorMessage;
                    List<ParameterValue> parameterValues = null;

                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Debug.Assert(model.ReportFrequency != null, "model.ReportFrequency != null");
                            string scheduleId;
                            //TODO:Fix this controller, controllers should not be accessing the http context like this
                            parameterValues = Request.Form.ToReportParameters().ToList();
                            if (!string.IsNullOrWhiteSpace(Request.Form["NewSchedule"]))
                            {
                                //This is very poorly designed. Updates and creations should not be handled by the same action
                                //as a result of this design we now have to check permissions inside of an action. Permissions
                                //should always be handled by filters not actions.
                                if(!user.HasPermission(PermissionNames.CanScheduleReports)) return new HttpStatusCodeResult(403);
                                scheduleId = reportingClient.ScheduleReport(
                                    model.ReportId, model.Description,
                                    parameterValues.Concat(
                                        DateTimeRangeParameterArrayForReportFrequency(model.ReportFrequency.Value)),
                                    ScheduleDefinitionFromReportSchedulerModel(model),
                                    DeliverySettingsFromReportSchedulerModel(model));
                            }
                            else
                            {
                                scheduleId = model.ScheduleId;

                                reportingClient.UpdateSubscription(
                                    model.ScheduleId, model.Description,
                                    parameterValues.Concat(
                                        DateTimeRangeParameterArrayForReportFrequency(model.ReportFrequency.Value)),
                                    ScheduleDefinitionFromReportSchedulerModel(model),
                                    DeliverySettingsFromReportSchedulerModel(model));
                            }

                            return RedirectToAction("ScheduledReport", new {scheduleId, reportId = model.ReportId});
                        }

                        errorMessage = GetModelStateErrorMessages();
                    }
                    catch (ReportingLocationException)
                    {
                        errorMessage = Resources.SchedulerReportLocationInvalidErrorMessage;
                    }
                    catch (ReportingUnexpectedException ex)
                    {
                        Log.Error("{0}\n{1}", ex.Message, ex.StackTrace);
                        errorMessage = Resources.SchedulerReportUnexpectedErrorMessage;
                    }
                    catch (HttpRequestValidationException)
                    {
                        errorMessage = Resources.SchedulerReportUnacceptableValueMessage;
                    }
                    
                    try
                    {
                        model.Parameters = new ReportParameters(reportingClient.GetParameters(model.ReportId, DefaultReportDataSouce, parameterValues), true);
                    }
                    catch (HttpRequestValidationException)
                    {
                        model.Parameters = new ReportParameters(reportingClient.GetParameters(model.ReportId, DefaultReportDataSouce, null), true, parameterValues);
                        errorMessage = Resources.SchedulerReportUnacceptableValueMessage;
                    }

                    model.ParametersView = DynamicParameters;
                    SetMessage(model, MessageType.Error, errorMessage);

                    return View("ModifyScheduled", model);
                }
            }

            return View("Scheduled");
        }

        #endregion

        #region Model To Report Server Types

        private static DeliverySettings DeliverySettingsFromReportSchedulerModel(ReportSchedulerModel model)
        {
            DeliverySettings deliverySettings;

            Debug.Assert(model.ReportOutputType != null, "model.ReportOutputType Should not be null");
            ReportOutputType reportOutputType = model.ReportOutputType.Value;

            if (model.ReportDestination == ReportDestination.Email)
            {
                deliverySettings = new EmailDeliverySettings(FormatEmailAddresses(model.EmailAddresses), null, null, null,
                                                             reportOutputType, IncludeReport.Yes,
                                                             EmailPriority.Normal, model.Subject,
                                                             model.Comment, IncludeLink.No);
            }
            else
            {
                deliverySettings = new FileShareDeliverySettings(model.ReportName, true,
                                                                 model.Location, reportOutputType,
                                                                 model.UserName, model.Password,
                                                                 WriteMode.AutoIncrement);
            }

            return deliverySettings;
        }

        private static ScheduleDefinition ScheduleDefinitionFromReportSchedulerModel(ReportSchedulerModel model)
        {
            var startDate = DateTime.Parse(model.StartDate, CultureInfo.CurrentCulture);
            var runtTime = DateTime.Parse(model.ScheduledTime, CultureInfo.CurrentCulture);
            var startDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                                             runtTime.Hour, runtTime.Minute, runtTime.Second, DateTimeKind.Local);

            var recurrencePattern = new WeeklyRecurrence
            {
                WeeksIntervalSpecified = true,
                WeeksInterval = 1,
                DaysOfWeek =
                    model.ReportFrequency == ReportFrequency.Daily
                        ? DailyDaysOfWeek
                        : WeeklyDayOfWeek(model.Weekday)
            };

            var scheduleDefinition = new ScheduleDefinition
            {
                StartDateTime = startDateTime,
                EndDateSpecified = model.EndDateSpecified,
                Item = recurrencePattern
            };

            if (model.EndDateSpecified)
            {
                scheduleDefinition.EndDate = DateTime.Parse(model.EndDate, CultureInfo.CurrentCulture);
            }

            return scheduleDefinition;
        }

        private static readonly DaysOfWeekSelector DailyDaysOfWeek = new DaysOfWeekSelector
        {
            Sunday = true,
            Monday = true,
            Tuesday = true,
            Wednesday = true,
            Thursday = true,
            Friday = true,
            Saturday = true
        };

        private static DaysOfWeekSelector WeeklyDayOfWeek(WeeklyRecurrencePattern weekDay)
        {
            return new DaysOfWeekSelector
            {
                Sunday = (weekDay == WeeklyRecurrencePattern.Sunday),
                Monday = (weekDay == WeeklyRecurrencePattern.Monday),
                Tuesday = (weekDay == WeeklyRecurrencePattern.Tuesday),
                Wednesday = (weekDay == WeeklyRecurrencePattern.Wednesday),
                Thursday = (weekDay == WeeklyRecurrencePattern.Thursday),
                Friday = (weekDay == WeeklyRecurrencePattern.Friday),
                Saturday = (weekDay == WeeklyRecurrencePattern.Saturday),
            };
        }

        #endregion

        #region Report Server To Model

        private static ReportSchedulerModel CreateReportSchedulerModel(IReportingClient reportingClient, string reportUiCulture,
                                                                       Report report, SubscriptionDetails subscriptionDetails)
        {
            var subscriptionParameters = reportingClient.GetParameters(report.Id, DefaultReportDataSouce, subscriptionDetails.Parameters);
            bool endDateSpecified = subscriptionDetails.EndDateTime != DateTime.MinValue;
            var model = new ReportSchedulerModel
            {
                ReportId = report.Id,
                ScheduleId = subscriptionDetails.ScheduleId,
                ReportUiCulture = reportUiCulture,
                ReportName = report.Name,
                Description = subscriptionDetails.Description,
                Parameters = new ReportParameters(subscriptionParameters, true),
                ParametersView = DynamicParameters,
                ReportOutputType =
                    ReportOutputTypeFromDeliverySettings(
                        subscriptionDetails.DeliverySettings),
                ReportFrequency = ReportFrequencyFromScheduleDefinition(subscriptionDetails.ScheduleDefinition),
                ScheduledTime = subscriptionDetails.StartDateTime.ToShortTimeString(),
                StartDate = subscriptionDetails.StartDateTime.ToShortDateString(),
                EndDateSpecified = endDateSpecified,
                ReportDestination =
                    ReportDestinationFromDeliverySettings(
                        subscriptionDetails.DeliverySettings)
            };

            if (model.ReportFrequency == ReportFrequency.Weekly)
            {
                model.Weekday = WeeklyRecurrencePatternFromScheduleDefintion(subscriptionDetails.ScheduleDefinition);
            }

            if (endDateSpecified) model.EndDate = subscriptionDetails.EndDateTime.ToShortDateString();

            if (subscriptionDetails.DeliverySettings.ReportDestination == ReportDestination.FileShare)
            {
                LoadModelWithFileShareValues(model, subscriptionDetails.DeliverySettings.ToFileShareSettings());
            }
            else
            {
                LoadModelWithEmailValues(model, subscriptionDetails.DeliverySettings.ToEmailSettings());
            }

            return model;
        }

        private static void LoadModelWithFileShareValues(ReportSchedulerModel model, FileShareDeliverySettings ds)
        {
            string location;
            string userName;
            ds.TryGetPathValue(out location);
            ds.TryGetUserNameValue(out userName);
            model.Location = location;
            model.UserName = userName;
        }

        private static void LoadModelWithEmailValues(ReportSchedulerModel model, EmailDeliverySettings ds)
        {
            string addresses;
            string subject;
            string comment;
            ds.TryGetToValue(out addresses);
            ds.TryGetSubjectValue(out subject);
            ds.TryGetCommentValue(out comment);
            model.EmailAddresses = addresses;
            model.Subject = subject;
            model.Comment = comment;
        }

        private static WeeklyRecurrencePattern WeeklyRecurrencePatternFromScheduleDefintion(ScheduleDefinition scheduleDefinition)
        {
            var weeklyRecurrance = (WeeklyRecurrence)scheduleDefinition.Item;
            return weeklyRecurrance.WeeklyRecurrencePattern();
        }

        private static ReportOutputType? ReportOutputTypeFromDeliverySettings(DeliverySettings deliverySettings)
        {
            ReportOutputType reportOutputType;
            return
                deliverySettings.TryGetRenderFormatValue(out reportOutputType)
                    ? reportOutputType
                    : (ReportOutputType?)null;
        }

        private static ReportDestination ReportDestinationFromDeliverySettings(DeliverySettings deliverySettings)
        {
            return deliverySettings.ReportDestination;
        }

        private static ReportFrequency? ReportFrequencyFromScheduleDefinition(ScheduleDefinition scheduleDefinition)
        {
            var weeklyRecurrance = scheduleDefinition.Item as WeeklyRecurrence;
            //The scheduleDefinition.Item may not be a WeeklyRecurrence, but we don't support that.
            //In that event we just default to null.
            if (weeklyRecurrance == null) return null;
            return weeklyRecurrance.ReportFrequency();
        }

        #endregion

        private static string GetDateRangeText(SubscriptionDetails subscriptionDetails)
        {
            var recurrencePattern = (WeeklyRecurrence)subscriptionDetails.ScheduleDefinition.Item;

            string endDate = subscriptionDetails.EndDateTime != DateTime.MinValue
                                 ? " " + Resources.ReportFrequencyEnding + " " +
                                   subscriptionDetails.EndDateTime.ToShortDateString()
                                 : "";

            if (recurrencePattern.ReportFrequency() == ReportFrequency.Daily)
            {
                return string.Format(CultureInfo.InvariantCulture,
                                     Resources.ReportFrequencyDailyText,
                                     subscriptionDetails.StartDateTime.ToShortTimeString(),
                                     " " + Resources.ReportFrequencyStarting + " " +
                                     subscriptionDetails.StartDateTime.ToShortDateString(),
                                     endDate);
            }

            string dayOfWeek = recurrencePattern.DayOfWeek();

            return string.Format(CultureInfo.InvariantCulture,
                                 Resources.ReportFrequencyWeeklyText,
                                 dayOfWeek,
                                 subscriptionDetails.StartDateTime.ToShortTimeString(),
                                 " " + Resources.ReportFrequencyStarting + " " +
                                 subscriptionDetails.StartDateTime.ToShortDateString(),
                                 endDate);
        }

        private ActionResult CallAndTransformErrors(Func<ActionResult> func, IUser user)
        {
            UserExtensions.PopulateViewBag(user, ViewBag);
            ReportsErrorModel model;

            try
            {
                return func();
            }
            catch (ReportingEndpointException)
            {
                // Invalid Report Server Uri Format or Endpoint
                model = new ReportsErrorModel(Resources.ReportingEndpointException, user.HasPermission(PermissionNames.CanEditReportsServerSettings));
            }
            catch (ReportingAuthenticationException)
            {
                // Invalid Credentials
                model = new ReportsErrorModel(Resources.ReportingAuthenticationException, user.HasPermission(PermissionNames.CanEditReportsServerSettings));
            }

            return View("Error", model);
        }

        private static string RequestedReport(ReportActionModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.DataSource))
            {
                string[] reportIdParts = SplitReportId(model.ReportId);
                reportIdParts[DataSourcePartIndex] = model.DataSource;

                return string.Join(ReportIdPartSeperatorString, reportIdParts);
            }

            return model.ReportId;
        }

        private static bool IsCustomReport(string reportId)
        {
            string[] reportIdParts = SplitReportId(reportId);
            return (reportIdParts.Length >= CustomReportIdPartIndex &&
                    reportIdParts[CustomReportIdPartIndex] == CustomReportId);
        }

        private IList<string> DataSourcesForReport(IReportingClient reportingClient, Report report)
        {
            var path = IsCustomReport(report.Id) ? _customReportsPath : _cannedReportsPath;
            var reports = reportingClient.GetReports(path);

            String baseReportName = string.Join("/", SplitReportId(report.Id), 0, DataSourcePartIndex);

            return (from reportInfo in reports
                    where reportInfo.Id.StartsWith(baseReportName, StringComparison.OrdinalIgnoreCase)
                    select SplitReportId(reportInfo.Id)[DataSourcePartIndex]).ToList();
        }

        private static string[] SplitReportId(string reportId)
        {
            return reportId.Split(ReportIdPartSeperatorChar);
        }

        private static IEnumerable<ReportInfo> CannedReports
        {
            get
            {
                return new[]
                           {
                               new ReportInfo("/JCIARS/DefaultReports/AlarmActivity/History/Standard",
                                              Resources.ReportNameAlarmActivity,
                                              Resources.ReportDescriptionAlarmActivity),

                               new ReportInfo("/JCIARS/DefaultReports/AlarmHistory/History/Standard",
                                              Resources.ReportNameAlarmHistory,
                                              Resources.ReportDescriptionAlarmHistory),

                               new ReportInfo("/JCIARS/DefaultReports/AlarmSummary/History/Standard",
                                              Resources.ReportNameAlarmSummary,
                                              Resources.ReportDescriptionAlarmSummary),

                               new ReportInfo("/JCIARS/DefaultReports/AreaTransaction/History/Standard",
                                              Resources.ReportNameAreaTransactionHistory,
                                              Resources.ReportDescriptionAreaTransactionHistory),

                               new ReportInfo("/JCIARS/DefaultReports/AuditTrail/History/Standard",
                                              Resources.ReportNameAuditTrail,
                                              Resources.ReportDescriptionAuditTrail),

                               new ReportInfo("/JCIARS/DefaultReports/CardholderTransaction/History/Standard",
                                              Resources.ReportNameCardholderTransaction,
                                              Resources.ReportDescriptionCardholderTransaction),

                               new ReportInfo("/JCIARS/DefaultReports/IntrusionTransaction/History/Standard",
                                              Resources.ReportNameIntrusionTransactionHistory,
                                              Resources.ReportDescriptionIntrusionTransactionHistory),

                               new ReportInfo("/JCIARS/DefaultReports/MusterAnalysis/History/Standard",
                                              Resources.ReportNameMusterAnalysis,
                                              Resources.ReportDescriptionMusterAnalysis),

                               new ReportInfo("/JCIARS/DefaultReports/GuardTourTransaction/History/Standard",
                                              Resources.ReportNameGuardTourTransactionHistory,
                                              Resources.ReportDescriptionGuardTourTransactionHistory),

                               new ReportInfo("/JCIARS/DefaultReports/Transaction/History/Standard",
                                              Resources.ReportNameTransactionHistory,
                                              Resources.ReportDescriptionTransactionHistory)
                           };
            }
        }

        private string GetCustomReportName(string reportId)
        {
            return reportId.Replace(_customReportsPath + "/", "").Replace("/History/Standard", "");
        }

        private IEnumerable<ReportInfo> GetCustomReports(IReportingClient reportingClient)
        {
            ICollection<ReportInfo> filteredCustomReports = new Collection<ReportInfo>();
            IEnumerable<ReportInfo> customReports = reportingClient.GetReports(_customReportsPath);

            foreach (ReportInfo reportInfo in customReports)
            {
                if (reportInfo.Id.Contains("/History/Standard"))
                {
                    filteredCustomReports.Add(new ReportInfo(reportInfo.Id, GetCustomReportName(reportInfo.Id), reportInfo.Description));
                }
            }

            return filteredCustomReports;
        }

        private Report RetrieveReportForReportId(IReportingClient reportingClient, string reportId, string dataSource, out string reportUiCulture)
        {
            reportUiCulture = CultureInfo.CurrentCulture.Name;
            Report report = reportingClient.GetReport(reportId, dataSource, LanguageParameterArrayForCulture(reportUiCulture));

            if (report != null)
            {
                ReportInfo reportInfo;
                string reportName;
                string reportDescription;
                List<ItemParameter> parameters = report.Parameters.ToList();

                if (TryGetReportInfo(reportId, out reportInfo))
                {
                    // Attempt to identify a CannedReport with the specified reportId,
                    // to use the Name and Description from Resources
                    reportName = reportInfo.Name;
                    reportDescription = reportInfo.Description;
                }
                else
                {
                    // If the reportId does not match a CannedReport, then use the
                    // Name and Description from the report itself (not localized)
                    reportName = GetCustomReportName(report.Id);
                    reportDescription = report.Description;
                }

                if (!IsRequestedLanguageSupported(parameters))
                {
                    // fall back to English (United States)
                    reportUiCulture = "en-US";
                    parameters = reportingClient.GetParameters(report.Id, dataSource,
                                                               LanguageParameterArrayForCulture(reportUiCulture)).ToList();
                }

                report = new Report(report.Id, reportName, reportDescription, parameters);
            }

            return report;
        }

        private static bool TryGetReportInfo(string reportId, out ReportInfo reportInfo)
        {
            reportInfo = CannedReports.FirstOrDefault(cr => cr.Id == reportId);
            return (reportInfo != null);
        }

        private static bool IsRequestedLanguageSupported(IEnumerable<ItemParameter> reportParameters)
        {
            if (reportParameters != null)
            {
                ItemParameter languageSelect = reportParameters.FirstOrDefault(m => m.Name == LanguageParameterName);
                return (languageSelect != null && languageSelect.ParameterStateName == "HasValidValue");
            }

            return false;
        }

        private static IEnumerable<ParameterValue> DateTimeRangeParameterArrayForReportFrequency(ReportFrequency reportFrequency)
        {
            return new[] { DateTimeRangeParameterForReportFrequency(reportFrequency) };
        }

        private static ParameterValue DateTimeRangeParameterForReportFrequency(ReportFrequency reportFrequency)
        {
            return new ParameterValue(DateTimeRangeParameterName, reportFrequency == ReportFrequency.Daily ? YesterdayDateTimeRangeValue : LastSevenDaysDateTimeRangeValue);
        }

        private static IEnumerable<ParameterValue> LanguageParameterArrayForCulture(string cultureName)
        {
            return new[] { LanguageParameterValueForCulture(cultureName) };
        }

        private static ParameterValue LanguageParameterValueForCulture(string cultureName)
        {
            return new ParameterValue(LanguageParameterName, cultureName);
        }

        private static string FormatEmailAddresses(string emailAddresses)
        {
            if (emailAddresses != null)
            {
                emailAddresses = emailAddresses.Replace(",", ";").Replace(" ", ";");

                while (emailAddresses.Contains(";;"))
                {
                    emailAddresses = emailAddresses.Replace(";;", ";");
                }
            }

            return emailAddresses;
        }

        private string GetModelStateErrorMessages()
        {
            return ModelState[""].Errors.Aggregate("", (current, modelError) => current + (modelError.ErrorMessage + " "));
        }

        private static void SetMessage(ReportSchedulerModel model, string messageType, string messageText)
        {
            model.MessageType = messageType;
            model.MessageText = messageText;
        }

        private static void SetMessage(ReportActionModel model, string messageType, string messageText)
        {
            model.MessageType = messageType;
            model.MessageText = messageText;
        }
    }
}
