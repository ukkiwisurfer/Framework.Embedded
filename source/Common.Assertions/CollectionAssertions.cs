namespace Ignite.Framework.Micro.Common.Assertions
{
    using System;
    using System.Collections;
    using NetMf.CommonExtensions;
    using VikingErik.NetMF.MicroLinq;

    /// <summary>
    /// Extension methods for assertions.
    /// </summary>
    public static class CollectionAssertions
    {
        /// <summary>
        /// Asserts that a collection is empty.
        /// </summary>
        /// <typeparam name="T">
        /// The data type of the collection's items.
        /// </typeparam>
        /// <param name="list">
        /// The collection being checked.
        /// </param>
        public static void ShouldBeEmpty(this IEnumerable list)
        {
            if (list == null) throw new ArgumentNullException("Collection is null");
            if (list.Count() != 0) throw new ArgumentException("Collection is not empty");
        }

        /// <summary>
        /// Asserts that two collections are equal.
        /// </summary>
        /// <typeparam name="T">
        /// The data type of the collection's items.
        /// </typeparam>
        /// <param name="source">
        /// The source collection being checked.
        /// </param>
        /// <param name="expected">
        /// The target collection being checked.
        /// </param>
        public static void ShouldBeEqualTo(this IEnumerable source, IEnumerable expected)
        {
            if (source == null) throw new ArgumentNullException("Source collection is null");
            if (expected == null) throw new ArgumentNullException("Expected collection is null");
            
            if (source.Count() != expected.Count()) throw new ArgumentException("Collections do not contain the same number of elements");

            foreach (var sourceItem in source)
            {
                if (!expected.Contains(sourceItem)) throw new ArgumentException(StringUtility.Format("Collections do not contain same items. Failed to find item {0}", sourceItem));
            }
        }

        /// <summary>
        /// Asserts that a collection contains all of the items specified.
        /// </summary>
        /// <typeparam name="T">
        /// The data type of the collection's items. 
        /// </typeparam>
        /// <param name="source">
        /// The source collection being checked.
        /// </param>
        /// <param name="expected">
        /// The target collection being checked.
        /// </param>
        public static void ShouldContainAll(this IEnumerable source, IEnumerable expected)
        {
            bool matches = source.All(x => expected.Contains(x));            
            if (!matches) throw new ArgumentException("Not all items in the source collection were found in the expected collection.");
        }
    }
}
