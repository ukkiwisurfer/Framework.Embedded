namespace Ignite.Framework.Micro.Common.Logging
{
    using System;

    /// <summary>
    /// Supports the dynamic creation of loggers for given types.
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// Retrieve an existing logging provider for the given domain type.
        /// </summary>
        /// <param name="type">
        /// The type requiring a logger.
        /// </param>
        /// <returns>
        /// The logging provider.
        /// </returns>
        ILogger GetLogger(Type type);
    }
}
