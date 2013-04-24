/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Class containing the date/time and basic text attributes from a Message.
    /// </summary>
    public class MessageDecode
    {
		public string MessageDateTime { get; set; }
        public string MessageTypeText { get; set; }
        public string MessageText { get; set; }
        public string DetailsText { get; set; }
    }
}