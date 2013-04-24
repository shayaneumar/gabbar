/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Diagnostics;

namespace BuildingSecurity.Web.Api
{
    public class KeepUserSessionActive : IActionFilter
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;

        public KeepUserSessionActive(IBuildingSecuritySessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }

        public bool AllowMultiple
        {
            get { return true; }
        }

        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null) throw new ArgumentNullException("actionContext");
            if (continuation == null) throw new ArgumentNullException("continuation");
            var userName = Thread.CurrentPrincipal.Identity.Name;
            IUser user;
            if(_sessionStore.TryRetrieveUser(userName, out user))
            {
                user.KeepAlive();
            }
            else
            {
                Log.Information("ExecuteActionFilterAsync: KeepAlive() Not Called for {0}:{1} because user {2} no longer exists in the session store.", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName, userName);
            }
            return continuation.Invoke();
        }
    }
}
