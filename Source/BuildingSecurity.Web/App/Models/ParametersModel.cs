/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using BuildingSecurity.Reporting;

namespace BuildingSecurity.Web.App.Models
{
    public class ParametersModel
    {
        public string ReportId { get; private set; }
        public IEnumerable<ItemParameter> Parameters { get; private set; }

        public ParametersModel()
        {}

        public ParametersModel(string reportId, IEnumerable<ItemParameter> parameters)
        {
            ReportId = reportId;
            Parameters = parameters;
        }
    }
}
