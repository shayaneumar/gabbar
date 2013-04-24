using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.App
{
    public class MvcBuildingSecurityProviderFactory : ValueProviderFactory
    {
        private readonly IBuildingSecuritySessionStore _sessionStore;

        public MvcBuildingSecurityProviderFactory(IBuildingSecuritySessionStore sessionStore)
        {
            if (sessionStore == null) throw new ArgumentNullException("sessionStore");
            _sessionStore = sessionStore;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");
            IUser user;
            _sessionStore.TryRetrieveUser(controllerContext.HttpContext.User.Identity.Name, out user);
            var valueDictionary = new Dictionary<string, object> { { @"user", user } };
            return new DictionaryValueProvider<object>(valueDictionary, CultureInfo.CurrentCulture);

        }

    }
}