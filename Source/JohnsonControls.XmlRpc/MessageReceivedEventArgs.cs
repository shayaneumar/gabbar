/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.XmlRpc
{
    /// <summary>
    /// The event arguments for a MessageReceived event.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance of <see cref="MessageReceivedEventArgs"/>
        /// </summary>
        /// <param name="message"></param>
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The message received. The XML Protocol specifies that this
        /// should be an XML message but this class doesn't try to interpret it.
        /// It just presents the message as a string.
        /// </summary>
        public string Message { get; private set; }
    }
}