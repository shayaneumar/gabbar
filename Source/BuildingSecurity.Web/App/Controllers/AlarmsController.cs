/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Web.Mvc;
using BuildingSecurity.Web.App.Models;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App.Controllers
{
    [RequiredPermission(PermissionNames.CanViewAlarmManager)]
    public class AlarmsController : Controller
    {

        // GET: /Alarms/Active
        public ActionResult Active(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            UserExtensions.PopulateViewBag(user, ViewBag);
            return View(new ActiveAlarmsModel(user.UserPreferences.SelectedTimeZone));
        }
    }
}
