
namespace Ignite.Infrastructure.Micro.Common.Logging
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Threading;

    using Ignite.Infrastructure.Micro.Common.Errors;
    using Ignite.Infrastructure.Micro.Common.Exceptions;

    /// <summary>
    /// Helper for building log messages.
    /// </summary>
    public class LogHelper : ILogHelper
    {
        private const string CnNewLine = "\r\n";

        /// <summary>
        /// The name of the machine that the .NET micro application is running on.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// The process identifier that hosts the .NET micro application.
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// See <see cref="ILogHelper.DefineFatalLogMessage"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <param name="errorCategory">
        /// The specifics of the error origin.
        /// </param>
        /// <param name="errorType">
        /// The type of error being logged.
        /// </param>
        /// <param name="exception">
        /// The exception (if one exists) associated with the error.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineFatalLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null)
        {
            var logMessage = new LogMessage(TraceEventType.Critical);

            logMessage.Exception = exception;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.Critical, errorCategory, errorType);

            return logMessage;
        }

        /// <summary>
        /// Creates an Information log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineInformationLogMessage(string message)
        {
            var logMessage = new LogMessage(TraceEventType.Information);

            logMessage.Exception = null;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.Information, ErrorCategory.None, ErrorType.None);

            return logMessage;
        }
        
        /// <summary>
        /// Creates an Information log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <param name="errorCategory">
        /// The specifics of the error origin.
        /// </param>
        /// <param name="errorType">
        /// The type of error being logged.
        /// </param>
        /// <param name="exception">
        /// The exception (if one exists) associated with the error.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineInformationLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null)
        {
            var logMessage = new LogMessage(TraceEventType.Information);

            logMessage.Exception = exception;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.Information, errorCategory, errorType);

            return logMessage;
        }

        /// <summary>
        /// Creates an Debug log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineDebugLogMessage(string message)
        {
            var logMessage = new LogMessage(TraceEventType.Verbose);

            logMessage.Exception = null;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.Information, ErrorCategory.None, ErrorType.None);

            return logMessage;
        }

        /// <summary>
        /// Creates a Warning log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <param name="errorCategory">
        /// The specifics of the error origin.
        /// </param>
        /// <param name="errorType">
        /// The type of error being logged.
        /// </param>
        /// <param name="exception">
        /// The exception (if one exists) associated with the error.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineWarningLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null)
        {
            var logMessage = new LogMessage(TraceEventType.Warning);

            logMessage.Exception = exception;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.Warning, errorCategory, errorType);

            return logMessage;
        }

        /// <summary>
        /// Creates an Error log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <param name="errorCategory">
        /// The specifics of the error origin.
        /// </param>
        /// <param name="errorType">
        /// The type of error being logged.
        /// </param>
        /// <param name="exception">
        /// The exception (if one exists) associated with the error.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineErrorLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null)
        {
            var logMessage = new LogMessage(TraceEventType.Error);

            logMessage.Exception = exception;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.Error, errorCategory, errorType);

            return logMessage;
        }

        /// <summary>
        /// Creates an NoError (severity-wise) log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <param name="errorCategory">
        /// The specifics of the error origin.
        /// </param>
        /// <param name="errorType">
        /// The type of  being logged.
        /// </param>
        /// <param name="exception">
        /// The exception (if one exists) associated with the message.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        public LogMessage DefineNoErrorLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null)
        {
            var logMessage = new LogMessage(TraceEventType.Verbose);

            logMessage.Exception = exception;
            logMessage.Message = message;
            logMessage.ErrorCode = new ErrorCode(Severity.NoError, errorCategory, errorType);

            return logMessage;
        }

        /// <summary>
        /// Builds a log entry.
        /// </summary>
        /// <param name="logMessage">
        /// The details of the log entry to be logged.
        /// </param>
        /// <returns>
        /// A <see cref="LogEntry"/> containing the unique identifier of the log entry, and the log entry itself.
        /// </returns>
        internal LogEntry BuildLogEntry(LogMessage logMessage)
        {
            string logEntryId = BuildNextLogNumber();

            var logEntry = new LogEntry(logEntryId);
            logEntry.Message = CreateLogMessage(logMessage.Message, logMessage.Exception);
            logEntry.EventId = 0;
            logEntry.Priority = 1;
            logEntry.TimeStamp = DateTime.UtcNow;
            logEntry.MachineName = MachineName;
            logEntry.Win32ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();

            if (logMessage.Exception != null)
            {
                logEntry.ExtendedProperties["errorCategory"] = logMessage.Exception.ErrorCode.Category;
                logEntry.ExtendedProperties["errorSeverity"] = logMessage.Exception.ErrorCode.Severity;
                logEntry.ExtendedProperties["errorType"] = logMessage.Exception.ErrorCode.ErrorType;
            }
            else
            {
                logEntry.ExtendedProperties["errorCategory"] = logMessage.ErrorCode.Category;
                logEntry.ExtendedProperties["errorSeverity"] = logMessage.ErrorCode.Severity;
                logEntry.ExtendedProperties["errorType"] = logMessage.ErrorCode.ErrorType;
            }

            return logEntry;
        }
 
        /// <summary>
        /// Recursively walks the exception stack until we hit bottom.
        /// </summary>
        /// <param name="exception">
        /// The exception to log.
        /// </param>
        /// <param name="padding">
        /// Depth in the tree.
        /// </param>
        /// <returns>
        /// A string containing the exception message and innerexception up the stack.
        /// </returns>
        private static string BuildExceptionEntryText(Exception exception, int padding)
        {
            var paddedString = new string(' ', padding);

            var exceptionText = new StringBuilder(paddedString);
            exceptionText.Append(exception.Message);
            exceptionText.Append(CnNewLine);
            exceptionText.Append(paddedString);
            exceptionText.Append(exception.StackTrace);
            exceptionText.Append(CnNewLine);

            if (exception.InnerException != null)
            {
                exceptionText.Append(paddedString);
                exceptionText.Append("Inner Exception: ");
                exceptionText.Append(CnNewLine);
                exceptionText.Append(BuildExceptionEntryText(exception.InnerException, padding + 2));
            }

            return exceptionText.ToString();
        }

        /// <summary>
        /// Creates a formatted message for logging.
        /// </summary>
        /// <param name="message">
        /// The error message to log.
        /// </param>
        /// <param name="exception">
        /// Exception to be logged.
        /// </param>
        /// <returns>
        /// The error message
        /// </returns>
        private static string CreateLogMessage(string message, BaseException exception = null)
        {
            var logMessage = new StringBuilder(message);
            if (exception != null)
            {
                logMessage.Append(CnNewLine);
                logMessage.Append("Exception Details:");
                logMessage.Append(CnNewLine);
                logMessage.Append(BuildExceptionEntryText(exception, 0));
            }
            return logMessage.ToString();
        }

        /// <summary>
        /// Builds a unique log number based on a Guid, that is optimised for indexing.
        /// </summary>
        /// <returns>
        /// A unique identifier.
        /// </returns>
        private static string BuildNextLogNumber()
        {
            byte[] guidBytes = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.UtcNow;

            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var milliseconds = now.TimeOfDay.Ticks / 1000.0;

            byte[] daysBytes = BitConverter.GetBytes(days.Days);
            byte[] millisecondsBytes = BitConverter.GetBytes((long)(milliseconds / 3.333333));

            daysBytes = Reverse(daysBytes);
            Reverse(millisecondsBytes);

            Array.Copy(daysBytes, daysBytes.Length - 2, guidBytes, guidBytes.Length - 6, 2);
            Array.Copy(millisecondsBytes, millisecondsBytes.Length - 4, guidBytes, guidBytes.Length - 4, 4);

            return new Guid(guidBytes).ToString();
        }

        /// <summary>
        /// Reverses the byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static byte[] Reverse(byte[] array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                byte bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }

            return array;
        }

    }
}
