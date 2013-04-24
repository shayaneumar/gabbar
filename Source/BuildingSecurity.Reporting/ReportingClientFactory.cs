/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.ServiceModel;
using BuildingSecurity.Reporting.ReportingServices2010;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Diagnostics;

namespace BuildingSecurity.Reporting
{
    public class ReportingClientFactory : IReportingClientFactory
    {
        private readonly IReportServerConfigurationFactory _reportServerConfigurationFactory;

        public ReportingClientFactory(IReportServerConfigurationFactory reportServerConfigurationFactory)
        {
            _reportServerConfigurationFactory = reportServerConfigurationFactory;
        }

        public IReportingClient CreateClient(IUser user)
        {
            ReportServerConfiguration reportServerConfiguration = _reportServerConfigurationFactory.GetConfiguration(user);
            return CreateClient(reportServerConfiguration);
        }

        public IReportingClient CreateClient(ReportServerConfiguration reportServerConfiguration)
        {
            var binding = new BasicHttpBinding("ReportingServicesSoap");
            var ntlmBinding = new BasicHttpBinding("ReportingServicesSoapNtlm");
            if(reportServerConfiguration.ServiceUrl.StartsWith("https",StringComparison.InvariantCultureIgnoreCase))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                ntlmBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            var client = new ReportingClient(reportServerConfiguration, binding);

            try
            {
                client.TestConnection();
            }
            catch (ReportingAuthenticationException ex)
            {
                Log.Information("CreateClient ReportingAuthenticationException, Attempting ReportingServicesSoapNtlm Binding.\n{0}", ex.Message);
                client.Dispose();
                client = new ReportingClient(reportServerConfiguration, ntlmBinding);
            }
            return client;
        }
    }
}
