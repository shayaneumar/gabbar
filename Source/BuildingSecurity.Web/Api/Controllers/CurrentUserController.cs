/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System.Web.Http;

namespace BuildingSecurity.Web.Api.Controllers
{
    public class CurrentUserController : ApiController
    {
        [AllowAnonymous]
        public string Get()
        {
            return User.Identity.Name;
        }
    }
}
