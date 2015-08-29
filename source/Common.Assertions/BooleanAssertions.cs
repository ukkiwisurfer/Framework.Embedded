namespace Ignite.Framework.Micro.Common.Assertions
{
    using System;

    /// <summary>
    /// Extension methods for assertions.
    /// </summary>
    public static class BooleanAssertions
    {
        /// <summary>
        /// Asserts that the boolean evaluates as true.
        /// </summary>
        /// <param name="item">
        /// The boolean value been checked.
        /// </param>
        public static void ShouldBeTrue(this bool item)
        {
            if (item != true) throw new ArgumentException("Value is not true.");
        }

        /// <summary>
        /// Asserts that the boolean evaluates as true.
        /// </summary>
        /// <param name="item">
        /// The boolean value being checked.
        /// </param>
        /// <param name="errorMessage">
        /// The error message associated with failure to validate.
        /// </param>
        public static void ShouldBeTrue(this bool item, string errorMessage)
        {
            if (item != true) throw new ArgumentException(errorMessage);
        }

        /// <summary>
        /// Asserts that the boolean evaluates as false.
        /// </summary>
        /// <param name="item">
        /// The boolean value being checked.
        /// </param>
        public static void ShouldBeFalse(this bool item)
        {
            if (item != false) throw new ArgumentException("Value is not false.");
        }

        /// <summary>
        /// Asserts that the boolean evaluates as false.
        /// </summary>
        /// <param name="item">
        /// The boolean value being checked.
        /// </param>
        /// <param name="errorMessage">
        /// The error message associated with failure to validate.
        /// </param>
        public static void ShouldBeFalse(this bool item, string errorMessage)
        {
            if (item == true) throw new ArgumentException(errorMessage);
        }
    }
}
