
namespace Ignite.Infrastructure.Micro.Common.Logging
{
    using System;

    /// <summary>
    /// A log provider that does nothing.
    /// </summary>
    public class NullLogger : ILogProvider
    {
        /// <summary>
        /// See <see cref="ILogProvider.IsDebugEnabled"/> for more details.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// See <see cref="ILogProvider.IsInfoEnabled"/> for more details.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// See <see cref="ILogProvider.Log"/> for more details.
        /// </summary>
        /// <param name="entry">
        /// The log entry to process.
        /// </param>
        public void Log(LogEntry entry)
        {
            
        }
    }
}
