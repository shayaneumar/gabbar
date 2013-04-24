/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Exceptions;

namespace JohnsonControls.Web
{
    public class AuthenticationRequiredExceptionApiFilter : IExceptionFilter
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;

        public AuthenticationRequiredExceptionApiFilter(IBuildingSecuritySessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }

        public bool AllowMultiple
        {
            get { return true; }
        }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (actionExecutedContext.Exception is AuthenticationRequiredException)
                {
                    IUser user;
                    if (
                        _sessionStore.TryRetrieveUser(Thread.CurrentPrincipal.Identity.Name, out user))
                    {
                        user.SignOut();
                        _sessionStore.ClearUser(Thread.CurrentPrincipal.Identity.Name);
                        actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                }
            }, cancellationToken);
        }
    }
}
