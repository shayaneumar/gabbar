/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using BuildingSecurity.Globalization;

namespace BuildingSecurity.Web.Api.Models
{
    /// <summary>
    /// All the parameters required to perform an update on a case.
    /// </summary>
    /// <remarks>
    /// This class is used for serialization only. It is not intended for use other than serializing
    /// JSON or XML passed to a web method. It is intended as input only to a web api call.
    /// </remarks>
    [DataContract]
    public class CaseUpdateRequest : IValidatableObject
    {
        public CaseUpdateRequest(string caseId, string updatedField, string title, string owner, string status)
        {
            CaseId = caseId;
            UpdatedField = updatedField;
            Title = title;
            Owner = owner;
            Status = status;
        }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "CaseIdRequired", ErrorMessageResourceType = typeof(Resources))]
        [DataMember(Name = "caseId")]
        public string CaseId { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "CaseUpdateFieldRequired", ErrorMessageResourceType = typeof(Resources))]
        [DataMember(Name = "updatedField")]
        public string UpdatedField { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "status")]
        public string Status { get; private set; }

        [DataMember(Name = "owner")]
        public string Owner { get; private set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (UpdatedField)
            {
                case "title":
                    foreach (ValidationResult result in CaseValidation.ValidateTitle(Title)) yield return result;
                    break;

                case "owner":
                    foreach (ValidationResult result in CaseValidation.ValidateOwner(Owner)) yield return result;
                    break;

                case "status":
                    foreach (ValidationResult result in CaseValidation.ValidateStatus(Status)) yield return result;
                    break;

                default:
                    yield return new ValidationResult(Resources.CaseUpdateFieldIsNotRecognized);
                    break;
            }
        }
    }
}
