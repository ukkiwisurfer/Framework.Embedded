namespace Ignite.Framework.Micro.Common.Errors
{
    using System.Collections;
    using VikingErik.NetMF.MicroLinq;

    /// <summary>
    /// An error definition.
    /// </summary>
    public class Error
    {
        private IList m_InnerErrors;

        /// <summary>
        /// The severity of the error
        /// </summary>
        public Severity SeverityLevel { get; set; }

        /// <summary>
        /// The category of the error
        /// </summary>
        public ErrorCategory Category { get; set; }

        /// <summary>
        /// The type of the error
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The message to associate with the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Indicates whether the business logic that generated this error identified that this error is likely to be a temporary failure that won't happen again if the action is re-executed.
        /// </summary>
        public bool TemporaryFailure { get; set; }

        /// <summary>
        /// A collection of inner errors
        /// </summary>
        /// <remarks>
        /// Added a setter to enable XmlSerializer to serialize the property. This
        /// is a known limitation of the XmlSerializer class; Read-only properties
        /// are not serializable.
        /// </remarks>
        public IList InnerErrors
        {
            get { return m_InnerErrors; }
            set { m_InnerErrors = value; }
        }

        /// <summary>
        /// Creates and initialises the error.
        /// </summary>
        public Error()
        {
            m_InnerErrors = new ArrayList();
        }

        /// <summary>
        /// The error code associated with the error.
        /// </summary>
        public string ErrorCode
        {
            get
            {
                var code = new ErrorCode(SeverityLevel, Category, ErrorType);
                return code.Code;
            }
        }

        /// <summary>
        /// Creates an instance of an error. 
        /// </summary>
        /// <remarks>
        /// Does not create any inner error objects.
        /// </remarks>
        /// <param name="generalMessage">
        /// The general error message to associate with the error.
        /// </param>
        /// <param name="severity">
        /// The severity of the error.
        /// </param>
        /// <param name="category">
        /// The category of the error.
        /// </param>
        /// <param name="errorType">
        /// The type of the error.
        /// </param>
        /// <returns>
        /// A new instance of the error.
        /// </returns>
        public static Error CreateError(string generalMessage, Severity severity, ErrorCategory category, ErrorType errorType)
        {
            var error = new Error();
            error.ErrorMessage = generalMessage;
            error.Category = category;
            error.ErrorType = errorType;
            error.SeverityLevel = severity;          

            return error;
        }

        /// <summary>
        /// Creates an instance of an error. 
        /// </summary>
        /// <remarks>
        /// Creates an inner error object.
        /// </remarks>
        /// <param name="generalMessage">
        /// The general error message to associate with the error.
        /// </param>
        /// <param name="innerMessage">
        /// Describes the inner error message to assoxiate with the inner error.
        /// </param>
        /// <param name="severity">
        /// The severity of the error.
        /// </param>
        /// <param name="category">
        /// The category of the error.
        /// </param>
        /// <param name="errorType">
        /// The type of the error.
        /// </param>
        /// <returns>
        /// A new instance of the error.
        /// </returns>
        public static Error CreateError(string generalMessage, string innerMessage, Severity severity, ErrorCategory category, ErrorType errorType)
        {
            var error = new Error();
            error.ErrorMessage = generalMessage;
            error.Category = ErrorCategory.Application;
            error.ErrorType = ErrorType.Validation;
            error.SeverityLevel = Severity.Error;

            error.InnerErrors.Add(InnerError.CreateError(innerMessage, severity, category, errorType));

            return error;
        }    

        /// <summary>
        /// Creates an application validation error object.
        /// </summary>
        /// <param name="generalMessage">
        /// The general error message to associated with the error.
        /// </param>
        /// <returns>
        /// A new instance of the error.
        /// </returns>
        public static Error CreateApplicationValidationError(string generalMessage)
        {
            return CreateError(generalMessage, Severity.Error, ErrorCategory.Application, ErrorType.Validation);
        }

        /// <summary>
        /// Removes any inner errors that contain sensitive information that may introduce a security risk and should not be sent over the network.
        /// The intention is that various parts of the system should call this method before they convert this object to a DTO to send it over the network.
        /// </summary>
        public void RemoveInnerErrorsWithSensitiveInformation()
        {
            foreach (InnerError innerError in m_InnerErrors)
            {
                if (innerError.ContainsSensitiveInformation) { m_InnerErrors.Remove(innerError); }
            }
        }

        /// <summary>
        /// Detects if the InnerErrors property contains any entries of type Error (i.e. with severity level of Error or Critical).
        /// </summary>
        /// <returns>
        /// True if the error container contains any errors.
        /// </returns>
        public bool HasErrors()
        {
            bool result = InnerErrors.Any(o => ((InnerError) o).SeverityLevel == Severity.Error || ((InnerError) o).SeverityLevel == Severity.Critical);
            return result;
        }

        /// <summary>
        /// Detects if the InnerErrors property contains any entries of type Warning (i.e. with severity level of Warning).
        /// </summary>
        /// <returns>
        /// True if the error container contains any warnings.
        /// </returns>
        public bool HasWarnings()
        {
            bool result = InnerErrors.Any(o => ((InnerError)o).SeverityLevel == Severity.Warning);
            return result;
        }

        /// <summary>
        /// Retrieve the base properties of this Error type as a series of name value pairs. This allows us to 
        /// override this method in child types so the base Error can have additional properties without 
        /// needing to change the fundamental behaviour of the Serialize method.
        /// </summary>
        /// <returns>A dictionary containing the base pairs for this Error</returns>
        protected virtual IDictionary GetBasePropertiesForSerialisation()
        {
            var properties = new Hashtable()
            {
                { "SeverityLevel", this.SeverityLevel.ToString() },
                { "Category", this.Category.ToString() },
                { "ErrorType", this.ErrorType.ToString() },
                { "ErrorMessage", ErrorMessage }
            };
            
            return properties;
        }
    }
}