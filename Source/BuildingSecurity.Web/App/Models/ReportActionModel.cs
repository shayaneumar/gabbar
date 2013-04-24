/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using BuildingSecurity.Globalization;
using BuildingSecurity.Reporting;

namespace BuildingSecurity.Web.App.Models
{
    public class ReportActionModel
    {
        public string ReportId { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
            MessageId = "Ui", Justification = "Resharper and FxCop can not agree")]
        public string ReportUiCulture { get; set; }
        public string ReportName { get; set; }
        public ReportParameters Parameters { get; set; }
        public string ParametersView { get; set; }

        [Required(ErrorMessageResourceName = "ReportModelReportOutputTypeErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public ReportOutputType? ReportOutputType { get; set; }

        public string ReportOutputTypeLabel { get { return ReportOutputType.Label(); }}
        public string ReportOutputTypeIconPath { get { return ReportOutputType.IconPath(); }}

        public string DataSource { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is just a delivery vehicle to the view.")]
        public IList<string> DataSources { get; set; }
        public bool IsDataSourceSelectionRequired { get { return DataSources != null && DataSources.Count > 1; } }
        public IEnumerable<SelectListItem> DataSourceSelections
        {
            get
            {
                // TODO: Define Constant for "History" (History is currently used in many places)
                return DataSources.Select(ds => new SelectListItem { Text = (ds == "History") ? Resources.DefaultDataSourceName : ds, Value = ds, Selected = DataSource == ds });
            }
        }

        public string MessageText { get; set; }
        public string MessageType { get; set; }
    }
}
