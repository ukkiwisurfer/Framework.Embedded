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

namespace Ignite.Framework.Micro.Common.Logging
{
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Core;
    using Ignite.Framework.Micro.Common.Errors;
    using Ignite.Framework.Micro.Common.Exceptions;

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
        /// Returns the status of whether error level of logging is enabled.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return m_Implementation.IsErrorEnabled; }
        }

        /// <summary>
        /// Returns the status of whether fatal level of logging is enabled.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return m_Implementation.IsFatalEnabled; }
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
            logProvider.ShouldNotBeNull();
            helper.ShouldNotBeNull();

            m_Implementation = logProvider;
            m_Helper = helper;
        }

        /// <summary>
        /// See <see cref="Microsoft.SPOT.Debug"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                var logMessageEntry = m_Helper.DefineNoErrorLogMessage(message, ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
        }

        /// <summary>
        /// See <see cref="Microsoft.SPOT.Debug"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        public void Debug(string message, params object[] formatting)
        {
            if (IsDebugEnabled)
            { 
                var logMessageEntry = m_Helper.DefineNoErrorLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
        }       

        /// <summary>
        /// See <see cref="Errors.Error"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                var logMessageEntry = m_Helper.DefineErrorLogMessage(message, ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
        }

        /// <summary>
        /// See <see cref="Errors.Error"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        /// <param name="ex">
        /// An exception associated with the recorded error.
        /// </param>
        public void Error(string message, BaseException ex)
        {
            if (IsErrorEnabled)
            {
                var logMessageEntry = m_Helper.DefineErrorLogMessage(message, ErrorCategory.None, ErrorType.None, ex);
                this.LogMessage(logMessageEntry);
            }
        }

        /// <summary>
        /// See <see cref="Errors.Error"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        public void Error(string message, params object[] formatting)
        {
            if (IsErrorEnabled)
            {
                var logMessageEntry = m_Helper.DefineErrorLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
        }

        /// <summary>
        /// See <see cref="ILogger.Info"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                var logMessageEntry = m_Helper.DefineInformationLogMessage(message, ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
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
            if (IsInfoEnabled)
            {
                var logMessageEntry = m_Helper.DefineInformationLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
        }

        /// <summary>
        /// See <see cref="ILogger.Fatal"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                var logMessageEntry = m_Helper.DefineFatalLogMessage(message, ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
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
            if (IsFatalEnabled)
            {
                var logMessageEntry = m_Helper.DefineFatalLogMessage(message, ErrorCategory.None, ErrorType.None, ex);
                this.LogMessage(logMessageEntry);
            }

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
            if (IsFatalEnabled)
            {
                var logMessageEntry = m_Helper.DefineFatalLogMessage(StringUtility.Format(message, formatting), ErrorCategory.None, ErrorType.None);
                this.LogMessage(logMessageEntry);
            }
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

            if (m_Implementation != null)
            {
                m_Implementation.Log(logEntry);
            }

            return logEntry.LogEntryId;
        }
      
    }
}
