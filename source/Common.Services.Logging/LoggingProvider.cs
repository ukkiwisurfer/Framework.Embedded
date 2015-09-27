
namespace Ignite.Framework.Micro.Common.Services.Logging
{
    using Ignite.Framework.Micro.Common.Contract.Logging;

    /// <summary>
    /// Provides the support to persist logging messages to a queued logging provider.
    /// </summary>
    public class LoggingProvider : ILogProvider
    {
        private readonly BufferedLoggingService m_LoggingService;

        /// <summary>
        /// Initialises ans instance of the <see cref="LoggingProvider"/> class.
        /// </summary>
        /// <param name="loggingService">
        /// Service that supports the queuing and persisting of log messages.
        /// </param>
        public LoggingProvider(BufferedLoggingService loggingService)
        {
            this.m_LoggingService = loggingService;
        }
    
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
        /// The logging entry to add.
        /// </param>
        public void Log(LogEntry entry)
        {
 	        this.m_LoggingService.AddLogEntry(entry);
        }
    }
}
