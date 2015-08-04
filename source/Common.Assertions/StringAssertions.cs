namespace Ignite.Infrastructure.Micro.Common.Assertions
{
    using System;

    /// <summary>
    /// Defines extension methods for String that assert if conditions are not met. 
    /// </summary>
    public static class StringAssertions
    {
        /// <summary>
        /// Asserts if the string is null or empty.
        /// </summary>
        /// <param name="text">
        /// The text to be verified.
        /// </param>
        public static void ShouldNotBeEmpty(this string text)
        {
            if ((text == null) || (string.Empty.Equals(text))) throw new ArgumentNullException();
        }
    }
}
