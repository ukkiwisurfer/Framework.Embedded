namespace Ignite.Framework.Micro.Common.Assertions
{
    using System;

    /// <summary>
    /// Extension methods for assertions.
    /// </summary>
    public static class TypeAssertions
    {
        /// <summary>
        /// Verifies that an object is of a given type.
        /// </summary>
        /// <param name="type">
        /// The type being verified.
        /// </param>
        /// <param name="item">
        /// The item being checked.
        /// </param>
        public static void ShouldBeOfType(this object item, Type type)
        {
            if (item.GetType() != type) throw new ArgumentException("Type of item does not match expected type."); 
        }

        /// <summary>
        /// Verifies that an object is of a given type.
        /// </summary>
        /// <param name="type">
        /// The type being verified.
        /// </param>
        /// <param name="item">
        /// The item being checked.
        /// </param>
        /// <param name="errorMessage">
        /// The error message to associate with the assertion, should it fail.
        /// </param>
        public static void ShouldBeOfType(this object item, Type type, string errorMessage)
        {
            if (item.GetType() != type) throw new ArgumentException(errorMessage); 
        }

        /// <summary>
        /// Verifies that an exception is of a specified type.
        /// </summary>
        /// <param name="type">
        /// The type to verify.
        /// </param>
        /// <param name="item">
        /// The exception being checked.
        /// </param>
        public static void OfType(this Exception item, Type type)
        {
            ShouldBeOfType(item, type);
        }
    }
}
