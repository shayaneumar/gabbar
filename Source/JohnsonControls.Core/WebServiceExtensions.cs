/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.ServiceModel;

namespace JohnsonControls
{
    public static class WebServiceExtensions
    {
        public static void CloseOrAbort(this ICommunicationObject client)
        {
            try
            {
                client.Close();
            }
            catch(CommunicationException)
            {
                client.Abort();
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
        }
    }
}
