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
    [DataContract]
    public class CaseNoteModel : IValidatableObject
    {
        private string _text;

        /// <summary>
        /// The title of the case.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "CaseNoteTextRequired", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(2000, ErrorMessageResourceName = "CaseNoteTextTooLong", ErrorMessageResourceType = typeof(Resources))]
        [DataMember(Name = "text")]
        public string Text
        {
            get { return _text; }
            set { _text = value == null ? null : value.Trim(); }
        }

        [Required(ErrorMessageResourceName = "CaseNoteCaseIdRequired", ErrorMessageResourceType = typeof(Resources)), DataMember(Name = "caseId")]
        public string CaseId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate Text is specified
            if (string.IsNullOrWhiteSpace(Text))
                yield return new ValidationResult(Resources.CaseNoteTextRequired);

            // Validate Text Length
            if (Text.Length > 2000)
            {
                yield return new ValidationResult(Resources.CaseNoteTextTooLong);
            }
        }
    }
}