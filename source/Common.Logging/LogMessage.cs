
namespace Ignite.Infrastructure.Micro.Common.Logging
{
    using Ignite.Infrastructure.Micro.Common.Errors;
    using Ignite.Infrastructure.Micro.Common.Exceptions;

    /// <summary>
    /// Standard domain class for describing a message to be logged.
    /// </summary>
    /// <remarks>
    /// Groups related information associated to logging messages or errors
    /// into a single domain entity.
    /// </remarks>
    public class LogMessage
    {
        /// <summary>
        /// The message to be logged.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Identifies the type of error that caused the trace.
        /// </summary>
        public TraceEventType SeverityType { get; internal set; }

        /// <summary>
        /// Title to associate with the error message.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The exception (if one exists) associated with the message.
        /// </summary>
        public BaseException Exception { get; set; }

        /// <summary>
        /// The specifics of the error's origin.
        /// </summary>
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class. 
        /// </summary>
        /// <param name="severity">
        /// Severity level associated with logging message. 
        /// </param>
        public LogMessage(TraceEventType severity)
        {
            this.SeverityType = severity;
        }
    }
}
