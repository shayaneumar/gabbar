/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Web.Http;
using BuildingSecurity.Reporting;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers
{
    /// <summary>
    /// The <see cref="ReportParametersController"/> provides the ability for a
    /// client application to retrieve the list of report parameters based on new user selections
    /// in a negotiated format, particularly JSON or XML, over the standard http protocol.
    /// </summary>
    [RequiredPermission(PermissionNames.CanViewReports)]
    public class ReportParametersController : BaseApiController
    {
        private readonly IReportingClientFactory _reportingClientFactory;

        public ReportParametersController(IBuildingSecuritySessionStore sessionStore, IReportingClientFactory reportingClientFactory) : base(sessionStore)
        {
            _reportingClientFactory = reportingClientFactory;
        }

        /// <summary>
        /// Retrieve the updated list of report parameters based on a new user selection.
        /// </summary>
        /// <param name="reportParametersInput">A <see cref="ReportParametersInput"/> that contains all the parameters needed to retrieve the updated collection of parameters.</param>
        /// <returns>A collection of <see cref="BuildingSecurity.Reporting.ItemParameter"/>.</returns>
        public IEnumerable<ItemParameter> Post(ReportParametersInput reportParametersInput)
        {
            if (reportParametersInput == null || reportParametersInput.ReportId == null)
            {
                throw new HttpResponseException(HttpResponses.BadRequestMessage);
            }

            using (var reportingClient = _reportingClientFactory.CreateClient(BuildingSecurityUser))
            {
                return reportingClient.GetParameters(reportParametersInput.ReportId, reportParametersInput.DataSource, reportParametersInput.ParameterValues);
            }
        }
    }
}
