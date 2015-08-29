
namespace Ignite.Framework.Micro.Common.Exceptions
{
    using System;
    using Ignite.Framework.Micro.Common.Errors;

    /// <summary>
    /// An exception to propogate across a service boundary.
    /// </summary>
    public class PropogatedException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropogatedException"/> class. 
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
        public PropogatedException(string message, Exception innerException, ErrorCode errorCode = null) : base(message, innerException, errorCode)
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
        public PropogatedException(string message, ErrorCode errorCode = null) : base(message, errorCode)
        {
        }
    }
}
