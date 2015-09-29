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

namespace Ignite.Framework.Micro.Common.Core
{
    using System;

    /// <summary>
    /// The exception that is thrown when the format of an argument does not meet the parameter specifications of the invoked method.
    /// </summary>
    public class FormatException : Exception
    {
        internal static string ERROR_MESSAGE = "Format string is not valid.";

        /// <summary>
        /// Initializes a new instance of the FormatException class.
        /// </summary>
        public FormatException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FormatException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public FormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FormatException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="ex">The exception that is the cause of the current exception. If the innerException parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception. </param>
        public FormatException(string message, Exception ex) : base(message, ex)
        {
        }

    }
}
