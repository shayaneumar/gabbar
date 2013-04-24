/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Configuration;
using BuildingSecurity.Reporting.ReportingServices2010;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.BuildingSecurity.Pseudo.Client;
using JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime;
using JohnsonControls.BuildingSecurity.XmlRpc3.Client;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;

#if IN_MEMORY_SETTINGS
using System.Collections.Generic;
using JohnsonControls.Serialization.Xml;
#else
using System.Runtime.Caching;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services.Caching;
using JohnsonControls.Runtime.Caching;
#endif

using Ninject;
using Ninject.Extensions.Conventions;

namespace BuildingSecurity.Web
{
    public class BootLoader : IBootLoader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "MGW: We expect that the injection code WILL have lots of dependencies.")]
        public IKernel CreateKernel(Uri serviceUrl, string realTimeServiceAddress)
        {
            StandardKernel kernel = null;

            try
            {
                kernel = new StandardKernel();

                kernel.Bind(x => x.FromAssembliesMatching("BuildingSecurity.*", "JohnsonControls.*")
                                        .SelectAllClasses()
                                        .Excluding<ReportingClient>()
                                        .Excluding<AlarmService>()
                                        .Excluding<MessageProcessingClient>()
                                        .Excluding<BuildingSecuritySessionStore>()
                                        .Excluding<BuildingSecurityClient>()
                                        .BindDefaultInterface()
                                        .Configure((b, c) => b.WithConstructorArgument("serviceUrl", serviceUrl))
                    );

#if IN_MEMORY_SETTINGS
                ReportServerConfiguration reportServer = new ReportServerConfiguration("http://10.10.93.183/ReportServer", "", "cwelchmi", "").CloneWithNewPassword("cwelchmi");
                var appSettings = new SettingsDictionary
                                                     {
                                                         {
                                                             ApplicationSettings.ReportServerConfiguration,
                                                             new DataSerializer<ReportServerConfiguration>().Serialize(
                                                                 reportServer)
                                                             }
                                                     };

                kernel.Bind<ITypedApplicationPreference>().ToMethod(x => new InMemoryApplicationSettings(appSettings, new Dictionary<string, SettingsDictionary>())).InSingletonScope();
#else
                kernel.Bind<ITypedApplicationPreference>().ToMethod(x => new CachingApplicationPreferences(new ApplicationPreferenceService(serviceUrl), new Cache(MemoryCache.Default, "ITypedApplicationPreference")));
#endif
                kernel.Bind<IBuildingSecuritySessionStore>().To<BuildingSecuritySessionStore>().InSingletonScope();

                kernel.Rebind<IBuildingSecurityClient>().To<BuildingSecurityClient>().InSingletonScope();
                kernel.Rebind<ISimulatorClient>().ToConstant<ISimulatorClient>(null);

                if (@"true".Equals(ConfigurationManager.AppSettings["UseSimulation"], StringComparison.InvariantCultureIgnoreCase))
                {
                    kernel.Rebind<IBuildingSecurityClient>().To<PseudoBuildingSecurityClient>().InSingletonScope();
                    kernel.Rebind<Scheduler>().To<Scheduler>().InSingletonScope();
                    kernel.Rebind<ISimulatorClient>().To<InMemorySimulationClient>().InSingletonScope();
                }

                kernel.Bind<ITypedSessionManagement>().ToMethod(x => new SessionManagementService(serviceUrl));
                kernel.Bind<ITypedAlarmService>().ToMethod(x => new AlarmService(serviceUrl, realTimeServiceAddress)).InSingletonScope();
                kernel.Bind<ITypedSystemInformationService>().ToMethod(x => new SystemInformationService(serviceUrl)).InSingletonScope();

                return kernel;
            }
            catch
            {
                if (kernel != null)
                {
                    kernel.Dispose();
                }

                throw;
            }
        }
    }
}
