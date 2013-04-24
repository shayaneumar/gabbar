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
using BuildingSecurity.Globalization;
using BuildingSecurity.Web.Api.Models;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Collections;

namespace BuildingSecurity.Web.Api.Controllers
{
    [RequiredPermission(PermissionNames.CanViewCaseManager)]
    public class CasesController : BaseApiController
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;

        public CasesController(IBuildingSecuritySessionStore sessionStore, IBuildingSecurityClient buildingSecurityClient)
            : base(sessionStore)
        {
            _buildingSecurityClient = buildingSecurityClient;
        }

        public DataChunk<Case> Get()
        {
            return _buildingSecurityClient.RetrieveCases(BuildingSecurityUser.BuildingSecurityCookie);
        }

        public Case Get(string id)
        {
            if (id == null) throw new HttpResponseException(HttpResponses.BadRequestMessage);

            var caseDetails = _buildingSecurityClient.RetrieveCase(BuildingSecurityUser.BuildingSecurityCookie, id);

            if (caseDetails == null) throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.ErrorResourceNotFound));

            return caseDetails;
        }

        public string Post(CaseCreateRequest caseModel)
        {
            if (caseModel == null)
            {
                throw new ArgumentNullException("caseModel");
            }

            ThrowBadRequestIfRequestInvalid();
            return _buildingSecurityClient.CreateCase(BuildingSecurityUser.BuildingSecurityCookie, caseModel.Title);
        }

        [HttpPut]
        public Case Put(CaseUpdateRequest caseUpdate)
        {
            if (caseUpdate == null)
            {
                throw new ArgumentNullException("caseUpdate");
            }

            ThrowBadRequestIfRequestInvalid();
            switch (caseUpdate.UpdatedField)
            {
                case "title":
                    return UpdateCaseTitle(caseUpdate.CaseId, caseUpdate.Title);

                case "owner":
                    return UpdateCaseOwner(caseUpdate.CaseId, caseUpdate.Owner);

                case "status":
                    return UpdateCaseStatus(caseUpdate.CaseId, caseUpdate.Status);
            }

            // If you get here, we have changed the CaseUpdateRequest validation to allow another field and the above switch has not been updated.
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.ErrorInvalidRequest));
        }

        private Case UpdateCaseTitle(string caseId, string newTitle)
        {
            return _buildingSecurityClient.UpdateCase(BuildingSecurityUser.BuildingSecurityCookie, caseId, new { Title = newTitle });
        }

        private Case UpdateCaseOwner(string caseId, string newOwner)
        {
            return _buildingSecurityClient.UpdateCase(BuildingSecurityUser.BuildingSecurityCookie, caseId, new { Owner = newOwner });
        }

        private Case UpdateCaseStatus(string caseId, string newStatus)
        {
            return _buildingSecurityClient.UpdateCase(BuildingSecurityUser.BuildingSecurityCookie, caseId, new { Status = newStatus });
        }

        private void ThrowBadRequestIfRequestInvalid()
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(x => x.Errors.Where(error => !string.IsNullOrWhiteSpace(error.ErrorMessage)).Select(error => error.ErrorMessage));
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(", ", errorList)));
            }
        }
    }
}