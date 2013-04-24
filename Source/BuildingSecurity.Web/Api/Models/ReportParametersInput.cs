/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using BuildingSecurity.Reporting;

namespace BuildingSecurity.Web.Api.Models
{
    /// <summary>
    /// All the parameters required to retrieve an updated list of report parameters.
    /// </summary>
    [DataContract]
    public class ReportParametersInput
    {
        // This class is used for serialization only. It is not intended for use other than serializing
        // JSON or XML passed to a web method. It is intended as input only to a web API call.
        public ReportParametersInput(string reportId, string dataSource, Collection<ParameterValue> parameterValues)
        {
            ReportId = reportId;
            DataSource = dataSource;
            ParameterValues = parameterValues;
        }

        /// <summary>
        /// The id of the report.
        /// </summary>
        [DataMember(Name = "reportId")]
        public string ReportId { get; private set; }

        [DataMember(Name = "dataSource")]
        public string DataSource { get; private set; }

        [DataMember(Name = "parameterValues")]
        public Collection<ParameterValue> ParameterValues { get; private set; }
    }
}