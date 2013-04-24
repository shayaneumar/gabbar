/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Diagnostics;

namespace JohnsonControls.Diagnostics
{
    /// <summary>
    /// Writes messages to the trace log
    /// </summary>
    public class TraceAdapter : ILogger
    {
        public void WriteMessage(string message, LogLevel level)
        {
            Trace.WriteLine(level+": "+message);
        }
    }
}
