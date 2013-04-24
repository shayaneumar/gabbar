/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web;
using System.Web.Mvc;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Security
{
    /// <summary>
    /// Represents an attribute that is used to restrict access to an action method
    /// to users that are logged on to the system.
    /// NOTE:  Only use this as a filter, not as a controller or action attribute.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "Session store is not a normal attribute parameter. It is a service the attribute needs to do it's check."),
     AttributeUsage(AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = false)]
    //setting attribute usage to return type so that this can never be used on an action or controller
    public sealed class MvcLoggedOnAttribute : AuthorizeAttribute
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;

        /// <summary>
        /// Initiates a new instance of the <see cref="MvcLoggedOnAttribute"/> class.
        /// </summary>
        public MvcLoggedOnAttribute() :
            this(null)
        {
            // The default constructor is not intended to be used. It is needed to appease the compiler.
            // Using the default will result in an ArgumentNullException
        }

        /// <summary>
        /// Initiates a new instance of the <see cref="MvcLoggedOnAttribute"/> class.
        /// </summary>
        /// <param name="sessionStore">The <see cref="IBuildingSecuritySessionStore"/> service that contains the user's session information.</param>
        public MvcLoggedOnAttribute(IBuildingSecuritySessionStore sessionStore)
        {
            if (sessionStore == null) throw new ArgumentNullException("sessionStore");
            _sessionStore = sessionStore;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext != null && httpContext.User != null && httpContext.Session != null)
            {
                return base.AuthorizeCore(httpContext) && httpContext.User.IsLoggedOn(_sessionStore, httpContext.Session.SessionID);
            }
            return false;
        }
    }
}
