namespace Ignite.Infrastructure.Micro.Common.Errors
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
