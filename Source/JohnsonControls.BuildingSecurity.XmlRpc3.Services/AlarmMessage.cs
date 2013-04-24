/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Class based on a Message, also containing an AlarmMessageDetails object for Alarms (both on demand list and incoming Real-Time).
    /// </summary>
    public class AlarmMessage: Message
    {
        public AlarmMessageDetails MessageDetails { get; set; }
    }
}