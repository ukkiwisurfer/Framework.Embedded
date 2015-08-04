
namespace Ignite.Infrastructure.Micro.Common.Exceptions
{
    using System;

    using Ignite.Infrastructure.Micro.Common.Errors;

    /// <summary>
    /// The parent class for all exceptions raised within the system. If a BaseException is raised we probably want to wrap it at the 
    /// service boundary and pass on an error message.
    /// </summary>
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The Exception message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception raised by this baseException.
        /// </param>
        /// <param name="errorCode">
        /// The Error Code for this exception.
        /// </param>
        protected BaseException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException)
        {
            this.ErrorCode = errorCode;

            if (errorCode == null)
            {
                this.ErrorCode = GetDefaultErrorCode();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="errorCode">
        /// The error code for the exception class.
        /// </param>
        protected BaseException(string message, ErrorCode errorCode) : base(message)
        {
            this.ErrorCode = errorCode;

            if (errorCode == null)
            {
                this.ErrorCode = GetDefaultErrorCode();
            }
        }

        /// <summary>
        /// The <seealso cref="Micro.Common.Errors.ErrorCode">ErrorCode</seealso> for this message.
        /// </summary>
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Retrieve the default <seealso cref="ErrorCode">ErrorCode</seealso> for a BaseException.
        /// </summary>
        /// <returns>
        /// The default error code.
        /// </returns>
        private static ErrorCode GetDefaultErrorCode()
        {
            return new ErrorCode(Severity.Error, ErrorCategory.Application, ErrorType.Operation);
        }
    }
}
