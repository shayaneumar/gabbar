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
    public class AlarmManagerController : Controller
    {
        // GET: /AlarmManager
        public ActionResult Index(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            UserExtensions.PopulateViewBag(user, ViewBag);
            return View(new ActiveAlarmsModel(user.UserPreferences.SelectedTimeZone));
        }
        
        // GET: /AlarmManager
        public ActionResult IndexV2(IUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            UserExtensions.PopulateViewBag(user, ViewBag);
            return View(new ActiveAlarmsModel(user.UserPreferences.SelectedTimeZone));
        }
    }
}
