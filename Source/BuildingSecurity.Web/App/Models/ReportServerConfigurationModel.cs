/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuildingSecurity.Globalization;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App.Models
{
    public class ReportServerConfigurationModel : IValidatableObject
    {
        [Display(Name = "ReportServerModelUseSslLabel", ResourceType = typeof(Resources))]
        public bool UseSsl { get; set; }

        [Display(Name = "ReportServerModelServerLabel", ResourceType = typeof(Resources))]
        public string Server { get; set; }

        [Display(Name = "ReportServerModelWebServiceVirtualDirectoryLabel", ResourceType = typeof(Resources))]
        public string WebServiceVirtualDirectory { get; set; }

        [Display(Name = "ReportServerModelWebServiceUrlLabel", ResourceType = typeof(Resources))]
        public string WebServiceUrl { get; set; }

        [Display(Name = "ReportServerModelDomainLabel", ResourceType = typeof(Resources))]
        public string Domain { get; set; }

        [Display(Name = "ReportServerModelUserNameLabel", ResourceType = typeof(Resources))]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ReportServerModelPasswordLabel", ResourceType = typeof(Resources))]
        public string Password { get; set; }

        public string MessageText { get; set; }
        public string MessageType { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate Server is specified
            if (string.IsNullOrWhiteSpace(Server))
                yield return new ValidationResult(Resources.ReportServerModelServerRequiredErrorMessage);

            // Validate WebServiceVirtualDirectory is specified
            if (string.IsNullOrWhiteSpace(WebServiceVirtualDirectory))
                yield return new ValidationResult(Resources.ReportServerModelWebServiceVirtualDirectoryRequiredErrorMessage);

            // Validate UserName is specified
            if (string.IsNullOrWhiteSpace(UserName))
                yield return new ValidationResult(Resources.ReportServerModelUserNameRequiredErrorMessage);

            // Validate Password is specified
            if (string.IsNullOrEmpty(Password))
                yield return new ValidationResult(Resources.ReportServerModelPasswordRequiredErrorMessage);

            if (!string.IsNullOrWhiteSpace(WebServiceVirtualDirectory))
            {
                var reportServerConfiguration = new ReportServerConfiguration(UseSsl, Server,
                                                                              WebServiceVirtualDirectory,
                                                                              Domain, UserName,
                                                                              Password, Password);

                // Validate the Server (based on ServiceUrl)
                if (!reportServerConfiguration.ServiceUrl.IsValidHttpAbsoluteUrl())
                {
                    yield return new ValidationResult(Resources.ReportServerModelInvalidServer);
                }
            }
        }
    }
}
