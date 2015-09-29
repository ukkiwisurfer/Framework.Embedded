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
    /// Provides support to filter items in an <see cref="IEnumerable"/> collection using a <see cref="Predicate"/>.
    /// </summary>
    public sealed class Filter : IEnumerable
    {
        private IEnumerable e;
        private Predicate p;

        /// <summary>
        /// Initialises
        /// </summary>
        /// <param name="e">
        /// The <see cref="IEnumerable"/> instance to wrap.
        /// </param>
        /// <param name="p">
        /// The <see cref="Predicate"/> to filter items with.
        /// </param>
        internal Filter(IEnumerable e, Predicate p)
        {
            this.e = e;
            this.p = p;
        }

        /// <summary>
        /// Returns a filtered enumerable collection. 
        /// </summary>
        /// <returns>
        /// The filtered enumerable collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(e.GetEnumerator(), p);
        }
    }
}
