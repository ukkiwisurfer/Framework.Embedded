namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    using System.Collections;

    public sealed class Filter : IEnumerable
    {
        private IEnumerable e;
        private Predicate p;

        internal Filter(IEnumerable e, Predicate p)
        {
            this.e = e;
            this.p = p;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(e.GetEnumerator(), p);
        }
    }
}
