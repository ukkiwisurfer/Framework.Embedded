
namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    using System.Collections;

    public delegate bool Predicate(object o);

    public sealed class Enumerator : IEnumerator
    {
        private readonly IEnumerator e;
        private readonly Predicate p;

        /// <summary>
        /// Initialises an instance of a enumerator that supports predicate filtering.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="p"></param>
        internal Enumerator(IEnumerator e, Predicate p)
        {
            this.e = e;
            this.p = p;
        }

        /// <summary>
        /// See <see cref="IEnumerator.Current"/> for more details.
        /// </summary>
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
        /// <returns></returns>
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
