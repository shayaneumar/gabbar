/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BuildingSecurity.Web;
using BuildingSecurity.Web.App;
using BuildingSecurity.Web.Security;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Diagnostics;
using JohnsonControls.Web;
using Ninject;
using Ninject.Web.Common;
using WebApi = System.Web.Http;

namespace BuildingSecurity.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class BuildingSecurityHttpApplication : NinjectHttpApplication
    {
        private const string P2KServiceUrlKey = "p2k:service:url";
        private const string RealTimeServiceAddressKey = "RealTimeServiceAddress";
        private static IBuildingSecuritySessionStore _sessionStore;
        private static readonly Lazy<IKernel> LazyKernel = new Lazy<IKernel>(() => new BootLoader().CreateKernel(P2KServiceUrl, RealTimeServiceAddress),LazyThreadSafetyMode.ExecutionAndPublication);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is called by asp.net.")]
        void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var browserSessionManager = LazyKernel.Value.Get<IHttpSessionManager>();
                IUser user;
                HttpContext.Current.User = _sessionStore.TryRetrieveUser(User.Identity.Name, out user) && browserSessionManager.CurrentSessionIs(user.UserSessionId)
                    ? new GenericPrincipal(User.Identity, user.Permissions.ToArray()) 
                    : new GenericPrincipal(new GenericIdentity(""),new string[0]);
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }
        }

        protected override void  OnApplicationStopped()
        {
            if (_sessionStore != null)
            {
                foreach (var user in _sessionStore.ToList())
                {
                    user.SignOut();
                }
            }
            base.OnApplicationStopped();
        }

        void Application_EndRequest(Object sender, EventArgs e)
        {
            foreach (var c in Response.Cookies.AllKeys.Select(x=>Response.Cookies[x]))
            {//Secure all cookies as best we can
                c.HttpOnly = true;
                #if ssl
                c.Secure = true;
                #endif
            }
        }

        protected override void OnApplicationStarted()
        {
            Log.Logger = new EventAdapter("JohnsonControls.P2000.WebUI");
            _sessionStore = LazyKernel.Value.Get<IBuildingSecuritySessionStore>();

            AreaRegistration.RegisterAllAreas();

            // Add Filters for CsrfPrevention
            var crsfProtection = new CsrfPreventionFilter();
            GlobalFilters.Filters.Add(crsfProtection);
            WebApi.GlobalConfiguration.Configuration.Filters.Add(crsfProtection);

            // Add Filters for AuthenticationRequiredException
            var authenticationRequiredExceptionFilter = new AuthenticationRequiredExceptionFilter(_sessionStore);
            var authenticationRequiredExceptionApiFilter = new AuthenticationRequiredExceptionApiFilter(_sessionStore);
            GlobalFilters.Filters.Add(authenticationRequiredExceptionFilter);
            WebApi.GlobalConfiguration.Configuration.Filters.Add(authenticationRequiredExceptionApiFilter);

            // Register Filters
            RegisterGlobalMvcFilters(GlobalFilters.Filters);
            RegisterGlobalWebApiFilters(WebApi.GlobalConfiguration.Configuration.Filters);

            // Register Routes
            RegisterRoutes(RouteTable.Routes);
            
            // Register Bundles
            RegisterBundles();

            // This line lets us share the ninject dependency resolver between the mvc and web api call stacks
            WebApi.GlobalConfiguration.Configuration.DependencyResolver = new WebApiResolverAdapter(DependencyResolver.Current);
            var factory = new MvcBuildingSecurityProviderFactory(LazyKernel.Value.Get<IBuildingSecuritySessionStore>());
            ValueProviderFactories.Factories.Add(factory);
            base.OnApplicationStarted();
        }

        protected override IKernel CreateKernel()
        {
            return LazyKernel.Value;
        }

        private static Uri P2KServiceUrl
        {
            get
            {
                return new Uri(ConfigurationManager.AppSettings[P2KServiceUrlKey].Trim());
            }
        }

        private static string RealTimeServiceAddress
        {
            get
            {
                return ConfigurationManager.AppSettings[RealTimeServiceAddressKey];
            }
        }

        private static void RegisterGlobalMvcFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new KeepUserSessionActive(_sessionStore));
            filters.Add(new HttpErrorHandlerAttribute());

            #if ssl
            filters.Add(new RequireHttpsAttribute());
            #endif
        }

        private static void RegisterGlobalWebApiFilters(WebApi.Filters.HttpFilterCollection filters)
        {
            filters.Add(new WebApi.AuthorizeAttribute());
            filters.Add(new Web.Api.KeepUserSessionActive(_sessionStore));
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            WebApi.RouteCollectionExtensions.MapHttpRoute(routes, "ItemsApi",
                "api/v1/alarms/{id}/{controller}"
            );

            WebApi.RouteCollectionExtensions.MapHttpRoute(routes, "DefaultApi",
                "api/v1/{controller}/{id}",
                new { id = WebApi.RouteParameter.Optional }
            );

            routes.MapRoute("default-action", "{controller}", new { action = "Index" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional }
            );
        }

        private static void RegisterBundles()
        {
            // Declare BundleTransforms (based on current build configuration)
#if DEBUG
            IBundleTransform jsTransformer = new NoTransform("text/javascript");
            IBundleTransform cssTransformer = new NoTransform("text/css");
#else
            IBundleTransform jsTransformer = new JsMinify();
            IBundleTransform cssTransformer = new CssMinify();
#endif

            // Create Bundles
            BundleTable.Bundles.Add(CreateLogonJsBundle(jsTransformer));

            Bundle bundleScripts = new Bundle("~/Scripts/js", jsTransformer);
            Bundle bundleStyles = new Bundle("~/Content/css", cssTransformer);
            Bundle bundleJqueryStyles = new Bundle("~/Content/themes/base/css", cssTransformer);

#if DEBUG
            // Disable the automatic use of .min.css files when built in Debug mode
            bundleStyles.EnableFileExtensionReplacements = false;
#endif

            // Populate bundleScripts (with defaults from RegisterTemplateBundles())
            bundleScripts.AddDirectory("~/Scripts", "jquery-*", false, false);
            bundleScripts.AddDirectory("~/Scripts", "jquery.mobile*", false, false);
            bundleScripts.AddDirectory("~/Scripts", "jquery-ui*", false, false);
            bundleScripts.AddDirectory("~/Scripts", "jquery.unobtrusive*", false, false);
            bundleScripts.AddDirectory("~/Scripts", "jquery.validate*", false, false);
            bundleScripts.AddDirectory("~/Scripts", "modernizr*", false, false);
            bundleScripts.AddDirectory("~/Scripts", "jquery.signalR-*", false, false);

            // Populate bundleScripts (with supplemental files)
            bundleScripts.AddFile("~/Scripts/logging.js", false);
            bundleScripts.AddFile("~/Scripts/knockout-2.1.0.js", false);
            bundleScripts.AddFile("~/Scripts/bootstrap.js", false);
            bundleScripts.AddFile("~/Scripts/jci.polyfills.js", false);
            bundleScripts.AddFile("~/Scripts/lib/signals.min.js", false);
            bundleScripts.AddFile("~/Scripts/lib/crossroads.min.js", false);
            bundleScripts.AddFile("~/Scripts/require.js", false);
            bundleScripts.AddFile("~/Scripts/rivets.js", false);
            bundleScripts.AddFile("~/Scripts/require.config.js", false);
            bundleScripts.AddFile("~/Scripts/webapi.ajax.js", false);
            bundleScripts.AddFile("~/Scripts/webapi.csrf.js", false);
            bundleScripts.AddFile("~/Scripts/webapi.errors.js", false);
            bundleScripts.AddFile("~/Scripts/webapi.routes.js", false);

            // Populate bundleStyles (with defaults from RegisterTemplateBundles())
            bundleStyles.AddFile("~/Content/site.css", false);
            bundleStyles.AddDirectory("~/Content/", "jquery.mobile*", false, false);

            // Populate bundleStyles (with supplemental files)
            bundleStyles.AddFile("~/Content/bootstrap.css", false);

            // Populate bundleJqueryStyles (with defaults from RegisterTemplateBundles())
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.core.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.resizable.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.selectable.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.accordion.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.autocomplete.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.button.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.dialog.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.slider.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.tabs.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.datepicker.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.progressbar.css", false);
            bundleJqueryStyles.AddFile("~/Content/themes/base/jquery.ui.theme.css", false);

            // Add Bundles to BundleTable.Bundles
            BundleTable.Bundles.Add(bundleScripts);
            BundleTable.Bundles.Add(bundleStyles);
            BundleTable.Bundles.Add(bundleJqueryStyles);
        }

        private static Bundle CreateLogonJsBundle(IBundleTransform jsTransformer)
        {
            Bundle bundle = new Bundle("~/Scripts/Logon/js", jsTransformer);
            bundle.AddFile("~/Scripts/jquery-1.8.0.js");
            bundle.AddFile("~/Scripts/jquery.validate.js");
            bundle.AddFile("~/Scripts/jquery.unobtrusive-ajax.js");
            bundle.AddFile("~/Scripts/jquery.validate.unobtrusive.js");
            bundle.AddFile("~/Scripts/jci.polyfills.js");
            return bundle;
        }
    }
}
