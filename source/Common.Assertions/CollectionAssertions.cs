//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 

namespace Ignite.Framework.Micro.Common.Assertions
{
    using System;
    using System.Collections;

    using Ignite.Framework.Micro.Common.Core;
    using Ignite.Framework.Micro.Common.Core.Extensions;


    /// <summary>
    /// Extension methods for asserting validity against <see cref="IEnumerable"/> collections.
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
            bool matches = source.All(expected.Contains);            
            if (!matches) throw new ArgumentException("Not all items in the source collection were found in the expected collection.");
        }

       
    }
}
