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

namespace Ignite.Framework.Micro.Common.Exceptions
{
    using System;

    /// <summary>
    /// Extension methods for converting standard <see cref="Exception"/>'s to framework specific types.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Creates a new <see cref="PropogatedException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static PropogatedException CreatePropogatedException(this Exception e, string message)
        {
            return new PropogatedException(message, e);
        }

        /// <summary>
        /// Creates a new <see cref="PropogatedException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static InterceptedException CreateInterceptedException(this Exception e, string message)
        {
            return new InterceptedException(message, e);
        }

        /// <summary>
        /// Creates a new <see cref="ValidationException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static ValidationException CreateValidationException(this Exception e, string message)
        {
            return new ValidationException(message, e);
        }

        /// <summary>
        /// Creates a new <see cref="ApplicationException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static ApplicationException CreateApplicationException(this Exception e, string message)
        {
            return new ApplicationException(message, e);
        }
    }
}