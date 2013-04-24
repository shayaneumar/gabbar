/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public class AlarmActionReply
    {
        public AlarmActionFilter AlarmActionFilter { get; set; }

        public string Command { get; set; }

        public Parameters Parameters { get; set; }

        public AlarmActionResponse[] AlarmActionResponses { get; set; }
    }
}
