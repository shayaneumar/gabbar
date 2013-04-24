/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace JohnsonControls.Diagnostics
{
    public class EventAdapter : ILogger
    {
        private readonly EventLog _eventLog;
        private static readonly string DefaultEventSourceName = Assembly.GetExecutingAssembly().FullName;

        public EventAdapter() : this(DefaultEventSourceName)
        {}

        public EventAdapter(string sourceName)
        {
            if (string.IsNullOrWhiteSpace(sourceName)) throw new ArgumentException(@"Can not be null or whitespace.","sourceName");

            _eventLog = new EventLog("Application", ".", sourceName);
        }

        /// <summary>
        /// Writes directly to <see cref="Trace.WriteLine(string)"/>
        /// </summary>
        /// <param name="message">The formatted message.</param>
        /// <param name="logLevel"> </param>
        public void WriteMessage(string message, LogLevel logLevel)
        {
            EventLogEntryType eventType;
            switch (logLevel)
            {
                    case LogLevel.Error:
                    {
                        eventType = EventLogEntryType.Error;
                        break;
                    }
                    case LogLevel.Warning:
                    {
                        eventType = EventLogEntryType.Warning;
                        break;
                    }
                    default:
                    {
                        eventType = EventLogEntryType.Information;
                        break;
                    }
            }
            try
            {
                _eventLog.WriteEntry(message, eventType, 0);
            }
            catch (InvalidOperationException e)
            {
                Trace.Write(e.Message);
            }
            catch (Win32Exception e)
            {
                Trace.Write(e.Message);
            }
            catch(SecurityException e)
            {
                Trace.Write(e.Message);
            }
            
        }
    }
}
