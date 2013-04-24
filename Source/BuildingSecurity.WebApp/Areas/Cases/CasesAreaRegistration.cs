/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web.Mvc;
using System.Web.Optimization;

namespace BuildingSecurity.WebApp.Cases
{
    public class CasesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Cases";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.MapRoute(
                "Cases_default",
                "Cases/{*all}",
                new {Controller = "Cases", action = "Index"});

            // Declare BundleTransforms (based on current build configuration)
#if DEBUG
            IBundleTransform jsTrans = new NoTransform("text/javascript");
            IBundleTransform cssTrans = new NoTransform("text/css");
#else
            IBundleTransform jsTrans = new JsMinify();
            IBundleTransform cssTrans = new CssMinify();
#endif
            Bundle casesViewModels = new Bundle("~/Cases/js", jsTrans);
            casesViewModels.AddFile("~/Areas/Cases/Scripts/cases.js", false);
            casesViewModels.AddDirectory("~/Areas/Cases/ViewModels","*.js");

            BundleTable.Bundles.Add(casesViewModels);

            Bundle casesCss = new  Bundle("~/Cases/css", cssTrans);
            casesCss.AddFile("~/Areas/Cases/Content/cases.css", false);

            BundleTable.Bundles.Add(casesCss);
        }
    }
}
