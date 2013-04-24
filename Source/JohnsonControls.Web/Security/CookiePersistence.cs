/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.Web.Security
{
    /// <summary>
    /// Specifies the persistence of cookies used in a web application.
    /// </summary>
    public enum CookiePersistence
    {
        /// <summary>
        /// Specifies a cookie that is only valid for the current browser session.
        /// </summary>
        SingleSession,
        
        /// <summary>
        /// Specifies a persistent cookie (one that is saved across browser sessions)
        /// </summary>
        AcrossSessions
    }
}
