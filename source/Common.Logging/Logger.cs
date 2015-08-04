namespace Ignite.Infrastructure.Micro.Common.Logging
{
    using Ignite.Infrastructure.Micro.Common.Errors;
    using Ignite.Infrastructure.Micro.Common.Exceptions;

    using NetMf.CommonExtensions;

    /// <summary>
    /// Logging provider.
    /// </summary>
    /// <remarks>
    /// A bridge pattern that separates the implementation from the interface definition for a logging provider.
    /// </remarks>
    public class Logger : ILogger
    {
        private readonly ILogProvider m_Implementation;
        private readonly LogHelper m_Helper;

        /// <summary>
        /// See <see cref="ILogProvider.IsDebugEnabled"/> for more details.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return m_Implementation.IsDebugEnabled; }
        }

        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return m_Implementation.IsInfoEnabled; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class. 
        /// </summary>
        /// <param name="logProvider">
        /// The logging provider.
        /// </param>
        /// <param name="helper">
        /// Helper class used to create detailed, formatted log messages.
        /// </param>
        public Logger(ILogProvider logProvider, LogHelper helper)
        {
            //logProvider.ShouldNotBeNull();
            //helper.ShouldNotBeNull();

            m_Implementation = logProvider;
            m_Helper = helper;
        }

        /// <summary>
        /// See <see cref="ILogger.Debug"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Debug(string message)
        {
            var logMessageEntry = m_Helper.DefineNoErrorLogMessage(message, ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Debug"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        public void Debug(string message, params object[] formatting)
        {
            var logMessageEntry = m_Helper.DefineNoErrorLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }       

        /// <summary>
        /// See <see cref="ILogger.Error"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Error(string message)
        {
            var logMessageEntry = m_Helper.DefineErrorLogMessage(message, ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Error"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        /// <param name="ex">
        /// An exception associated with the recorded error.
        /// </param>
        public void Error(string message, BaseException ex)
        {
            var logMessageEntry = m_Helper.DefineErrorLogMessage(message, ErrorCategory.None, ErrorType.None, ex);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Error"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        public void Error(string message, params object[] formatting)
        {
            var logMessageEntry = m_Helper.DefineErrorLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Info"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Info(string message)
        {
            var logMessageEntry = m_Helper.DefineInformationLogMessage(message, ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Info"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        public void Info(string message, params object[] formatting)
        {
            var logMessageEntry = m_Helper.DefineInformationLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Fatal"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Fatal(string message)
        {
            var logMessageEntry = m_Helper.DefineFatalLogMessage(message, ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.Fatal"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        /// <param name="ex">
        /// The exception associated with the fatal message.
        /// </param>
        public void Fatal(string message, BaseException ex)
        {
            var logMessageEntry = m_Helper.DefineFatalLogMessage(message, ErrorCategory.None, ErrorType.None, ex);
            this.LogMessage(logMessageEntry);

        }

        /// <summary>
        /// See <see cref="ILogger.Fatal"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        public void Fatal(string message, params object[] formatting)
        {
            var logMessageEntry = m_Helper.DefineFatalLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
            this.LogMessage(logMessageEntry);
        }

        /// <summary>
        /// See <see cref="ILogger.LogMessage"/> for more details.
        /// </summary>
        /// <param name="logMessage">
        /// The details of the logging entry to log.
        /// </param>
        /// <returns>
        /// The unique identifier of the log entry created.
        /// </returns>
        public string LogMessage(LogMessage logMessage)
        {
            var logEntry = m_Helper.BuildLogEntry(logMessage);

            m_Implementation.Log(logEntry);

            return logEntry.LogEntryId;
        }
      
    }
}
