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

using System;
using System.Text;
using Microsoft.SPOT;

namespace Ignite.Framework.Micro.Common.Logging
{
    using Ignite.Framework.Micro.Common.Contract.Logging;

    /// <summary>
    /// A log provider that does nothing.
    /// </summary>
    public class ConsoleLogger : ILogProvider
    {
        private readonly string m_LoggerName;

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
        /// Initialises an instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="loggerName"></param>
        public ConsoleLogger(string loggerName)
        {
            m_LoggerName = loggerName;
        }

        /// <summary>
        /// See <see cref="ILogProvider.Log"/> for more details.
        /// </summary>
        /// <param name="entry">
        /// The log entry to process.
        /// </param>
        public void Log(LogEntry entry)
        {
            var timestamp = entry.TimeStamp;

            var builder = new StringBuilder(timestamp.ToString("yyyy-MM-dd HH:mm:ss:"));
            builder.Append(timestamp.Millisecond.ToString("D3"));
            builder.Append("  ");
            builder.Append(entry.Win32ThreadId);
            builder.Append("  ");
            builder.Append(m_LoggerName);
            builder.Append("  ");
            builder.Append(entry.Message);

            Debug.Print(builder.ToString());
        }
    }
}
