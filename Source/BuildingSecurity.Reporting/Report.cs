/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace BuildingSecurity.Reporting
{
    public class Report : ReportInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier for the report (path)</param>
        /// <param name="name">Localized Name of the Report</param>
        /// <param name="description">Localized Description for the report</param>
        /// <param name="parameters">List of Parameter objects</param>
        public Report(string id, string name, string description, IEnumerable<ItemParameter> parameters)
            : base(id, name, description)
        {
            _parameters = parameters;
        }

        public IEnumerable<ItemParameter> Parameters { get { return _parameters; } }

        private readonly IEnumerable<ItemParameter> _parameters;
    }
}
