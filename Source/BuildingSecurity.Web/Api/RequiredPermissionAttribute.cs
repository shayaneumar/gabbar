using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BuildingSecurity.Web.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class RequiredPermissionAttribute : AuthorizeAttribute
    {
        public RequiredPermissionAttribute(string roles)
        {
            if (roles == null) throw new ArgumentNullException("roles");
            Roles = roles.ToUpperInvariant();//our permissions are forced to be uppercase;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext == null) throw new ArgumentNullException("actionContext");
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}
