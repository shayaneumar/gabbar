/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Diagnostics;

namespace JohnsonControls.Diagnostics
{
    /// <summary>
    /// A static class that can be used for logging messages. By default
    /// messages are logged using the <see cref="Trace"/> class. You can
    /// override this behavior by specifying a different logger using the
    /// <see cref="Logger"/> property.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Instance of the logging adapter
        /// </summary>
        private static ILogger _traceLogger;
        private static ILogger _logger;

        /// <summary>
        /// The minimum level a message must be tagged with for it to be logged.
        /// Defaults to LogLevel.Information.
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the trace logger. The trace logger will write all events out regardless of
        /// specified log level.
        /// </summary>
        /// <value>
        /// The trace logger.
        /// </value>
        public static ILogger TraceLogger
        {
            get { return _traceLogger ?? (_traceLogger = new TraceAdapter()); }
        }

        static Log()
        {
            LogLevel = LogLevel.Information;
        }

        /// <summary>
        /// Gets or sets the logger. The logger will only write out events which have a severity
        /// greater than or equal to the log level
        /// </summary>
        public static ILogger Logger
        {
            get { return _logger ?? (_logger = new EventAdapter()); }
            set { _logger = value; }
        }

        /// <summary>
        /// Writes a Verbose statement to the logging adapter.        
        /// Log Level must be set to less than or equal to 1 in order for this message
        /// to be logged.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Verbose(string format, params object[] args)
        {
            WriteToLog(LogLevel.Verbose, format, args);
        }

        /// <summary>
        /// Writes an Information statement as a Windows Event and writes to the logging adapter.
        /// Log Level must be set to less than or equal to 2 in order for this message
        /// to be logged.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Information(string format, params object[] args)
        {
            WriteToLog(LogLevel.Information, format, args);
        }

        /// <summary>
        /// Writes a Warning statement to the logging adapter.  
        /// Log Level must be set to less than or equal to 3 in order for this message
        /// to be logged.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Warning(string format, params object[] args)
        {
            WriteToLog(LogLevel.Warning, format, args);
        }

        /// <summary>
        /// Writes an Error statement as a Windows Event and writes to the logging adapter.
        /// Log Level must be set to less than or equal to 4 in order for this message
        /// to be logged.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(string format, params object[] args)
        {
            WriteToLog(LogLevel.Error, format, args);
        }

        /// <summary>
        /// Determines whether or not message should be logged depending on the
        /// LogLevel, then formats the message, and writes the message.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="formatString">The format string.</param>
        /// <param name="values">The values.</param>
        private static void WriteToLog(LogLevel level, string formatString, params object[] values)
        {
            string message;
            try
            {
                message = string.Format(formatString, values);
            }
            catch (FormatException)
            {
                message = "Invalid log message " + formatString + string.Join(", ", values);
            }

            TraceLogger.WriteMessage(message, level);

            if(LogLevel.HasFlag(level)) Logger.WriteMessage(message, level);

        }
    }
}
