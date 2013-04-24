/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System.Web;
using System.Web.Http;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected IBuildingSecuritySessionStore SessionStore { get; private set; }

        public IUser BuildingSecurityUser {
            get
            {
                IUser user;
                SessionStore.TryRetrieveUser(HttpContext.Current.User.Identity.Name, out user);
                return user;
            }
        }

        protected BaseApiController(IBuildingSecuritySessionStore sessionStore)
        {
            SessionStore = sessionStore;
        }
    }
}
