
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
            this.LogEntryId = logEntryId;

            this.m_ExtendedProperties = new Hashtable();
            this.ExtendedProperties["logEntryNumber"] = logEntryId;
        }
    }

}
