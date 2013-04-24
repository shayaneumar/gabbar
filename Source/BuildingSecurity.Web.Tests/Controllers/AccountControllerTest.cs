// ----------------------------------------------------------------------------
// 
//   (C) Copyright 2012 - 2013 Johnson Controls, Inc.
//       Use or Copying of all or any part of this program, except as
//       permitted by License Agreement, is prohibited.
// 
// ------------------------------------------------------------------------------

using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BuildingSecurity.Web.App.Controllers;
using BuildingSecurity.Web.App.Models;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BuildingSecurity.WebApp.Tests.Controllers
{
    /// <summary>
    /// Summary description for AccountControllerTest
    /// </summary>
    [TestClass]
    public class AccountControllerTest
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;
        private readonly IUser _user;
        private readonly Mock<ControllerContext> _controllerContextStub;
        private const string UserName = "userName";
        private const string FullName = "fullName";

        public AccountControllerTest()
        {
            var principalStub = new Mock<IPrincipal>();
            var identityStub = new Mock<IIdentity>();

            identityStub.SetupGet(i => i.Name).Returns(UserName);
            principalStub.SetupGet(p => p.Identity).Returns(identityStub.Object);

            var httpContextStub = new Mock<HttpContextBase>();
            httpContextStub.SetupGet(context => context.User).Returns(principalStub.Object);
            _controllerContextStub = new Mock<ControllerContext>();
            _controllerContextStub.SetupGet(c => c.HttpContext).Returns(httpContextStub.Object);
            _controllerContextStub.SetupGet(c => c.IsChildAction).Returns(false);
            _controllerContextStub.SetupGet(c => c.RouteData).Returns((RouteData)null);

            _user = new MutableUser {Name =UserName, FullName = FullName};
            _sessionStore = new BuildingSecuritySessionStore();
            _sessionStore.AddUser(_user);
        }


        public AccountController GetAccountController(IBuildingSecuritySessionStore sessionStore = null, IAuthenticationServices authenticationServices = null)
        {
            return new AccountController(sessionStore ?? _sessionStore, authenticationServices ?? new Mock<IAuthenticationServices>().Object);
        }

        [TestMethod]
        public void LogOnGet_CurrentUserIsNotAuthenticated()
        {
            // Arrange
            var authServicesStub = new Mock<IAuthenticationServices>();
            authServicesStub.SetupGet(a => a.IsCurrentUserLoggedOn).Returns(false);
            var controller = GetAccountController(authenticationServices: authServicesStub.Object);

            // Act
            var result = controller.LogOn();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void LogOnGet_CurrentUserIsAuthenticated()
        {
            // Arrange
            var authServicesStub = new Mock<IAuthenticationServices>();
            authServicesStub.SetupGet(a => a.IsCurrentUserLoggedOn).Returns(true);
            var controller = GetAccountController(authenticationServices: authServicesStub.Object);
            controller.ControllerContext = _controllerContextStub.Object;

            // Act
            var result = controller.LogOn();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void LogOffGet()
        {
            // Arrange
            var authServicesStub = new MockAuthenticationServices(removeAuthCookieHandler: () => { });
            var controller = GetAccountController(authenticationServices: authServicesStub);

            // Act
            var result = controller.LogOff(_user) as RedirectToRouteResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void LogOffGet_RemovesCookie()
        {
            bool cookieRemoveWasCalled = false;
            // Arrange
            var authServicesMock = new MockAuthenticationServices(removeAuthCookieHandler: () => cookieRemoveWasCalled = true);
            var controller = GetAccountController(authenticationServices: authServicesMock);

            // Act
            controller.LogOff(_user);

            // Assert
            Assert.IsTrue(cookieRemoveWasCalled, "Log off failed to remove the user's cookie.");
        }

        const string TestUserName = "test";
        const string TestPassword = "pass";

        [TestMethod]
        public void LogOnPost_ValidUserCausesAuthCookie()
        {
            // Arrange
            var cookieWasSet = false;
            var authServicesMock = new MockAuthenticationServices(passwordValidator: (x, y) => true,
                                                                  setAuthenticationCookieHandler:(x, y) => cookieWasSet = true,
                                                                  userAccount: new MutableUser());

            var controller = GetAccountController(authenticationServices: authServicesMock);

            // Act
            controller.LogOn(new LogOnModel { UserName = TestUserName, Password = TestPassword }, "/desired/url");

            // Assert
            Assert.IsTrue(cookieWasSet);
        }

        [TestMethod]
        public void LogOnPost_ValidUser_LocalUrl()
        {
            // Arrange
            var authServicesStub = new MockAuthenticationServices(passwordValidator: (x, y) => true,
                                                                  setAuthenticationCookieHandler: (x, y) => { },
                                                                  userAccount: new MutableUser());

            var controller = GetAccountController(authenticationServices: authServicesStub);

            // Act
            var result = controller.LogOn(new LogOnModel { UserName = TestUserName, Password = TestPassword }, "/desired/url");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public void LogOnPost_ValidUser_EmptyUrl()
        {
            // Arrange
            var authServicesStub = new MockAuthenticationServices(passwordValidator: (x, y) => true,
                                                                  setAuthenticationCookieHandler: (x, y) => { },
                                                                  userAccount:new MutableUser());


            var controller = GetAccountController(authenticationServices: authServicesStub);

            // Act
            var result = controller.LogOn(new LogOnModel { UserName = TestUserName, Password = TestPassword }, string.Empty);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void LogOnPost_InValidUser()
        {
            // Arrange
            var authServicesStub = new MockAuthenticationServices(passwordValidator: (x, y) => false);

            var controller = GetAccountController(authenticationServices: authServicesStub);

            // Act
            var result = controller.LogOn(new LogOnModel { UserName = TestUserName, Password = TestPassword }, string.Empty);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void LogOnPost_NullModel()
        {
            // Arrange
            var controller = new AccountController(null, null);

            // Act
            var result = controller.LogOn(null, string.Empty) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
