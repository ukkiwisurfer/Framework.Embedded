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
    /// Helper for logging errors.
    /// </summary>
    public interface ILogHelper
    {
        /// <summary>
        /// Creates an Fatal log message.
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
        LogMessage DefineFatalLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null);

        /// <summary>
        /// Creates an Information log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        LogMessage DefineInformationLogMessage(string message);

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
        LogMessage DefineInformationLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null);

        /// <summary>
        /// Creates an Debug log message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <returns>
        /// A new, initalised <see cref="LogMessage"/> instance.
        /// </returns>
        LogMessage DefineDebugLogMessage(string message);

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
        LogMessage DefineWarningLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null);

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
        LogMessage DefineErrorLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null);

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
        LogMessage DefineNoErrorLogMessage(string message, ErrorCategory errorCategory, ErrorType errorType, BaseException exception = null);
    }
}