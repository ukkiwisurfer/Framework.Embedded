
namespace Ignite.Framework.Micro.Common.Logging
{
    using System;

    public delegate ILogger CreateLogger(Type type);

    /// <summary>
    /// Defines a factory for creating logging providers.
    /// </summary>
    public class LoggingFactory : ILogFactory
    {
        private readonly CreateLogger m_Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingFactory"/> class. 
        /// </summary>
        /// <param name="factory">
        /// Delegate callback to create a logging provider instance from. 
        /// </param>
        public LoggingFactory(CreateLogger factory)
        {
            m_Factory = factory;
        }

        /// <summary>
        /// See <see cref="ILogFactory.GetLogger"/> for more details.
        /// </summary>
        /// <param name="type">
        /// The data type to create the logging provider instance for.
        /// </param>
        /// <returns>
        /// The logging provider instance.
        /// </returns>
        public ILogger GetLogger(Type type)
        {
            return m_Factory(type);
        }
    }
}
