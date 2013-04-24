/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web.Mvc;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App
{
    public class KeepUserSessionActive : IActionFilter
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;

        public KeepUserSessionActive(IBuildingSecuritySessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            var userName = filterContext.HttpContext.User.Identity.Name;
            IUser user;
            if (_sessionStore.TryRetrieveUser(userName, out user))
            {
                user.KeepAlive();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {}
    }
}
