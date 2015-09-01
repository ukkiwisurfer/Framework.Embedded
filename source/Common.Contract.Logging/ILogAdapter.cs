
namespace Ignite.Framework.Micro.Common.Contract.Logging
{

    /// <summary>
    /// Logging adapter to provide common logging properties to underlying
    /// provider.
    /// </summary>
    public interface ILogAdapter
    {
        /// <summary>
        /// Writes an entry to the logging provider. 
        /// </summary>
        /// <param name="entry">
        /// The logging entry to persist.
        /// </param>
        void Write(LogEntry entry);
    }
}
