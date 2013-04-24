/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web;

namespace BuildingSecurity.Web.Security
{
    public class HttpSessionManager : IHttpSessionManager
    {
        public string RetrieveSessionId()
        {
            var browserId = HttpContext.Current.Request.Cookies["browserId"];
            if (browserId == null)
            {
                var newId = Guid.NewGuid().ToString();
                var browserIdCookie = new HttpCookie("browserId", newId);
                HttpContext.Current.Request.Cookies.Add(browserIdCookie);
                HttpContext.Current.Response.Cookies.Add(browserIdCookie);
                return newId;
            }

            return browserId.Value;
        }

        public bool CurrentSessionIs(string sessionId)
        {
            return RetrieveSessionId() == sessionId;
        }
    }
}
