using System;
using System.Collections.Generic;
using BuildingSecurity.Reporting.ReportingService;

namespace BuildingSecurity.Reporting
{
    /// <summary>
    /// Container object to hold all objects needed for a subscription.
    /// </summary>
    public class SubscriptionDetails
    {
        public string ScheduleId { get; set; }
        public string Description { get; set; }
        public IEnumerable<ParameterValue> Parameters { get; set; }
        public ScheduleDefinition ScheduleDefinition { get; set; }
        public DeliverySettings DeliverySettings { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public SubscriptionDetails(string scheduleId, string description, IEnumerable<ParameterValue> parameters, ScheduleDefinition scheduleDefinition, DeliverySettings deliverySettings, DateTime startDateTime, DateTime endDateTime)
        {
            ScheduleId = scheduleId;
            Description = description;
            Parameters = parameters;
            ScheduleDefinition = scheduleDefinition;
            DeliverySettings = deliverySettings;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }
    }
}
