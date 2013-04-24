/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web.Mvc;
using BuildingSecurity.Web.App;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.WebApp.Cases.Controllers
{
    [RequiredPermission(PermissionNames.CanViewCaseManager)]
    public class CasesController : Controller
    {
        //
        // GET: /Cases/Cases/

        public ActionResult Index(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            UserExtensions.PopulateViewBag(user, ViewBag);
            return View();
        }
    }
}
