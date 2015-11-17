﻿//--------------------------------------------------------------------------- 
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
    /// <summary>
    /// Common logging provider.
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        /// <summary>
        /// Controls whether logging 
        /// </summary>
        bool IsLoggingEnabled { get; set; }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="entry">
        /// Definition of the information to be logged.
        /// </param>
        void Log(LogEntry entry);
    }
}
