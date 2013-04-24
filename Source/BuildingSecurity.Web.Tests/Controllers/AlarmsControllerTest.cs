/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using BuildingSecurity.Web.App.Controllers;
using BuildingSecurity.Web.App.Models;
using JohnsonControls.BuildingSecurity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BuildingSecurity.WebApp.Tests.Controllers
{
    /// <summary>
    /// Summary description for AlarmsControllerTest
    /// </summary>
    [TestClass]
    public class AlarmsControllerTest
    {
        private readonly MockRepository _mockFactory;
        private readonly IBuildingSecuritySessionStore _sessionStore;
        private readonly IUser _user;
        private readonly Mock<ControllerContext> _controllerContextStub;
        private const string UserName = "userName";
        private const string FullName = "fullName";

        public AlarmsControllerTest()
        {
            _mockFactory = new MockRepository(MockBehavior.Loose);

            //need to mock Thread.CurrentPrincipal.Identity.Name
            var principalStub = _mockFactory.Create<IPrincipal>();
            var identityStub = _mockFactory.Create<IIdentity>();

            identityStub.SetupGet(n => n.Name).Returns(UserName);
            principalStub.SetupGet(p => p.Identity).Returns(identityStub.Object);

            var mockHttpContext = _mockFactory.Create<HttpContextBase>();
            mockHttpContext.SetupGet(x => x.User).Returns(principalStub.Object);
            _controllerContextStub = _mockFactory.Create<ControllerContext>();
            _controllerContextStub.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);
            _controllerContextStub.SetupGet(y => y.IsChildAction).Returns(false);

            _user =new MutableUser{ Name =UserName,FullName = FullName,UserPreferences = new UserPreferences(TimeZoneInfo.Local.Id), Partitions = Enumerable.Empty<Partition>(), CanViewAlarmManager = true};
            _sessionStore = new BuildingSecuritySessionStore();
            _sessionStore.AddUser(_user);
        }

        public AlarmsController GetAlarmController()
        {
            return new AlarmsController();
        }

        [TestMethod]
        public void ActiveTestTimeZone()
        {
            // Arrange
            var controller = GetAlarmController();
            controller.ControllerContext = _controllerContextStub.Object;

            // Act
            var result = (ViewResult)controller.Active(_user);
            var activeAlarmsModel = (ActiveAlarmsModel)result.Model;

            // Assert
            Assert.AreEqual(activeAlarmsModel.UserTimeZone, TimeZoneInfo.Local.Id);
        }
    }
}
