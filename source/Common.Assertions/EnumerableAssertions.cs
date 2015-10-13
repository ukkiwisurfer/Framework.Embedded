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
    using Ignite.Framework.Micro.Common.Core.Extensions;

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
