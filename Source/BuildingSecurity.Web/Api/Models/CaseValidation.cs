/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuildingSecurity.Globalization;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Models
{
    public static class CaseValidation
    {
        public static IEnumerable<ValidationResult> ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                yield return new ValidationResult(Resources.CaseTitleRequired);
            }
            else if (title.Length > 255)
            {
                yield return new ValidationResult(Resources.CaseTitleExceedsMaxLength);
            }
        }

        public static IEnumerable<ValidationResult> ValidateOwner(string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                yield return new ValidationResult(Resources.CaseOwnerMissing);
            }
        }

        public static IEnumerable<ValidationResult> ValidateStatus(string status)
        {
            CaseStatus caseStatus;
            if (!Enum.TryParse(status, ignoreCase: true, result: out caseStatus))
            {
                yield return new ValidationResult(Resources.CaseStatusInvalid);
            }
        }
    }
}
 