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

namespace Ignite.Framework.Micro.Common.Errors
{
    using System;

    /// <summary>
    /// Attribute to associate an error severity, category and error type to  
    /// </summary>    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ErrorCodeAttribute : Attribute
    {
        private readonly ErrorCode m_ErrorCode;

        /// <summary>
        /// The severity of the error to associate with this validation.
        /// </summary>
        public Severity Severity
        {
            get { return this.m_ErrorCode.Severity; }
        }

        /// <summary>
        /// The category of error to associate with this validation.
        /// </summary>
        public ErrorCategory ErrorCategory
        {
            get { return this.m_ErrorCode.Category; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeAttribute"/> class. 
        /// </summary>
        /// <param name="severity">
        /// The severity of the error to associate with this validation.
        /// </param>
        /// <param name="category">
        /// The category of the error to associate with this validation.
        /// </param>
        public ErrorCodeAttribute(Severity severity, ErrorCategory category)
        {
            this.m_ErrorCode = new ErrorCode(severity, category, ErrorType.Validation);
        }


    }
}
