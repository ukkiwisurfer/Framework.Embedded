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

    public delegate bool Predicate(object o);

    /// <summary>
    /// Provides the capability to enumerate over items that meet a given condition defined by a <see cref="Predicate"/>.
    /// </summary>
    public sealed class Enumerator : IEnumerator
    {
        private readonly IEnumerator e;
        private readonly Predicate p;

        /// <summary>
        /// Initialises an instance of a enumerator that supports predicate filtering.
        /// </summary>
        /// <param name="e">
        /// The standard enumerator to wrap.
        /// </param>
        /// <param name="p">
        /// The predicate to use when evaluating whether to include an element as part of filtering. 
        /// </param>
        internal Enumerator(IEnumerator e, Predicate p)
        {
            this.e = e;
            this.p = p;
        }

        /// <summary>
        /// See <see cref="IEnumerator.Current"/> for more details.
        /// </summary>
        /// <returns>
        /// The current item pointed to by the enumerator.
        /// </returns>
        object IEnumerator.Current
        {
            get { return e.Current; }
        }

        /// <summary>
        /// See <see cref="IEnumerator.Reset"/> for more details.
        /// </summary>
        void IEnumerator.Reset()
        {
            e.Reset();
        }

        /// <summary>
        /// Moves to the next item in the collection that meets the predicate condition.
        /// </summary>
        /// <returns>
        /// True if the enumerator could move to the next item.
        /// </returns>
        bool IEnumerator.MoveNext()
        {
            var b = e.MoveNext();
            while (b && !p(e.Current))
            {
                b = e.MoveNext();
            }
            return b;
        }
    }

   
}
