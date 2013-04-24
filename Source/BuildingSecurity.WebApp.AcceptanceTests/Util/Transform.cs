/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace BuildingSecurity.WebApp.AcceptanceTests.Util
{
    public static class Transformer
    {
        private static readonly Dictionary<string, string> NameToElementId = new Dictionary<string, string>
        {
            {"all cases", "caselist"}
        };

        /// <summary>
        /// Returns an elementId based on the specified name
        /// </summary>
        /// <param name="name">Name, as it is referenced within a .feature file</param>
        /// <returns>elementId that corresponds with the specified name</returns>
        public static string GetElementIdByName(string name)
        {
            // Should we ever need alternate names for different featues, consider prefixing the key with the feature title (i.e. FeatureContext.Current.FeatureInfo.Title)
            if (NameToElementId.ContainsKey(name))
            {
                return NameToElementId[name];
            }

            return name.Replace(" ", "");
        }
    }
}
