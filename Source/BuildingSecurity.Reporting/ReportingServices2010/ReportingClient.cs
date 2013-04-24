/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Web.Services.Protocols;
using BuildingSecurity.Reporting.ReportExecution2005;
using BuildingSecurity.Reporting.ReportingService;
using JohnsonControls;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Exceptions;
using JohnsonControls.Serialization.Xml;
using TrustedUserHeader = BuildingSecurity.Reporting.ReportingService.TrustedUserHeader;
using JohnsonControls.Diagnostics;

namespace BuildingSecurity.Reporting.ReportingServices2010
{
    public class ReportingClient : IReportingClient
    {
        private readonly ReportingService2010SoapClient _reportService;
        private readonly ReportExecutionServiceSoapClient _reportExecution;
        private const string DefaultReportDataSource = "History";

        public ReportingClient(ReportServerConfiguration reportServerConfiguration, BasicHttpBinding binding)
        {
            NetworkCredential networkCredential = !string.IsNullOrWhiteSpace(reportServerConfiguration.UserName)
                                    ? new NetworkCredential(reportServerConfiguration.UserName, reportServerConfiguration.Password, reportServerConfiguration.Domain ?? "")
                                    : CredentialCache.DefaultNetworkCredentials;

            // Initialize Soap Client for ReportingServices2010
            var reportServiceRemoteAddress = CreateEndpointAddress(reportServerConfiguration.ServiceUrl + "/ReportService2010.asmx");
            _reportService = new ReportingService2010SoapClient(binding, reportServiceRemoteAddress);
            InitializeCredentials(_reportService, networkCredential);

            // Initialize Soap Client for ReportingExecutionService
            var reportExecutionRemoteAddress = CreateEndpointAddress(reportServerConfiguration.ServiceUrl + "/ReportExecution2005.asmx");
            _reportExecution = new ReportExecutionServiceSoapClient(binding, reportExecutionRemoteAddress);
            InitializeCredentials(_reportExecution, networkCredential);
        }

        private static EndpointAddress CreateEndpointAddress(string uri)
        {
            try
            {
                return new EndpointAddress(uri);
            }
            catch (UriFormatException exception)
            {
                throw new ReportingEndpointException(exception.Message, exception.InnerException);
            }
        }

        private static void InitializeCredentials<T>(ClientBase<T> clientBase, NetworkCredential networkCredential) where T : class
        {
            ClientCredentials clientCredentials = clientBase.ClientCredentials;
            if (clientCredentials == null) throw new InvalidOperationException("Unable to initialize credentials for SSRS.") ; 
            clientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
            clientCredentials.Windows.ClientCredential = networkCredential;
        }

        private bool ProxyTestConnection()
        {
            var listChildrenRequest = new ListChildrenRequest
            {
                TrustedUserHeader = new TrustedUserHeader(),
                ItemPath = "/",
                Recursive = false,
            };

            _reportService.ListChildren(listChildrenRequest);
            return true;
        }

        /// <summary>
        /// Gets the report list.
        /// </summary>
        /// <param name="itemPath">The item path.</param>
        /// <returns></returns>
        private IEnumerable<ReportInfo> ProxyGetReports(string itemPath)
        {
            var listChildrenRequest = new ListChildrenRequest
            {
                TrustedUserHeader = new TrustedUserHeader(),
                ItemPath = itemPath,
                Recursive = true,
            };

            var response = _reportService.ListChildren(listChildrenRequest);
            ICollection<ReportInfo> reportCollection = new Collection<ReportInfo>();
            foreach (var catalogItem in response.CatalogItems.Where(catalogItem =>
                !catalogItem.Hidden
                && catalogItem.Path.EndsWith("/Standard", StringComparison.InvariantCultureIgnoreCase)
                && "REPORT".Equals(catalogItem.TypeName, StringComparison.InvariantCultureIgnoreCase)))
            {
                reportCollection.Add(new ReportInfo(id: catalogItem.Path, name: catalogItem.Name, description: catalogItem.Description));
            }

            return reportCollection;
        }

        /// <summary>
        /// Returns a list of Parameter objects for the specified report.
        /// </summary>
        /// <param name="reportId">Path to the report. i.e. /JCIARS/DefaultReports/AuditTrail/History/Standard</param>
        /// <param name="dataSource">Value of the selected Data Source</param>
        /// <param name="parameterValues">Parameter values control the list of available values 
        /// for query based parameters that have dependencies</param>
        /// <returns></returns>
        private IEnumerable<ItemParameter> ProxyGetParameters(string reportId, string dataSource, IEnumerable<ParameterValue> parameterValues)
        {
            var itemPath = (dataSource == null) ? reportId : reportId.Replace("/" + DefaultReportDataSource + "/", "/" + dataSource + "/");

            var getItemParametersRequest = new GetItemParametersRequest
            {
                TrustedUserHeader = new TrustedUserHeader(),
                ItemPath = itemPath,
                Values = FilterInvalidParameterValues(ExpandParameterValues(parameterValues)).ToSsrsObject(),
                ForRendering = true
            };

            var response = _reportService.GetItemParameters(getItemParametersRequest);
            var itemParameters = response.Parameters.ToDomain();

            return GetLocalizedParameters(itemParameters);
        }

        private static IEnumerable<ItemParameter> GetLocalizedParameters(ICollection<ItemParameter> itemParameters)
        {
            Debug.Assert(itemParameters != null);

            ItemParameter reportLabels = itemParameters.FirstOrDefault(m => m.Name == "ReportLabels");
            if (reportLabels == null) return itemParameters;

            IDictionary<string, string> validValues = reportLabels.ValidValues;
            if (validValues == null) return itemParameters;

            return from item in itemParameters let validValue = validValues.FirstOrDefault(m => m.Key == (item.Name + "Label")) select ItemParameter.CreateItemParameter(item, validValue.Value);
        }
        
        private static string GetLocalizedParameterPrompt(IEnumerable<ReportParameter> itemParameters, string parameterName, string defaultPrompt)
        {
            Debug.Assert(itemParameters != null);

            ReportParameter reportLabels = itemParameters.FirstOrDefault(m => m.Name == "ReportLabels");
            if (reportLabels == null) return defaultPrompt;

            ReportExecution2005.ValidValue validValue = reportLabels.ValidValues.FirstOrDefault(x => x.Value == parameterName + "Label");

            return validValue != null ? validValue.Label : defaultPrompt;
        }

        private static string GetLocalizedParameterPrompt(IEnumerable<ItemParameter> itemParameters, string parameterName, string defaultPrompt)
        {
            Debug.Assert(itemParameters != null);

            ItemParameter reportLabels = itemParameters.FirstOrDefault(m => m.Name == "ReportLabels");
            if (reportLabels == null) return defaultPrompt;

            KeyValuePair<string, string> validValue = reportLabels.ValidValues.FirstOrDefault(x => x.Key == parameterName + "Label");

            return validValue.Value ?? defaultPrompt;
        }

        private Report ProxyGetReport(string reportId, string dataSource, IEnumerable<ParameterValue> parameterValues)
        {
            if (string.IsNullOrEmpty(reportId)) throw new ArgumentNullException("reportId");
           
            var listChildrenRequest = new ListChildrenRequest
            {
                TrustedUserHeader = new TrustedUserHeader(),
                ItemPath = reportId.Substring(0, reportId.LastIndexOf("/", StringComparison.InvariantCulture)),
                Recursive = true,
            };

            var response = _reportService.ListChildren(listChildrenRequest);
            var report = response.CatalogItems.FirstOrDefault(c => c.Path == reportId);
            if (report != null)
            {
                return new Report(report.Path, report.Name, report.Description, GetParameters(reportId, dataSource, parameterValues));
            }

            return null;
        }

        // TODO: Make call to ListRenderingExtensions (SSRS API) to find all available rendering extensions.
        // TODO: create out parameter with custom warning object or string array

        public byte[] GenerateReport(string reportId, IEnumerable<ParameterValue> parameterValues, ReportOutputType outputType)
        {
            return GenerateFileReport(reportId, parameterValues, outputType.Format());
        }

        public IEnumerable<ParameterValue> ExpandParameterValues(ParameterValue parameterValue, bool multiValue)
        {
            var expandedParameterValues = new List<ParameterValue>();

            try
            {
                if (multiValue)
                {
                    string[] values = parameterValue.Value.Split(',');

                    expandedParameterValues.AddRange(values.Select(value => new ParameterValue(parameterValue.Name, value)));
                }
                else
                {
                    expandedParameterValues.Add(new ParameterValue(parameterValue.Name, parameterValue.Value));
                }
            }
            catch (Exception)
            {
                return expandedParameterValues;
            }

            return expandedParameterValues;
        }
        
        private static IEnumerable<ReportExecution2005.ParameterValue> ExpandParameterValues(IEnumerable<ParameterValue> parameterValues, string name, bool multiValue)
        {
            var expandedParameterValues = new List<ReportExecution2005.ParameterValue>();

            try
            {
                ParameterValue parameterValue = parameterValues.First(p => p.Name == name);
                if (multiValue)
                {
                    string[] values = parameterValue.Value.Split(',');

                    expandedParameterValues.AddRange(values.Select(value => new ReportExecution2005.ParameterValue { Name = parameterValue.Name, Value = value }));
                }
                else
                {
                    expandedParameterValues.Add(new ReportExecution2005.ParameterValue
                                         {Name = parameterValue.Name, Value = parameterValue.Value});
                }
            }
            catch(Exception)
            {
                return expandedParameterValues;
            }

            return expandedParameterValues;
        }

        private static IEnumerable<ReportExecution2005.ParameterValue> ExpandParameterValues(IEnumerable<ParameterValue> parameterValues, IEnumerable<ReportParameter> reportParameters)
        {
            var expandedParameterValues = new List<ReportExecution2005.ParameterValue>();

            try
            {
                IEnumerable<ParameterValue> parameterValueList = parameterValues.ToList();
                foreach (ReportParameter reportParameter in reportParameters)
                {
                    expandedParameterValues.AddRange(ExpandParameterValues(parameterValueList, reportParameter.Name, reportParameter.MultiValue));
                }
            }
            catch (Exception)
            {
                return expandedParameterValues;
            }

            return expandedParameterValues;
        }

        public IEnumerable<ParameterValue> ExpandParameterValues(IEnumerable<ParameterValue> parameterValues)
        {
            var expandedParameterValues = new List<ParameterValue>();

            try
            {
                IEnumerable<ParameterValue> parameterValueList = parameterValues.ToList();
                foreach (ParameterValue parameterValue in parameterValueList)
                {
                    expandedParameterValues.AddRange(ExpandParameterValues(parameterValue, true));
                }
            }
            catch(Exception)
            {
                return expandedParameterValues;
            }

            return expandedParameterValues;
        }
        
        private static bool ValidateRequiredParameters(IEnumerable<ReportParameter> reportParameters, List<ParameterValue> parameterValues, out string parameterName)
        {
            List<ReportParameter> reportParameterList = reportParameters.ToList();
            foreach (ReportParameter reportParameter in reportParameterList)
            {
                if (!reportParameter.Nullable)
                {
                    ParameterValue parameterValue = parameterValues.FirstOrDefault(pv => pv.Name == reportParameter.Name);
                    if (parameterValue == null)
                    {
                        parameterName = GetLocalizedParameterPrompt(reportParameterList, reportParameter.Name, reportParameter.Prompt);
                        return false;
                    }
                }
            }

            parameterName = null;
            return true;
        }

        private static bool ValidateRequiredParameters(IEnumerable<ItemParameter> reportParameters, List<ParameterValue> parameterValues, out string parameterName)
        {
            List<ItemParameter> reportParameterList = reportParameters.ToList();
            foreach (ItemParameter reportParameter in reportParameterList)
            {
                if (!reportParameter.Nullable)
                {
                    ParameterValue parameterValue = parameterValues.FirstOrDefault(pv => pv.Name == reportParameter.Name);
                    if (parameterValue == null)
                    {
                        parameterName = GetLocalizedParameterPrompt(reportParameterList, reportParameter.Name, reportParameter.Prompt);
                        return false;
                    }
                }
            }

            parameterName = null;
            return true;
        }

        public IEnumerable<ParameterValue> FilterInvalidParameterValues(IEnumerable<ParameterValue> parameterValues)
        {
            var expandedParameterValues = new List<ParameterValue>();

            foreach (ParameterValue parameterValue in parameterValues)
            {
                if ((parameterValue.Name == "DateTimeFrom") || (parameterValue.Name == "DateTimeTo"))
                {
                    DateTime dateTime;
                    bool couldParseValue = DateTime.TryParse(parameterValue.Value, out dateTime);

                    // TODO: Consider validating IsReasonableDate(dateTime)
                    if (couldParseValue || string.IsNullOrEmpty(parameterValue.Value))
                    {
                        expandedParameterValues.AddRange(ExpandParameterValues(parameterValue, true));
                    }
                }
                else
                {
                    expandedParameterValues.AddRange(ExpandParameterValues(parameterValue, true));
                }
            }

            return expandedParameterValues;
        }

        /// <summary>
        /// Base method that exports a report of the specified exportFormat
        /// </summary>
        /// <param name="reportId">ID of the Report to be executed (this is the Path to the Report).</param>
        /// <param name="parameterValues">List of input ParameterValue objects containing the values to be passed to the report</param>
        /// <param name="exportFormat">The export format.</param>
        /// <returns></returns>
        private byte[] ProxyGenerateFileReport(string reportId, IEnumerable<ParameterValue> parameterValues, string exportFormat)
        {
            // TODO: Extract Methods for LoadReport, SetExecutionParameters, and Render
            // Init Report to execute
            string reportPath = reportId;
            ReportExecution2005.ServerInfoHeader serverInfoHeader;
            ExecutionInfo executionInfo;
            ExecutionHeader executionHeader = _reportExecution.LoadReport(null, reportPath, null, out serverInfoHeader, out executionInfo);
            string parameterPrompt;
            List<ParameterValue> parameterValueList = parameterValues.ToList();

            if (!ValidateRequiredParameters(executionInfo.Parameters, parameterValueList, out parameterPrompt))
            {
                throw new ReportingParameterValueException(ReportingParameterValueException.ErrorCodeEnum.MissingRequiredValue, parameterPrompt);
            }

            // Compose Parameter Values array
            var expandedParameterValues = ExpandParameterValues(parameterValueList, executionInfo.Parameters);
            var parameterValueArray = expandedParameterValues.ToArray();

            try
            {
                // Attach Report Parameters
                _reportExecution.SetExecutionParameters(executionHeader, null, parameterValueArray, null, out executionInfo);
            }
            catch (FaultException exception)
            {
                if(exception.Message.Contains("Microsoft.ReportingServices.Diagnostics.Utilities.ReportParameterTypeMismatchException: The value provided for the report parameter"))
                {
                    throw new ReportingParameterValueException(exception.Message, exception.InnerException);
                }

                throw new ReportingUnexpectedException(exception.Message, exception.InnerException);
            }

            try
            {
                // Render
                byte[] output;
                string extension;
                string mimeType;
                string encoding;
                ReportExecution2005.Warning[] warnings;
                string[] streamIds;
                _reportExecution.Render(executionHeader, null, exportFormat, null, out output, out extension, out mimeType, out encoding, out warnings, out streamIds);

                return output;
            }
            catch (FaultException exception)
            {
                if (exception.Message.StartsWith("Excel Rendering Extension"))
                {
                    throw new ReportingExcelRenderingException(exception.Message, exception.InnerException);
                }

                throw new ReportingUnexpectedException(exception.Message, exception.InnerException);
            }
            catch (CommunicationException exception)
            {
                if (exception.InnerException is QuotaExceededException)
                {
                    throw new ReportingMessageQuotaExceededException(exception.Message);
                }

                throw new ReportingUnexpectedException(exception.Message, exception.InnerException);
            }
        }

        /// <summary>
        /// Creates the subscription.
        /// </summary>
        /// <param name="itemPath">The SSRS path of the item to be saved on the SSRS server.</param>
        /// <param name="description">The description.</param>
        /// <param name="parameterValues">The parameter values for the specified report.</param>
        /// <param name="scheduleDefinition">The schedule definition.  Contains either a daily or weekly recurrence item.</param>
        /// <param name="deliverySettings">The delivery settings, either a FileShareDeliverySetting or an EmailDeliverySetting.</param>
        /// <returns></returns>
        private string ProxyCreateSubscription(string itemPath, string description, IEnumerable<ParameterValue> parameterValues, ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings)
        {
            string parameterPrompt;
            List<ParameterValue> parameterValueList = parameterValues.ToList();
            IEnumerable<ItemParameter> parameters = GetParameters(itemPath, DefaultReportDataSource, parameterValueList);

            if (!ValidateRequiredParameters(parameters, parameterValueList, out parameterPrompt))
            {
                throw new ReportingParameterValueException(ReportingParameterValueException.ErrorCodeEnum.MissingRequiredValue, parameterPrompt);
            }
            
            var createSubscriptionRequest = new CreateSubscriptionRequest
            {
                TrustedUserHeader = new TrustedUserHeader(),
                Description = description,
                ItemPath = itemPath,
                EventType = "TimedSubscription",
                ExtensionSettings = deliverySettings.GetExtensionSettings(),
                MatchData = GetSerializedSchedule(scheduleDefinition),
                Parameters = ExpandParameterValues(parameterValueList).ToSsrsObject(),
            };

            var response = _reportService.CreateSubscription(createSubscriptionRequest);
            return response.SubscriptionID;
        }

        /// <summary>
        /// Updates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="description">The description.</param>
        /// <param name="parameterValues">The parameter values for the specified report.</param>
        /// <param name="scheduleDefinition">The schedule definition.  Contains either a daily or weekly recurrence item.</param>
        /// <param name="deliverySettings">The delivery settings, either a FileShareDeliverySetting or an EmailDeliverySetting.</param>
        /// <returns></returns>
        private bool ProxySetSubscriptionProperties(string subscriptionId, string description, IEnumerable<ParameterValue> parameterValues, ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings)
        {
            var setSubscriptionPropertiesRequest = new SetSubscriptionPropertiesRequest
            {
                SubscriptionID = subscriptionId,
                TrustedUserHeader = new TrustedUserHeader(),
                Description = description,
                EventType = "TimedSubscription",
                ExtensionSettings = deliverySettings.GetExtensionSettings(),
                MatchData = GetSerializedSchedule(scheduleDefinition),
                Parameters = ExpandParameterValues(parameterValues).ToSsrsObject(),
            };

            var response = _reportService.SetSubscriptionProperties(setSubscriptionPropertiesRequest);
            if (response == null)
            {
                throw new ReportingNullResponseException("Response object returned by: SetSubscriptionProperties is null.");
            }

            return true;
        }

        /// <summary>
        /// Gets the subscription list for all application users from the SSRS server.  This method defaults the item path to the
        /// ReportServerCannedReportsPath setting found in the web.config.
        /// </summary>
        /// <returns></returns>
        private Subscription[] ProxyGetSubscriptionList()
        {
            return ProxyGetSubscriptionList(ConfigurationManager.AppSettings.Get("ReportServerCannedReportsPath"));
        }

        /// <summary>
        /// Gets the subscription list for the specified reporting path
        /// for all application users from the SSRS server.
        /// </summary>
        /// <param name="itemPath">The path specifying the location of the Subscriptions.</param>
        /// <returns></returns>
        private Subscription[] ProxyGetSubscriptionList(string itemPath)
        {
            try
            {
                var listSchedulesRequest = new ListSubscriptionsRequest
                                               {
                                                   ItemPathOrSiteURL = itemPath,
                                                   TrustedUserHeader = new TrustedUserHeader()
                                               };
                ListSubscriptionsResponse listSubscriptionsResponse = _reportService.ListSubscriptions(listSchedulesRequest);
                return listSubscriptionsResponse.SubscriptionItems;
            }
            catch (SoapException e)
            {
                Log.Verbose("A SoapException occurred while getting subscription list. Exception={0}, Trace={1}", e, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Deletes the subscription with the specified SubscriptionId.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns></returns>
        private bool ProxyDeleteSubscription(string subscriptionId)
        {
            var deleteSubscriptionRequest = new DeleteSubscriptionRequest
                                                {
                                                    SubscriptionID = subscriptionId,
                                                    TrustedUserHeader = new TrustedUserHeader()
                                                };
            var deleteSubscriptionResponse = _reportService.DeleteSubscription(deleteSubscriptionRequest);
            return deleteSubscriptionResponse != null;
        }

        private GetSubscriptionPropertiesResponse GetSubscriptionResponse(string subscriptionId)
        {
            var getSubscriptionPropertiesRequest = new GetSubscriptionPropertiesRequest
            {
                SubscriptionID = subscriptionId,
                TrustedUserHeader = new TrustedUserHeader()
            };
            var getSubscriptionPropertiesResponse = _reportService.GetSubscriptionProperties(getSubscriptionPropertiesRequest);
            return getSubscriptionPropertiesResponse;
        }

        private SubscriptionDetails ProxyGetSubscription(string subscriptionId)
        {
            var subscriptionResponse = GetSubscriptionResponse(subscriptionId);
            ScheduleDefinition schedule = GetDeserializedSchedule(subscriptionResponse.MatchData);

            var subscriptionObject = new SubscriptionDetails(subscriptionId,
                subscriptionResponse.Description,
                subscriptionResponse.Parameters.ToDomain(),
                schedule,
                DeliverySettings.CreateFromParameterValues(subscriptionResponse.ExtensionSettings.ParameterValues),
                schedule.StartDateTime,
                schedule.EndDate);
            return subscriptionObject;
        }

        /// <summary>
        /// Gets the serialized schedule as a string.
        /// </summary>
        /// <param name="scheduleDefinition">A schedule definition.</param>
        /// <returns></returns>
        private static string GetSerializedSchedule(ScheduleDefinition scheduleDefinition)
        {
            var scheduleSerializer = new XmlSerializer<ScheduleDefinition>();
            return scheduleSerializer.Serialize(scheduleDefinition);
        }

        private static ScheduleDefinition GetDeserializedSchedule(string scheduleDefinition)
        {
            var scheduleSerializer = new XmlSerializer<ScheduleDefinition>();
            return scheduleSerializer.Deserialize(scheduleDefinition);
        }

        public void Dispose()
        {
            _reportService.CloseOrAbort();
            _reportExecution.CloseOrAbort();
        }

        /// <summary>
        /// Calls the specified function within a try/catch to transform potential exceptions into BuildingSecurity.Reporting exceptions.
        /// Throws ReportingEndpointException if the Reporting Service URL is invalid or inaccessible.
        /// Throws ReportingAuthenticationException if the username or password is invalid or not permitted.
        /// And SoapExceptions that occur are written to the logging adapter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private static T Invoke<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (EndpointNotFoundException exception)
            {
                throw new ReportingEndpointException(exception.Message, exception.InnerException);
            }
            catch (MessageSecurityException exception)
            {
                // Invalid Credentials
                throw new ReportingAuthenticationException(exception.Message, exception.InnerException);
            }
            catch(SecurityNegotiationException)
            {
                throw new UntrustedCertificateException();
            }
            catch (ArgumentException exception)
            {
                // Invalid scheme (http vs. https)
                if ((exception.ParamName == "via") && (exception.Message.StartsWith("The provided URI scheme")))
                {
                    throw new ReportingSchemeException(exception.Message, exception.InnerException);
                }

                throw;
            }

            catch (ReportingNullResponseException exception)
            {
                throw new ReportingUnexpectedException(exception.Message, exception.InnerException);
            }
            catch (SoapException exception)
            {
                Log.Verbose("SoapException occurred while accessing SSRS. Exception={0}, Trace={1}", exception, exception.StackTrace);
                throw new ReportingUnexpectedException(exception.Message, exception.InnerException);
            }
            catch (FaultException exception)
            {
                Log.Verbose("FaultException occurred while accessing SSRS. Exception={0}, Trace={1}", exception, exception.StackTrace);

                if (exception.Message.Contains("The path is not valid.") || (exception.Message.Contains("cannot be found")))
                {
                    throw new ReportingLocationException(exception.Message, exception.InnerException);
                }

                throw new ReportingUnexpectedException(exception.Message, exception.InnerException);
            }
        }

        public bool TestConnection()
        {
            return Invoke(ProxyTestConnection);
        }

        /// <summary>
        /// Gets the report list.
        /// </summary>
        /// <param name="itemPath">The item path.</param>
        /// <returns></returns>
        public IEnumerable<ReportInfo> GetReports(string itemPath)
        {
            return Invoke(() => ProxyGetReports(itemPath));
        }

        /// <summary>
        /// Returns a list of Parameter objects for the specified report.
        /// </summary>
        /// <param name="reportId">Path to the report. i.e. /JCIARS/DefaultReports/AuditTrail/History/Standard</param>
        /// <param name="dataSource">Value of the selected Data Source</param>
        /// <param name="parameterValues">Parameter values control the list of available values 
        /// for query based parameters that have dependencies</param>
        /// <returns></returns>
        public IEnumerable<ItemParameter> GetParameters(string reportId, string dataSource, IEnumerable<ParameterValue> parameterValues = null)
        {
            return Invoke(() => ProxyGetParameters(reportId, dataSource, parameterValues));
        }

        public Report GetReport(string reportId, string dataSource, IEnumerable<ParameterValue> parameterValues)
        {
            return Invoke(() => ProxyGetReport(reportId, dataSource, parameterValues));
        }

        /// <summary>
        /// Base method that exports a report of the specified exportFormat
        /// </summary>
        /// <param name="reportId">ID of the Report to be executed (this is the Path to the Report).</param>
        /// <param name="parameterValues">List of input ParameterValue objects containing the values to be passed to the report</param>
        /// <param name="exportFormat">The export format.</param>
        /// <returns></returns>
        private byte[] GenerateFileReport(string reportId, IEnumerable<ParameterValue> parameterValues, string exportFormat)
        {
            return Invoke(() => ProxyGenerateFileReport(reportId, parameterValues, exportFormat));
        }

        public string ScheduleReport(string itemPath, string description, IEnumerable<ParameterValue> parameterValues, ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings)
        {
            return Invoke(() => ProxyCreateSubscription(itemPath, description, parameterValues, scheduleDefinition, deliverySettings));
        }

        /// <summary>
        /// Gets the subscription list for all application users from the SSRS server.
        /// </summary>
        /// <returns></returns>
        public Subscription[] GetSubscriptionList()
        {
            return Invoke(ProxyGetSubscriptionList);
        }

        /// <summary>
        /// Gets the subscription list for the specified reporting path
        /// for all application users from the SSRS server.
        /// </summary>
        /// <param name="itemPath">The path specifying the location of the Subscriptions.</param>
        /// <returns></returns>
        public Subscription[] GetSubscriptionList(string itemPath)
        {
            return Invoke(() => ProxyGetSubscriptionList(itemPath));
        }

        /// <summary>
        /// Gets the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns></returns>
        public SubscriptionDetails GetSubscription(string subscriptionId)
        {
            return Invoke(() => ProxyGetSubscription(subscriptionId));
        }

        /// <summary>
        /// Deletes the subscription with the specified SubscriptionId.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns></returns>
        public bool DeleteSubscription(string subscriptionId)
        {
            return Invoke(() => ProxyDeleteSubscription(subscriptionId));
        }

        /// <summary>
        /// Updates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="description">The description.</param>
        /// <param name="parameterValues">The parameter values for the specified report.</param>
        /// <param name="scheduleDefinition">The schedule definition.  Contains either a daily or weekly recurrence item.</param>
        /// <param name="deliverySettings">The delivery settings, either a FileShareDeliverySetting or an EmailDeliverySetting.</param>
        /// <returns></returns>
        public bool UpdateSubscription(string subscriptionId, string description, IEnumerable<ParameterValue> parameterValues, ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings)
        {
            return Invoke(() => ProxySetSubscriptionProperties(subscriptionId, description, parameterValues, scheduleDefinition, deliverySettings));
        }
    }
}
