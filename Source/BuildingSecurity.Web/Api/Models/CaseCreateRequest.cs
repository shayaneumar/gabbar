/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using BuildingSecurity.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BuildingSecurity.Web.Api.Models
{
    [DataContract]
    public class CaseCreateRequest
    {
        private string _title;

        /// <summary>
        /// The title for the new case.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "CaseTitleRequired", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(255, ErrorMessageResourceName = "CaseTitleExceedsMaxLength", ErrorMessageResourceType = typeof(Resources))]
        [DataMember(Name = "title")]
        public string Title
        {
            get { return _title; }
            set { _title = value == null ? null : value.Trim(); }
        }
    }
}