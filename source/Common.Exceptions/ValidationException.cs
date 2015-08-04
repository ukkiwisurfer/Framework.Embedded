
namespace Ignite.Infrastructure.Micro.Common.Exceptions
{
    using System;

    using Ignite.Infrastructure.Micro.Common.Errors;

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
