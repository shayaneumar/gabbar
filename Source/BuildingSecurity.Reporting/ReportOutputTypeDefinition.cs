/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Resources;
using BuildingSecurity.Globalization;

namespace BuildingSecurity.Reporting
{
    public class ReportOutputTypeDefinition
    {
        public ReportOutputTypeDefinition(string contentType, string labelKey, string fileNameFormat, string format, string iconPath)
        {
            ContentType = contentType;
            _labelKey = labelKey;
            FileNameFormat = fileNameFormat;
            Format = format;
            IconPath = iconPath;
        }

        public string ContentType { get; private set; }
        private readonly string _labelKey;
        public string Label { get { return new ResourceManager(typeof(Resources)).GetString(_labelKey); } }
        public string FileNameFormat { get; private set; }
        public string Format { get; private set; }
        public string IconPath { get; private set; }
    }
}
