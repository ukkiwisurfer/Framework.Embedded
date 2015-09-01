namespace Ignite.Framework.Micro.Common.Exceptions
{
    using System;

    public static class ExceptionExtensions
    {
        /// <summary>
        /// Creates a new <see cref="PropogatedException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static PropogatedException CreatePropogatedException(this Exception e, string message)
        {
            return new PropogatedException(message, e);
        }

        /// <summary>
        /// Creates a new <see cref="PropogatedException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static InterceptedException CreateInterceptedException(this Exception e, string message)
        {
            return new InterceptedException(message, e);
        }

        /// <summary>
        /// Creates a new <see cref="ValidationException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static ValidationException CreateValidationException(this Exception e, string message)
        {
            return new ValidationException(message, e);
        }

        /// <summary>
        /// Creates a new <see cref="ApplicationException"/> from an existing <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        /// The message to associate with the exception.
        /// </param>
        /// <param name="e">
        /// The general exception that is being wrapped.
        /// </param>
        /// <returns>
        /// A new instance of a <see cref="BaseException"/> class.
        /// </returns>
        public static ApplicationException CreateApplicationException(this Exception e, string message)
        {
            return new ApplicationException(message, e);
        }
    }
}