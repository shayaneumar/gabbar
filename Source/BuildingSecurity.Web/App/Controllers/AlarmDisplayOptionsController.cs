/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Web.Mvc;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App.Controllers
{
    [RequiredPermission(PermissionNames.CanViewAlarmDisplayOptions)]
    public class AlarmDisplayOptionsController : Controller
    {
        public ActionResult Index(IUser user)
        {
            UserExtensions.PopulateViewBag(user, ViewBag);
            return View();
        }
    }
}
