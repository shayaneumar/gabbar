/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Linq;
using BuildingSecurity.Globalization;
using BuildingSecurity.Reporting.ReportingService;

namespace BuildingSecurity.Reporting
{
    public class FileShareDeliverySettings : DeliverySettings
    {
        readonly ExtensionSettings _extensionSettings = new ExtensionSettings();
        internal readonly static string ExtensionString = "Report Server FileShare";
        public override ReportDestination ReportDestination { get { return ReportDestination.FileShare; } }

        public override FileShareDeliverySettings ToFileShareSettings()
        {
            return this;
        }

        public FileShareDeliverySettings(ExtensionSettings extensionSettings)
        {
            _extensionSettings = extensionSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareDeliverySettings"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="includeFileExtension">if set to <c>true</c> [include file extension].</param>
        /// <param name="path">The URI of the file path.</param>
        /// <param name="renderFormat">The render format. <see cref="renderFormat"/></param>
        /// <param name="userName">Username for access to the network share.</param>
        /// <param name="password">Password for the network share associated with the specified Username.</param>
        /// <param name="writeMode">The write mode of the file. <see cref="WriteMode"/></param>
        public FileShareDeliverySettings(string fileName, bool includeFileExtension, string path
            , ReportOutputType renderFormat, string userName, string password, WriteMode writeMode)
        {
            // Parameters are passed into the web service using the ExtensionSettings object.
            _extensionSettings.ParameterValues = new ParameterValueOrFieldReference[7];
            _extensionSettings.ParameterValues[0] = new ReportingService.ParameterValue { Name = "FILENAME", Value = fileName };

            _extensionSettings.ParameterValues[1] = new ReportingService.ParameterValue
                                         {
                                             Name = "FILEEXTN",
                                             Value = includeFileExtension.ToString(CultureInfo.InvariantCulture)
                                         };

            _extensionSettings.ParameterValues[2] = new ReportingService.ParameterValue { Name = "PATH", Value = GetCleanUriString(path) };

            _extensionSettings.ParameterValues[3] = new ReportingService.ParameterValue { Name = "RENDER_FORMAT", Value = renderFormat.Format() };

            _extensionSettings.ParameterValues[4] = new ReportingService.ParameterValue { Name = "USERNAME", Value = userName };

            _extensionSettings.ParameterValues[5] = new ReportingService.ParameterValue { Name = "PASSWORD", Value = password };

            _extensionSettings.ParameterValues[6] = new ReportingService.ParameterValue
                                         {Name = "WRITEMODE", Value = Convert.ToString(writeMode)};

            // The name of the extension as it appears in the report server configuration file.
            // Valid values are Report Server Email, Report Server DocumentLibrary and Report Server FileShare.
            _extensionSettings.Extension = ExtensionString;
        }

        public override ExtensionSettings GetExtensionSettings()
        {
            return _extensionSettings;
        }

        public bool TryGetFileNameValue(out string fileNameValue)
        {
            fileNameValue = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "FILENAME"
                       select paramValue.Value).FirstOrDefault();
            return fileNameValue != null;
        }

        public bool TryGetFileExtensionValue(out bool fileExtensionValue)
        {
            string fileExtensionValueString = (from value in _extensionSettings.ParameterValues
                       let paramValue = (ReportingService.ParameterValue)value
                       where paramValue.Name == "FILEEXTN"
                       select paramValue.Value).FirstOrDefault();
            if (fileExtensionValueString != null)
            {
                fileExtensionValue = bool.Parse(fileExtensionValueString);
                return true;
            }
            // returning the preferred default value if not found.
            fileExtensionValue = true;
            return false;
        }

        public bool TryGetPathValue(out string pathValue)
        {
            pathValue = (from value in _extensionSettings.ParameterValues
                        let paramValue = (ReportingService.ParameterValue)value
                        where paramValue.Name == "PATH"
                        select paramValue.Value).FirstOrDefault();
            return pathValue != null;
        }

        public override bool TryGetRenderFormatValue(out ReportOutputType renderFormatValue)
        {
            var renderFormatValueString = (from value in _extensionSettings.ParameterValues
                            let paramValue = (ReportingService.ParameterValue)value
                            where paramValue.Name == "RENDER_FORMAT"
                            select paramValue.Value).FirstOrDefault();
            return Enum.TryParse(renderFormatValueString, true, out renderFormatValue);
        }

        public bool TryGetUserNameValue(out string userNameValue)
        {
            userNameValue = (from value in _extensionSettings.ParameterValues
                                 let paramValue = (ReportingService.ParameterValue)value
                             where paramValue.Name == "USERNAME"
                                 select paramValue.Value).FirstOrDefault();
            return userNameValue != null;
        }

        public bool TryGetPasswordValue(out string passwordValue)
        {
            passwordValue = (from value in _extensionSettings.ParameterValues
                                  let paramValue = (ReportingService.ParameterValue)value
                                  where paramValue.Name == "PASSWORD"
                                  select paramValue.Value).FirstOrDefault();
            return passwordValue != null;
        }

        public bool TryGetWriteModeValue(out WriteMode writeModeValue)
        {
            string writeModeValueString = (from value in _extensionSettings.ParameterValues
                            let paramValue = (ReportingService.ParameterValue)value
                            where paramValue.Name == "WRITEMODE"
                            select paramValue.Value).FirstOrDefault();
            if (writeModeValueString != null)
            {
                writeModeValue = (WriteMode)Enum.Parse(typeof(WriteMode), writeModeValueString, true);
                return true;
            }
            writeModeValue = WriteMode.AutoIncrement;
            return false;
        }

        public override string ToString()
        {
            string pathValue;

            TryGetPathValue(out pathValue);
            return string.Format(Resources.FileShareDeliverySettingsText, pathValue);
        }

        private static string GetCleanUriString(string path)
        {
            var uri = new Uri(path, UriKind.Absolute);
            return uri.LocalPath.TrimEnd('\\').Trim();//SSRS does not want the trailing \
        }
    }
}
