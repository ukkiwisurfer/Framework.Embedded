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
    using Ignite.Framework.Micro.Common.Errors;

    /// <summary>
    /// The parent class for all Validation Exception types. 
    /// </summary>
    /// <remarks>
    /// The difference between a ValidationException and a BaseException is that the former is not 
    /// something we want to wrap at the service boundary.
    /// </remarks>
    public class ValidationException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="innerException">
        /// The inner exception details.
        /// </param>
        /// <param name="errorCode">
        /// The error code for the exception. If not provided defaults to Warning/Application/Validation
        /// </param>
        public ValidationException(string message, Exception innerException, ErrorCode errorCode = null) : base(message, innerException, errorCode)
        {
            if (errorCode == null)
            {
                this.ErrorCode = GetDefaultErrorCode();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="errorCode">
        /// The error code for the exception. If not provided defaults to Warning/Application/Validation
        /// </param>
        public ValidationException(string message, ErrorCode errorCode = null) : base(message, errorCode)
        {
            if (errorCode == null)
            {
                this.ErrorCode = GetDefaultErrorCode();
            }
        }

        /// <summary>
        /// Retrieve the default <seealso cref="ErrorCode">ErrorCode</seealso> for a BaseException.
        /// </summary>
        /// <returns>
        /// The default error code to use.
        /// </returns>
        private static ErrorCode GetDefaultErrorCode()
        {
            return new ErrorCode(Severity.Warning, ErrorCategory.Application, ErrorType.Validation);
        }
    }
}
