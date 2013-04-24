/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers
{
    [RequiredPermission(PermissionNames.CanViewCaseManager)]
    public class CaseNotesController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public CaseNotesController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient)
            : base(sessionStore)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        public CaseNote Post(CaseNoteModel caseNoteModel)
        {
            if (caseNoteModel == null)
            {
                throw new ArgumentNullException("caseNoteModel");
            }

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(x => x.Errors.Select(error => error.ErrorMessage));
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(", ", errorList)));
            }

            return _buildingSecurityClient.CreateCaseNote(BuildingSecurityUser.BuildingSecurityCookie, caseNoteModel.CaseId, caseNoteModel.Text);
        }
    }
}