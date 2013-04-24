/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace BuildingSecurity.Web.Security
{
    public interface IHttpSessionManager
    {
        string RetrieveSessionId();
        bool CurrentSessionIs(string sessionId);
    }
}
