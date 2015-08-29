namespace Ignite.Framework.Micro.Common.Errors
{
    /// <summary>
    /// Repesents an error that occured within the context of a larger group of operations or activities.
    /// </summary>
    public class InnerError
    {
        /// <summary>
        /// The severity level associated with the error.
        /// </summary>
        public Severity SeverityLevel { get; set; }

        /// <summary>
        /// The error category associated with the error. 
        /// </summary>
        public ErrorCategory Category { get; set; }

        /// <summary>
        /// The type of error.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The message associated with the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The unique identifier associated with the error.
        /// </summary>
        public string ErrorIdentifier { get; set; }

        /// <summary>
        /// Determines whether this inner error contains any sensitive information that shouldn't be exposed to any consumers 
        /// apart from operational staff / IS Support for security reasons.
        /// </summary>
        /// <remarks>
        /// Example of an error with sensitive information is a database exception. The inner error with database exception can be 
        /// logged but for security reasons should not be sent over the network.
        /// </remarks>
        public bool ContainsSensitiveInformation { get; set; }

        /// <summary>
        /// Indicates whether the business logic that generated this inner error identified that this inner error is likely to be 
        /// a temporary failure that won't happen again if the action is re-executed.
        /// </summary>
        public bool TemporaryFailure { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerError"/> class. 
        /// </summary>
        public InnerError()
        {
            
        }

        /// <summary>
        /// Creates an instance of an inner error. 
        /// </summary>
        /// <param name="generalMessage">
        /// The general error message to associate with the error.
        /// </param>
        /// <param name="severity">
        /// Error severity to associate with the error.
        /// </param>
        /// <param name="category">
        /// Error category to associate with the error.
        /// </param>
        /// <param name="errorType">
        /// Error type to associate with the error.
        /// </param>
        /// <returns>
        /// A newly initialised inner error.
        /// </returns>
        public static InnerError CreateError(string generalMessage, Severity severity, ErrorCategory category, ErrorType errorType)
        {
            var error = new InnerError();
            error.ErrorMessage = generalMessage;
            error.Category = ErrorCategory.Application;
            error.ErrorType = ErrorType.Validation;
            error.SeverityLevel = Severity.Error;

            return error;
        }

        /// <summary>
        /// Creates an instance of an inner error. 
        /// </summary>
        /// <param name="generalMessage">
        /// The general error message to associate with the error.
        /// </param>
        /// <param name="severity">
        /// Error severity to associate with the error.
        /// </param>
        /// <param name="category">
        /// Error category to associate with the error.
        /// </param>
        /// <param name="errorType">
        /// Error type to associate with the error.
        /// </param>
        /// <param name="errorIdentifier">
        /// Specific error identifier to associate with the error.
        /// </param>
        /// <returns>
        ///  A newly initialised inner error.
        /// </returns>
        public static InnerError CreateError(string generalMessage, Severity severity, ErrorCategory category, ErrorType errorType, string errorIdentifier)
        {
            var error = new InnerError();
            error.ErrorMessage = generalMessage;
            error.Category = ErrorCategory.Application;
            error.ErrorType = ErrorType.Validation;
            error.SeverityLevel = Severity.Error;
            error.ErrorIdentifier = errorIdentifier;

            return error;
        }
    }
}