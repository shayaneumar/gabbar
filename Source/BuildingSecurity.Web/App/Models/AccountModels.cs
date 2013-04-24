/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.ComponentModel.DataAnnotations;

namespace BuildingSecurity.Web.App.Models
{
    public class LogOnModel
    {
        [Required(ErrorMessageResourceName = "LogOnModelUserNameRequiredErrorMessage", ErrorMessageResourceType = typeof(Globalization.Resources))]
        [Display(Name = "LogOnModelUserNameLabel", ResourceType = typeof(Globalization.Resources))]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceName = "LogOnModelPasswordRequiredErrorMessage", ErrorMessageResourceType = typeof(Globalization.Resources))]
        [Display(Name = "LogOnModelPasswordLabel", ResourceType = typeof(Globalization.Resources))]
        public string Password { get; set; }
    }
}
