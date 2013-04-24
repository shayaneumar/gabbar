/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using BuildingSecurity.Reporting.ReportingService;

namespace BuildingSecurity.Reporting
{
    public interface IReportingClient : IDisposable
    {
        bool TestConnection();

        /// <summary>
        /// Returns a list of ReportInfo objects containing all accessible Reports.
        /// </summary>
        /// <returns>List of ReportInfo objects containing all accessible Reports.</returns>
        IEnumerable<ReportInfo> GetReports(string itemPath);

        /// <summary>
        /// Returns a list of Parameter objects for the specified report.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ItemParameter> GetParameters(string reportId, string dataSource, IEnumerable<ParameterValue> parameterValues);
        
        /// <summary>
        /// Returns a complete Report object based on the specified id.
        /// </summary>
        /// <param name="reportId">ID of the Report to be returned (this is the Path to the Report).</param>
        /// <param name="dataSource">Value of the selected Data Source</param>
        /// <param name="parameterValues">List of input ParameterValue objects containing the values to be passed to the report</param>
        /// <returns>Complete Report object based on the specified id.</returns>
        Report GetReport(string reportId, string dataSource, IEnumerable<ParameterValue> parameterValues);

        /// <summary>
        /// Executes the specified report.
        /// </summary>
        /// <param name="reportId">ID of the Report to be executed (this is the Path to the Report).</param>
        /// <param name="outputType">The <see cref="ReportOutputType"/> that the report is to be generated in.</param>
        /// <param name="parameterValues">List of input ParameterValue objects containing the values to be passed to the report</param>
        /// <returns>HTML version of the Report output</returns>
        byte[] GenerateReport(string reportId, IEnumerable<ParameterValue> parameterValues, ReportOutputType outputType);

        /// <summary>
        /// If multiValue is true and the specified ParameterValue contains a comma delimited list within the Value, this will return an enumeration
        /// of ParameterValue objects of the same name as the specified ParameterValue, and each value split from the CSV,
        /// else this will return a copy of the specified ParameterValue.
        /// A separate ParameterValue is required for each selection for MultiValue fields when a report is generated.
        /// </summary>
        /// <param name="parameterValue">Object that potentially contains a CSV list for a MultiValue field</param>
        /// <param name="multiValue">If true, this method will proceed to try to parse the value;
        /// pass false if the parameter is known to be a control type other than MultiValue.</param>
        /// <returns>List of ParameterValue objects, 1 for each item contained within the specified ParameterValue.Value CSV</returns>
        IEnumerable<ParameterValue> ExpandParameterValues(ParameterValue parameterValue, bool multiValue);

        /// <summary>
        /// This will return an enumeration of ParameterValue objects based on the specified input, with multiple items for any input item with CSV values
        /// A separate ParameterValue is required for each selection for MultiValue fields when a report is generated.
        /// </summary>
        /// <param name="parameterValues">List that potentially contains CSV lists for a MultiValue fields</param>
        /// <returns>List of ParameterValue objects, 1 or more for each item contained within the specified ParameterValues</returns>
        IEnumerable<ParameterValue> ExpandParameterValues(IEnumerable<ParameterValue> parameterValues);

        /// <summary>
        /// Create a subscription for a report.
        /// </summary>
        /// <param name="reportId">The report id.</param>
        /// <param name="description">The description of the subscription.</param>
        /// <param name="parameterValues">The parameter values for the specified report.</param>
        /// <param name="scheduleDefinition">The schedule definition.  Contains either a daily or weekly recurrence item.</param>
        /// <param name="deliverySettings">The delivery settings, either a FileShareDeliverySetting or an EmailDeliverySetting.</param>
        /// <returns>The id of the subscription that is created.</returns>
        string ScheduleReport(string reportId, string description, IEnumerable<ParameterValue> parameterValues,
                              ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings);

        /// <summary>
        /// Gets the subscription list for the specified reporting path
        /// for all application users from the SSRS server.
        /// </summary>
        /// <param name="itemPath">The path specifying the location of the Subscriptions.</param>
        /// <returns></returns>
        Subscription[] GetSubscriptionList(string itemPath);

        /// <summary>
        /// Gets the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns></returns>
        SubscriptionDetails GetSubscription(string subscriptionId);

        /// <summary>
        /// Deletes the subscription with the specified SubscriptionId.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns></returns>
        bool DeleteSubscription(string subscriptionId);

        /// <summary>
        /// Updates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="description">The description.</param>
        /// <param name="parameterValues">The parameter values for the specified report.</param>
        /// <param name="scheduleDefinition">The schedule definition.  Contains either a daily or weekly recurrence item.</param>
        /// <param name="deliverySettings">The delivery settings, either a FileShareDeliverySetting or an EmailDeliverySetting.</param>
        /// <returns></returns>
        bool UpdateSubscription(string subscriptionId, string description, IEnumerable<ParameterValue> parameterValues,
                                ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings);
    }
}
