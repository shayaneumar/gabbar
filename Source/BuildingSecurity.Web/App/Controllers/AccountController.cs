/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web.Mvc;
using System.Web.WebPages;
using BuildingSecurity.Web.App.Models;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Web.Security;

namespace BuildingSecurity.Web.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;
        private readonly IAuthenticationServices _authenticationServices;

        public AccountController(IBuildingSecuritySessionStore sessionStore, IAuthenticationServices authenticationServices)
        {
            _sessionStore = sessionStore;
            _authenticationServices = authenticationServices;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult LogOn()
        {
            if (_authenticationServices.IsCurrentUserLoggedOn)
            {
                IUser user;
                if (_sessionStore.TryRetrieveUser(User.Identity.Name, out user))
                {
                    return StartPageForUser(user);
                }
            }

            return View();
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string requestedResource)
        {
            if (model != null && ModelState.IsValid)
            {
                IUser user;
                string errorMessage;
                if (_authenticationServices.TryValidateUser(model.UserName, model.Password, out user, out errorMessage))
                {
                    _authenticationServices.SetAuthenticationCookie(user.Name, CookiePersistence.SingleSession);
                    if (IsUrlLocal(requestedResource))
                    {
                        // Go to requesting URL
                        return Redirect(requestedResource);
                    }

                    return StartPageForUser(user);
                }

                ModelState.AddModelError("", errorMessage);
            }

            // If we got this far, the model's state is invalid or log on failed, redisplay form
            return View(model);
        }

        // GET: /Account/LogOff
        public ActionResult LogOff(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            _authenticationServices.RemoveAuthenticationCookie();
            _sessionStore.ClearUser(user.Name);
            user.SignOut();

            return RedirectToAction("LogOn", "Account");
        }

        private RedirectToRouteResult StartPageForUser(IUser user)
        {
            if (user.HasPermission(PermissionNames.CanViewAlarmManager))
            {
                return RedirectToAction("Index", "AlarmManager");
            }

            if (user.HasPermission(PermissionNames.CanViewReports))
            {
                return RedirectToAction("Index", "Reports");
            }

            if (user.HasPermission(PermissionNames.CanEditSystemSettings))
            {
                return RedirectToAction("Index", "ReportServerConfiguration");
            }

            return RedirectToAction("Index", "Unauthorized");
        }

        // Url.IsLocalUrl() equivalent
        // The above was preventing unit testing
        // Traced down and found this is the actual call being executed
        // It is an extension method on HttpRequestBase that never references the caller
        // It is in effect a string extension
        private static bool IsUrlLocal(string url)
        {
            return RequestExtensions.IsUrlLocalToHost(null, url);
        }
    }
}
