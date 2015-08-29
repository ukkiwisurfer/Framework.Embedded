namespace Ignite.Framework.Micro.Common.Assertions
{
    using System;
    using System.Collections;
    using VikingErik.NetMF.MicroLinq;

    /// <summary>
    /// Defines extension methods for IEnumerable that assert if conditions are not met. 
    /// </summary>
    public static class EnumerableAssertions
    {
        /// <summary>
        /// Verifies that a collection is not empty.
        /// </summary>
        /// <typeparam name="T">
        /// The type contained by the collection.
        /// </typeparam>
        /// <param name="collection">
        /// The collection to be verified.
        /// </param>
        public static void ShouldNotBeEmpty(this IEnumerable collection)
        {
            if (collection == null) throw new ArgumentNullException();
            if (collection.Count() == 0) throw new ArgumentException();
        }
    }
}
