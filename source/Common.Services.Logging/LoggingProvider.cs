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

namespace Ignite.Framework.Micro.Common.Services.Logging
{
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;

    /// <summary>
    /// Provides the support to persist logging messages to a queued logging provider.
    /// </summary>
    public class LoggingProvider : ILogProvider
    {
        private readonly BufferedLoggingService m_LoggingService;

        /// <summary>
        /// Initialises ans instance of the <see cref="LoggingProvider"/> class.
        /// </summary>
        /// <param name="loggingService">
        /// Service that supports the queuing and persisting of log messages.
        /// </param>
        public LoggingProvider(BufferedLoggingService loggingService)
        {
            loggingService.ShouldNotBeNull();

            this.m_LoggingService = loggingService;
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
        /// See <see cref="ILogProvider.Log"/> for more details.
        /// </summary>
        /// <param name="entry">
        /// The logging entry to add.
        /// </param>
        public void Log(LogEntry entry)
        {
            entry.ShouldNotBeNull();

 	        this.m_LoggingService.AddLogEntry(entry);
        }
    }
}
