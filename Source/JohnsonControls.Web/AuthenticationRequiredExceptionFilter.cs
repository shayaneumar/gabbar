/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Web.Mvc;
using System.Web.Routing;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Exceptions;

namespace JohnsonControls.Web
{
    /// <summary>
    /// Represents an attribute that is used to handle an exception that is thrown by an action method.
    /// </summary>
    public class AuthenticationRequiredExceptionFilter : IExceptionFilter
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;

        public AuthenticationRequiredExceptionFilter(IBuildingSecuritySessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }

        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param><exception cref="T:System.ArgumentNullException">The <paramref name="filterContext"/> parameter is null.</exception>
        public virtual void OnException(ExceptionContext filterContext)
        {
            if ((filterContext == null) || filterContext.ExceptionHandled)
            {
                return;
            }

            if (filterContext.Exception is AuthenticationRequiredException)
            {
                IUser user;
                if (_sessionStore.TryRetrieveUser(filterContext.HttpContext.User.Identity.Name, out user))
                {
                    user.SignOut();
                    _sessionStore.ClearUser(filterContext.HttpContext.User.Identity.Name);
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "LogOn" }, });
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
