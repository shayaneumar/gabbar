/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Reporting
{
    public class ReportInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier for the report (path)</param>
        /// <param name="name">Localized Name of the Report</param>
        /// <param name="description">Localized Description for the report</param>
        public ReportInfo(string id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }

        public string Id { get { return _id; } }
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }

        private readonly string _id;
        private readonly string _name;
        private readonly string _description;
    }
}
