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

namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    using System.Collections;

    /// <summary>
    /// Extension methods for an <see cref="IEnumerable"/> instance.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Helper function 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int Count(this IEnumerable source)
        {
            var n = 0;
            foreach (var o in source)
            {
                n++;
            }
            return n;
        }

        /// <summary>
        /// Returns a wrapper around an IEnumerable to return elements matching the supplied condition.
        /// </summary>
        /// <param name="e">The IEnumerable object from which you want to get items matching a condition.</param>
        /// <param name="p">The predicate which will be applied to each element.</param>
        /// <returns>An IEnumerable which will return only objects which pass the predicate condition.</returns>
        public static IEnumerable Where(this IEnumerable e, Predicate p)
        {
            return new Filter(e, p);
        }

        /// <summary>
        /// Returns true if all objects in the IEnumerable satisfy the predicate condition.
        /// </summary>
        /// <param name="e">The IEnumerable object to be scanned.</param>
        /// <param name="p">The predicate which will be applied to each element unless one fails.</param>
        /// <returns>True if all objects satisfy the predicate condition, otherwise false.</returns>
        public static bool All(this IEnumerable e, Predicate p)
        {
            foreach (var o in e)
                if (!p(o))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if any object in the IEnumerable satisfies the predicate condition.
        /// </summary>
        /// <param name="e">The IEnumerable object to be scanned.</param>
        /// <param name="p">The predicate which will be applied to each element until one passes.</param>
        /// <returns>True if any object satisfies the predicate condition, otherwise false.</returns>
        public static bool Any(this IEnumerable e, Predicate p)
        {
            foreach (var o in e)
                if (p(o))
                    return true;

            return false;
        }

        /// <summary>
        /// Iterates the IEnumerable to see if it contains an object equal to the object being checked.
        /// </summary>
        /// <param name="e">The IEnumerable object to be scanned.</param>
        /// <param name="ob">The object to compare for equality.</param>
        /// <returns>True if the IEnumerable contains an equivalent object, otherwise false.</returns>
        public static bool Contains(this IEnumerable e, object ob)
        {
            foreach (var o in e)
                if (o.Equals(ob))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns a count of the number of elements in a IEnumerable which satisfy a condition.
        /// </summary>
        /// <param name="e">The IEnumerable object of which you want to know the count.</param>
        /// <param name="p">The predicate which will be applied to each element before it is counted.</param>
        /// <returns>The number of items in the IEnumerable which pass the predicate condition.</returns>
        public static int Count(this IEnumerable e, Predicate p)
        {
            int total = 0;

            foreach (var o in e)
            {
                if (p(o))
                    total++;
            }

            return total;
        }
    }
}
