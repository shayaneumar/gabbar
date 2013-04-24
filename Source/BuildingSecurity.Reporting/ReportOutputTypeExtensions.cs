/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace BuildingSecurity.Reporting
{
    public static class ReportOutputTypeExtensions
    {
        private const string DefaultLabel = "";
        private const string DefaultIcon = "~/Images/empty.png";
        private static readonly Dictionary<ReportOutputType, ReportOutputTypeDefinition> ReportOutputDefinitions = new Dictionary<ReportOutputType, ReportOutputTypeDefinition>
        {
            { ReportOutputType.Pdf, new ReportOutputTypeDefinition("application/pdf", "PdfLabel", "{0}.pdf", "PDF", "~/Images/pdf.png") },
            { ReportOutputType.Excel, new ReportOutputTypeDefinition("application/ms-excel", "ExcelLabel", "{0}.xls", "EXCEL", "~/Images/excel.png") },
            { ReportOutputType.Csv, new ReportOutputTypeDefinition("text/csv", "CsvLabel", "{0}.csv", "CSV", "~/Images/Csv.png") }
        };

        public static string ContentType(this ReportOutputType outputType)
        {
            if (!ReportOutputDefinitions.ContainsKey(outputType)) throw new ArgumentOutOfRangeException("outputType");

            return ReportOutputDefinitions[outputType].ContentType;
        }

        public static string FileNameFormat(this ReportOutputType outputType)
        {
            if (!ReportOutputDefinitions.ContainsKey(outputType)) throw new ArgumentOutOfRangeException("outputType");

            return ReportOutputDefinitions[outputType].FileNameFormat;
        }

        public static string Format(this ReportOutputType outputType)
        {
            if (!ReportOutputDefinitions.ContainsKey(outputType)) throw new ArgumentOutOfRangeException("outputType");

            return ReportOutputDefinitions[outputType].Format;
        }


        /// <summary>
        /// Report output type label requires an existing defined type format. If <see cref="outputType"/> is not set an empty string is used instead.
        /// </summary>
        public static string Label(this ReportOutputType? outputType)
        {
            if (!outputType.HasValue) return DefaultLabel;

            return outputType.Value.Label();
        }

        public static string IconPath(this ReportOutputType? outputType)
        {
            if (!outputType.HasValue) return DefaultIcon;

            return outputType.Value.IconPath();
        }

        public static string Label(this ReportOutputType outputType)
        {
            if (!ReportOutputDefinitions.ContainsKey(outputType)) throw new ArgumentOutOfRangeException("outputType");

            return ReportOutputDefinitions[outputType].Label;
        }

        public static string IconPath(this ReportOutputType outputType)
        {
            if (!ReportOutputDefinitions.ContainsKey(outputType)) throw new ArgumentOutOfRangeException("outputType");

            return ReportOutputDefinitions[outputType].IconPath;
        }
    }
}
