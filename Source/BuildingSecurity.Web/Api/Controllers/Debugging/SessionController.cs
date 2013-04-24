/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
#if DEBUG
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using JohnsonControls.BuildingSecurity;

namespace BuildingSecurity.Web.Api.Controllers.Debugging
{
    [Authorize(Users="cardkey")]
    public class SessionController : BaseApiController
    {
        public SessionController(IBuildingSecuritySessionStore sessionStore) : base(sessionStore)
        {}

        public IEnumerable<string> Get()
        {
            var activeSessions = SessionStore.ToList();
            return activeSessions.Select(x=>x.Name);
        }
    }
}
#endif
