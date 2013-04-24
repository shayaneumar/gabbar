/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.Diagnostics
{
    /// <summary>
    /// The possible logging levels
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        //0x01, 0x0F, 0x3F, and 0xFF remain available if more levels are needed

        /// <summary>
        /// A Trace logging level
        /// </summary>
        Verbose = 0x7F,

        /// <summary>
        /// An Information logging level
        /// </summary>
        Information = 0X1F,
        /// <summary>
        /// A Warning logging level
        /// </summary>
        Warning = 0x07,

        /// <summary>
        /// An Error logging level
        /// </summary>
        Error = 0x03,
    }
}