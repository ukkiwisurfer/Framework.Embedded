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

using System;

namespace Ignite.Framework.Micro.Common.Contract.Logging
{
    using Ignite.Framework.Micro.Common.Exceptions;

    /// <summary>
    /// Definition of a logging provider.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        void Debug(string message);

        /// <summary>
        /// Logs a debug message with formatting.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        void Debug(string message, params object[] formatting);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        void Error(string message);

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        /// <param name="ex">
        /// An exception associated with the recorded error.
        /// </param>
        void Error(string message, BaseException ex);

        /// <summary>
        /// Logs an error message with formatting.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        void Error(string message, params object[] formatting);

        /// <summary>
        /// Logs an error message with formatting.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        /// <param name="ex"></param>
        void Error(string message, BaseException ex, params object[] formatting);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        void Info(string message);

        /// <summary>
        /// Logs an information message with formatting.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        void Info(string message, params object[] formatting);

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        void Fatal(string message);

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">
        /// The message to output.
        /// </param>
        /// <param name="ex">
        /// The exception associated with the fatal message.
        /// </param>
        void Fatal(string message, BaseException ex);

        /// <summary>
        /// Logs an fatal message with formatting.
        /// </summary>
        /// <param name="message">
        /// The message to output including formatting placeholders.
        /// </param>
        /// <param name="formatting">
        /// Values to substitute for the formatting placeholders.
        /// </param>
        void Fatal(string message, params object[] formatting);

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="logMessage">
        /// Definition of the information to be logged.
        /// </param>
        /// <returns>
        /// The unique log identifier for the message.
        /// </returns>
        string LogMessage(LogMessage logMessage);
    }
}
