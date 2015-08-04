
namespace Ignite.Infrastructure.Micro.Common.Assertions
{
    using System;

    public static class GenericAssertions
    {
        /// <summary>
        /// Verifies that an object is not null.
        /// </summary>
        /// <param name="instance"></param>
        public static void ShouldNotBeNull(this object instance)
        {
            if (instance == null) throw new ArgumentNullException();
        }
    }
}
