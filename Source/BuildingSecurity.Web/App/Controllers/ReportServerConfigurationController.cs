/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Web.Mvc;
using BuildingSecurity.Web.App.Models;
using BuildingSecurity.Reporting;
using JohnsonControls.BuildingSecurity;
using BuildingSecurity.Globalization;
using JohnsonControls.Exceptions;

namespace BuildingSecurity.Web.App.Controllers
{
    [RequiredPermission(PermissionNames.CanViewReportsServerSettings)]
    public class ReportServerConfigurationController : Controller
    {
        private readonly IBuildingSecurityClient _buildingSecurityClient;
        private readonly IReportingClientFactory _reportingClientFactory;

        public ReportServerConfigurationController(IBuildingSecurityClient buildingSecurityClient, IReportingClientFactory reportingClientFactory)
        {
            _buildingSecurityClient = buildingSecurityClient;
            _reportingClientFactory = reportingClientFactory;
        }

        public ActionResult Index(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            // Attempt to read the ApplicationPreference
            ReportServerConfigurationModel viewModel;
            ReportServerConfiguration reportServerConfiguration;
            if (_buildingSecurityClient.TryReadApplicationPreference(user.BuildingSecurityCookie, ApplicationSettings.ReportServerConfiguration, out reportServerConfiguration))
            {
                bool useSsl;
                string server;
                string webServiceVirtualDirectory;

                // Attempt to parse the reportServerConfiguration for the model properties
                bool defaultConfiguration = !TryParseConfiguration(reportServerConfiguration,
                                                                   out useSsl,
                                                                   out server,
                                                                   out webServiceVirtualDirectory);
                viewModel = new ReportServerConfigurationModel
                                {
                                    UseSsl = useSsl,
                                    Server = server,
                                    WebServiceVirtualDirectory = webServiceVirtualDirectory,
                                    Domain = reportServerConfiguration.Domain,
                                    UserName = reportServerConfiguration.UserName,
                                    Password = string.IsNullOrEmpty(reportServerConfiguration.EncryptedPassword)
                                                   ? ""
                                                   : ReportServerConfiguration.ObfuscatedPassword,
                                    MessageText = null,
                                    MessageType = null
                                };

                // If TryParseConfiguration returned false (defaultConfiguration = true),
                // then indicate the default configuration was used
                if (defaultConfiguration)
                {
                    SetMessage(viewModel, MessageType.Info, Resources.ReportServerModelDefaultConfigurationMessage);
                }
            }
            else
            {
                viewModel = new ReportServerConfigurationModel();
                SetMessage(viewModel, MessageType.Info, Resources.ReportServerModelDefaultConfigurationMessage);
            }

            UserExtensions.PopulateViewBag(user, ViewBag);
            return View(viewModel);
        }

        [HttpPost]
        [RequiredPermission(PermissionNames.CanEditReportsServerSettings)]
        [ValidateInput(false)]
        public ActionResult Index(IUser user, ReportServerConfigurationModel model, string submitButton)
        {
            if (user == null) throw new ArgumentNullException("user");
            // Retrieve the current EncryptedPassword
            string currentEncryptedPassword = "";
            ReportServerConfiguration currentReportServerConfiguration;
            if (_buildingSecurityClient.TryReadApplicationPreference(user.BuildingSecurityCookie, ApplicationSettings.ReportServerConfiguration,
                                                                     out currentReportServerConfiguration))
            {
                currentEncryptedPassword = currentReportServerConfiguration.EncryptedPassword;
            }

            if (model != null)
            {
                if(ModelState.IsValid)
                {
                    // Construct the ReportServerConfiguration from the ReportServerModel properties 
                    var reportServerConfiguration = new ReportServerConfiguration(model.UseSsl, model.Server,
                                                                                  model.WebServiceVirtualDirectory,
                                                                                  model.Domain, model.UserName,
                                                                                  model.Password,
                                                                                  currentEncryptedPassword);

                    // Test the connection
                    bool connectionSucceded = TestConfiguration(reportServerConfiguration, model);

                    // If the connection succeeded and submitButton = "Save", then SaveApplicationPreference and update the message
                    if ((connectionSucceded) && (submitButton != null) && (submitButton.ToUpperInvariant() == "SAVE"))
                    {
                        //TODO:Refactor to notify the ReportServerConfigurationFactory of the change, and to pull setting name from a constant
                        _buildingSecurityClient.SaveApplicationPreference(user.BuildingSecurityCookie,
                                                                          ApplicationSettings.ReportServerConfiguration,
                                                                          reportServerConfiguration);

                        SetMessage(model, MessageType.Success, Resources.ReportServerOptionsSaveCompletedMessage);
                    }
                }
                else
                {
                    SetMessage(model, MessageType.Error, GetModelStateErrorMessages());                    
                }
            }

            UserExtensions.PopulateViewBag(user, ViewBag);
            return View("Index", model);
        }

        /// <summary>
        /// Attempt to transform a ReportServerConfiguration object into the values used for the ReportServerModel
        /// </summary>
        /// <param name="reportServerConfiguration">Input configuration</param>
        /// <param name="useSsl">Indicates of the ServiceUrl starts with "https"</param>
        /// <param name="server">IP Address or host name from the ServiceUrl</param>
        /// <param name="webServiceVirtualDirectory">Virtual Directory component of the ServiceUrl</param>
        /// <returns>True if the ReportServerConfiguration was successfully parsed, else false if an exception occurs
        /// during the attempt to construct the Uris or other output values.  If it fails to parse the input configuration,
        /// this will populate the output values for the default model</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static bool TryParseConfiguration(ReportServerConfiguration reportServerConfiguration, out bool useSsl, out string server, out string webServiceVirtualDirectory)
        {
            try
            {
                var serviceUri = new Uri(reportServerConfiguration.ServiceUrl);
                
                useSsl = serviceUri.GetLeftPart(UriPartial.Scheme).StartsWith("https", StringComparison.OrdinalIgnoreCase);
                string scheme = useSsl ? "https://" : "http://";
                server = serviceUri.GetLeftPart(UriPartial.Authority).Replace(scheme, "");
                webServiceVirtualDirectory = serviceUri.ToString().Replace(scheme + server + "/", "");

                return true;
            }
            catch (Exception)
            {
                // If an exception was thrown, set set the output values to the default configuration
                useSsl = false;
                server = "localhost";
                webServiceVirtualDirectory = "ReportServer";

                return false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool TestConfiguration(ReportServerConfiguration reportServerConfiguration, ReportServerConfigurationModel model)
        {
            try
            {
                using (var reportingClient = _reportingClientFactory.CreateClient(reportServerConfiguration))
                {
                    reportingClient.TestConnection();
                }

                // If CreateClient and TestConnection did not throw an exception,
                // then set the message in the model to succeeded, and return true
                SetMessage(model, MessageType.Success, Resources.ReportServerOptionsConnectionSucceeded);
                return true;
            }
            catch (ReportingEndpointException)
            {
                // Invalid Report Server Uri Format or Endpoint
                SetMessage(model, MessageType.Error, Resources.ReportingEndpointException);
            }
            catch (ReportingAuthenticationException)
            {
                // Invalid Credentials
                SetMessage(model, MessageType.Error, Resources.ReportingAuthenticationException);
            }
            catch (ReportingSchemeException)
            {
                // Invalid Scheme (http vs. https)
                SetMessage(model, MessageType.Error, Resources.ReportingSchemeException);
            }
            catch (UntrustedCertificateException)
            {
                SetMessage(model, MessageType.Error, Resources.ReportServerUntrustedCertificateMessage);
            }
            catch (Exception)
            {
                SetMessage(model, MessageType.Error, Resources.ReportServerOptionsConnectionFailedMessage);
            }

            return false;
        }

        private string GetModelStateErrorMessages()
        {
            return ModelState[""].Errors.Aggregate("", (current, modelError) => current + (modelError.ErrorMessage + " "));
        }

        private static void SetMessage(ReportServerConfigurationModel model, string messageType, string messageText)
        {
            model.MessageType = messageType;
            model.MessageText = messageText;
        }
    }
}
