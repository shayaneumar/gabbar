/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using JohnsonControls.Diagnostics;

namespace BuildingSecurity.Web
{
    public static class HelpLinkLanguageHelper
    {
        public static string GetLocalizedHelpLink(this HtmlHelper html, string helpLink)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            if (helpLink == null)
            {
                throw new ArgumentNullException("helpLink");
            }
            // Get current Culture
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            // Replace main help folder name with the localized version.
            string newHelpPath = helpLink.Replace("/Help/",
                                                  string.Format(CultureInfo.InvariantCulture, "/Help-{0}/", currentCulture.TwoLetterISOLanguageName));

            // Strip the query parameters from the URL
            string rawHelpPath = newHelpPath.Split(new [] {'?', '>', '#'})[0];
            try
            {
                // Return the Localized help if the Path exists on the web server, otherwise return the original path.
                return VirtualPathUtility.ToAbsolute(System.IO.File.Exists(HttpContext.Current.Server.MapPath(rawHelpPath)) ? newHelpPath : helpLink);
            }
            catch (HttpException)
            {
                // If any problem occurs, return English version.
                Log.Error("HttpException occurred while attempting to load Help Link: {0} for Culture Code: {1}", helpLink, currentCulture.TwoLetterISOLanguageName);
                return helpLink;
            }
        }
    }
}
