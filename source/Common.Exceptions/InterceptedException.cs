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

namespace Ignite.Framework.Micro.Common.Exceptions
{
    using System;
    using Ignite.Framework.Micro.Common.Errors;

    /// <summary>
    /// The exception thrown by the Interceptors when they have caught an exception previously.
    /// </summary>
    public class InterceptedException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptedException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The message to associate with this exception.
        /// </param>
        /// <param name="innerException">
        /// The inner exception details.
        /// </param>
        /// <param name="errorCode">
        /// The error code for the exception.
        /// </param>
        public InterceptedException(string message, Exception innerException, ErrorCode errorCode = null) : base(message, innerException, errorCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptedException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The message to associate with this exception.
        /// </param>
        /// <param name="errorCode">
        /// The error code for the exception.
        /// </param>
        public InterceptedException(string message, ErrorCode errorCode = null) : base(message, errorCode)
        {
        }
    }
}
