/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.Diagnostics
{
    /// <summary>
    /// Defines the methods necessary for a logging.
    /// </summary>
    public interface ILogger
    {
        void WriteMessage(string message, LogLevel level);
    }
}
