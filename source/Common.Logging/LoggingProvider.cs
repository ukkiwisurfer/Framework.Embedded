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

    /// <summary>
    /// Provides the support to persist logging messages to a queued logging provider.
    /// </summary>
    public class LoggingProvider : ILogProvider
    {
        private readonly LogContainer m_LogContainer;
        private readonly string m_LoggerName;

        /// <summary>
        /// Determines if any logging is to be performed against the <see cref="LogContainer"/> instance.
        /// </summary>
        public bool IsLoggingEnabled { get; set; }

        /// <summary>
        /// Initialises ans instance of the <see cref="LoggingProvider"/> class.
        /// </summary>
        /// <param name="loggingContainer">
        /// Container that provider instances write log messages to.
        /// </param>
        public LoggingProvider(string loggerName, LogContainer loggingContainer)
        {
            loggerName.ShouldNotBeNull();
            loggingContainer.ShouldNotBeNull();

            m_LoggerName = loggerName;
            m_LogContainer = loggingContainer;

            IsLoggingEnabled = true;
        }

        /// <summary>
        /// See <see cref="ILogProvider.IsDebugEnabled"/> for more details.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// See <see cref="ILogProvider.IsInfoEnabled"/> for more details.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// See <see cref="ILogProvider.IsErrorEnabled"/> for more details.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// See <see cref="ILogProvider.IsFatalEnabled"/> for more details.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// See <see cref="ILogProvider.Log"/> for more details.
        /// </summary>
        /// <param name="entry">
        /// The logging entry to add.
        /// </param>
        public void Log(LogEntry entry)
        {
            entry.ShouldNotBeNull();

            entry.SetLoggerName(m_LoggerName);
            if (IsLoggingEnabled)
            {
                m_LogContainer.AddLogEntry(entry);
            }
        }
    }
}
