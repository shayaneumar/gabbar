/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using JohnsonControls.BuildingSecurity;
using JohnsonControls.Web.Security;

namespace BuildingSecurity.WebApp.Tests.Controllers
{
    public class MockAuthenticationServices : IAuthenticationServices
    {
        private readonly Func<string, string, bool> _passwordValidator= (x, y) => { throw new NotImplementedException(); };
        private readonly Action<string, CookiePersistence> _setAuthenticationCookieHandler = (x,y) => { throw new NotImplementedException(); };
        private readonly Action _removeAuthCookieHandler= () => { throw new NotImplementedException(); };
        private readonly IUser _userAccount;
        private readonly string _errorMessage;

        public MockAuthenticationServices(Func<string, string, bool> passwordValidator = null,
            Action<string, CookiePersistence> setAuthenticationCookieHandler = null,
            Action removeAuthCookieHandler = null,
            IUser userAccount = null, string errorMessage = null)
        {
            _passwordValidator = passwordValidator ?? _passwordValidator;
            _setAuthenticationCookieHandler = setAuthenticationCookieHandler??_setAuthenticationCookieHandler;
            _removeAuthCookieHandler = removeAuthCookieHandler??_removeAuthCookieHandler;
            _userAccount = userAccount;
            _errorMessage = errorMessage;
        }

        public bool IsCurrentUserLoggedOn { get; set; }

        public bool TryValidateUser(string userName, string password, out IUser user, out string errorMessage)
        {
            if(_passwordValidator(userName, password))
            {
                user = _userAccount;
                errorMessage = "";
                return true;
            }
            user = null;
            errorMessage = _errorMessage;
            return false;
        }

        public void SetAuthenticationCookie(string userName, CookiePersistence cookiePersistence)
        {
            _setAuthenticationCookieHandler(userName, cookiePersistence);
        }

        public void RemoveAuthenticationCookie()
        {
            _removeAuthCookieHandler();
        }
    }
}
