//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 

namespace Ignite.Framework.Micro.Common.Contract.Logging
{
    using System;
    using System.Collections;

    using Ignite.Framework.Micro.Common.Errors;
    using Ignite.Framework.Micro.Common.Exceptions;

    /// <summary>
    /// Wraps the common information to capture and log.
    /// </summary>
    public class LogEntry
    {
        private string m_LoggerName;
        /// <summary>
        /// The name of the logger that recorded this entry.
        /// </summary>
        public string LoggerName
        {
            get { return m_LoggerName; }
        }

        /// <summary>
        /// The unique identifier for the log entry.
        /// </summary>
        public string LogEntryId { get; internal set; }

        /// <summary>
        /// The message to be logged.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The identifier of the event that the log entry was raised under.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// The priority of the log entry.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The logging severity level to associate with the message.
        /// </summary>
        public TraceEventType Severity { get; set; }

        /// <summary>
        /// The datetime the log event was raised.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The name of the machine where the log event was raised.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// The identifier of the thread the log event was raised under.
        /// </summary>
        public string Win32ThreadId { get; set; }

        private readonly Hashtable m_ExtendedProperties;
        /// <summary>
        /// Extended logging properties.
        /// </summary>
        public IDictionary ExtendedProperties
        {
            get { return this.m_ExtendedProperties; }
        }

        /// <summary>
        /// The error severity.
        /// </summary>
        public Severity ErrorSeverity { get; set; }

        /// <summary>
        /// The  error category.
        /// </summary>
        public ErrorCategory ErrorCategory { get; set; }

        /// <summary>
        /// The type of error.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The exception thrown within the code.
        /// </summary>
        public BaseException BaseException { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class. 
        /// </summary>
        public LogEntry(string logEntryId)
        {
            LogEntryId = logEntryId;

            m_LoggerName = "Undefined";
            m_ExtendedProperties = new Hashtable();

            ExtendedProperties["logEntryNumber"] = logEntryId;
        }

        /// <summary>
        /// Associates a given logger name with the entry.
        /// </summary>
        /// <param name="loggerName">
        /// The name of the logger to associate with the entry.
        /// </param>
        public void SetLoggerName(string loggerName)
        {
            m_LoggerName = loggerName;
        }
    }

}
