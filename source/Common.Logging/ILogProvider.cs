namespace Ignite.Framework.Micro.Common.Logging
{
    /// <summary>
    /// Common logging provider.
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Returns the status of whether the debug level of logging is enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="entry">
        /// Definition of the information to be logged.
        /// </param>
        void Log(LogEntry entry);
    }
}
