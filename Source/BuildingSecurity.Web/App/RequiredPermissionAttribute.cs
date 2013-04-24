using System;
using System.Web;
using System.Web.Mvc;

namespace BuildingSecurity.Web.App
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class RequiredPermissionAttribute : AuthorizeAttribute
    {
        public RequiredPermissionAttribute(string roles)
        {
            if (roles == null) throw new ArgumentNullException("roles");
            Roles = roles.ToUpperInvariant();//our permissions are forced to be uppercase;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            throw new HttpException(403,"Access Denied");
        }
    }
}
