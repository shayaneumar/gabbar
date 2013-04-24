/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web;
using System.Web.Mvc;

namespace BuildingSecurity.Web.App
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class HttpErrorHandlerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.ExceptionHandled)
            {
                return;
            }
            var result = filterContext.Exception as HttpException;

            if (result == null) //Don't do anything if there is an exception
            {
                return;
            }

            if (result.GetHttpCode() == 403)
            {
                filterContext.Result = new ViewResult { ViewName = "~/Views/Error/403.cshtml", };
                filterContext.HttpContext.Response.StatusCode = result.GetHttpCode();
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
