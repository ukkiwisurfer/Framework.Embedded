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
    using Ignite.Framework.Micro.Common.Errors;
    using Ignite.Framework.Micro.Common.Exceptions;

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
