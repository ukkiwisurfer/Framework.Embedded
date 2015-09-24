namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    using System.Collections;

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
