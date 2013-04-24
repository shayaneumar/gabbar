/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq;
using BuildingSecurity.Globalization;
using BuildingSecurity.Reporting.ReportingService;

namespace BuildingSecurity.Reporting
{
    public class EmailDeliverySettings : DeliverySettings
    {
        readonly ExtensionSettings _extensionSettings = new ExtensionSettings();
        internal readonly static string ExtensionString = "Report Server Email";
        public override ReportDestination ReportDestination { get { return ReportDestination.Email; } }

        public override EmailDeliverySettings ToEmailSettings()
        {
            return this;
        }

        public EmailDeliverySettings(ExtensionSettings extensionSettings)
        {
            _extensionSettings = extensionSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareDeliverySettings"/> class.
        /// </summary>
        /// <param name="to">The e-mail address that appears on the To line of the e-mail message. Multiple e-mail addresses are separated by semicolons. Required.</param>
        /// <param name="cc">The e-mail address that appears on the Cc line of the e-mail message. Multiple e-mail addresses are separated by semicolons. Optional.</param>
        /// <param name="bcc">The e-mail address that appears on the Bcc line of the e-mail message. Multiple e-mail addresses are separated by semicolons. Optional.</param>
        /// <param name="replyTo">The e-mail address that appears in the Reply-To header of the e-mail message. The value must be a single e-mail address. Optional.</param>
        /// <param name="renderFormat">The name of the rendering extension to use to generate the rendered report. The name must correspond to one of the visible rendering extensions installed on the report server. This value is required if the IncludeReport setting is set to a value of true.<see cref="renderFormat"/></param>
        /// <param name="includeReport">A value that indicates whether to include the report in the e-mail delivery. A value of true indicates that the report is delivered in the body of the e-mail message.</param>
        /// <param name="priority">The priority with which the e-mail message is sent. Valid values are LOW, NORMAL, and HIGH. The default value is NORMAL.</param>
        /// <param name="subject">The text in the subject line of the e-mail message.</param>
        /// <param name="comment">The text included in the body of the e-mail message.</param>
        /// <param name="includeLink">A value that indicates whether to include a link to the report in the body of the e-mail.</param>
        public EmailDeliverySettings(string to, string cc, string bcc, string replyTo
            , ReportOutputType renderFormat, IncludeReport includeReport, EmailPriority priority, string subject, string comment, IncludeLink includeLink)
        {
            // Parameters are passed into the web service using the ExtensionSettings object.
            _extensionSettings.ParameterValues = new ParameterValueOrFieldReference[10];
            _extensionSettings.ParameterValues[0] = new ReportingService.ParameterValue { Name = "TO", Value = to };

            _extensionSettings.ParameterValues[1] = new ReportingService.ParameterValue { Name = "CC", Value = cc };

            _extensionSettings.ParameterValues[2] = new ReportingService.ParameterValue { Name = "BCC", Value = bcc };

            _extensionSettings.ParameterValues[3] = new ReportingService.ParameterValue { Name = "ReplyTo", Value = replyTo };

            _extensionSettings.ParameterValues[4] = new ReportingService.ParameterValue
                                         {Name = "RenderFormat", Value = renderFormat.Format()};

            _extensionSettings.ParameterValues[5] = new ReportingService.ParameterValue
                                                        {Name = "IncludeReport", Value = includeReport.SsrsText()};

            _extensionSettings.ParameterValues[6] = new ReportingService.ParameterValue { Name = "Priority", Value = priority.SsrsText() };

            _extensionSettings.ParameterValues[7] = new ReportingService.ParameterValue { Name = "Subject", Value = subject };

            _extensionSettings.ParameterValues[8] = new ReportingService.ParameterValue { Name = "Comment", Value = comment };

            _extensionSettings.ParameterValues[9] = new ReportingService.ParameterValue { Name = "IncludeLink", Value = includeLink.SsrsText() };

            // The name of the extension as it appears in the report server configuration file. 
            // Valid values are Report Server Email, Report Server DocumentLibrary and Report Server FileShare.
            _extensionSettings.Extension = ExtensionString;
        }

        public override ExtensionSettings GetExtensionSettings()
        {
            return _extensionSettings;
        }

        public bool TryGetToValue(out string toValue)
        {
            toValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "TO"
                       select paramValue.Value).FirstOrDefault();
            return toValue != null;

        }

        public bool TryGetCcValue(out string ccValue)
        {
            ccValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "CC"
                       select paramValue.Value).FirstOrDefault();
            return ccValue != null;

        }

        public bool TryGetBccValue(out string bccValue)
        {
            bccValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "BCC"
                       select paramValue.Value).FirstOrDefault();
            return bccValue != null;

        }

        public bool TryGetReplyToValue(out string replyToValue)
        {
            replyToValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "ReplyTo"
                       select paramValue.Value).FirstOrDefault();
            return replyToValue != null;

        }

        public override bool TryGetRenderFormatValue(out ReportOutputType renderFormatValue)
        {
            string renderFormatValueString = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "RenderFormat"
                       select paramValue.Value).FirstOrDefault();
            return Enum.TryParse(renderFormatValueString, true, out renderFormatValue);
        }

        public bool TryGetIncludeReportValue(out IncludeReport includeReportValue)
        {
            string includeReportValueString = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "IncludeReport"
                       select paramValue.Value).FirstOrDefault();
            if (includeReportValueString != null)
            {
                includeReportValue = (IncludeReport)Enum.Parse(typeof(IncludeReport), includeReportValueString, true);
                return true;
            }
            includeReportValue = IncludeReport.Yes;
            return false;

        }

        public bool TryGetPriorityValue(out EmailPriority priortyValue)
        {
            string priortyValueString = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "Priority"
                       select paramValue.Value).FirstOrDefault();
            if (priortyValueString != null)
            {
                priortyValue = (EmailPriority)Enum.Parse(typeof(EmailPriority), priortyValueString, true);
                return true;
            }
            priortyValue = EmailPriority.Normal;
            return false;

        }

        public bool TryGetSubjectValue(out string subjectValue)
        {
            subjectValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "Subject"
                       select paramValue.Value).FirstOrDefault();
            return subjectValue != null;

        }

        public bool TryGetCommentValue(out string commentValue)
        {
            commentValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "Comment"
                       select paramValue.Value).FirstOrDefault();
            return commentValue != null;

        }

        public bool TryGetIncludeLinkValue(out IncludeLink includeLinkValue)
        {
            string includeLinkValueString = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "IncludeLink"
                       select paramValue.Value).FirstOrDefault();
            if (includeLinkValueString != null)
            {
                includeLinkValue = (IncludeLink)Enum.Parse(typeof(IncludeLink), includeLinkValueString, true);
                return true;
            }
            includeLinkValue = IncludeLink.Yes;
            return false;

        }

        public override string ToString()
        {
            string toValue;

            TryGetToValue(out toValue);
            return string.Format(Resources.EmailDeliverySettingsText, toValue);
        }
    }
}
