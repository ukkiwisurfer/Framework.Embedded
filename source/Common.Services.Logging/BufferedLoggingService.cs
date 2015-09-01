
namespace Ignite.Framework.Micro.Common.Services.Logging
{
    using System;
    using System.IO;
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.FileManagement;
    using Ignite.Framework.Micro.Common.Logging;
    using Ignite.Framework.Micro.Common.Services;
    using Json.NETMF;

    /// <summary>
    /// Captures logging requests and buffers them before writing them to disk.
    /// </summary>
    /// <remarks>
    /// The batch size determines how many entries are buffered before being written
    /// to disk.
    /// </remarks>
    public class BufferedLoggingService : BufferedDataService
    {

        /// <summary>
        /// Initialises an instance of the <see cref="BufferedLoggingService"/> class. 
        /// </summary>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// Configuration details for buffered data persistence. 
        /// </param>
        public BufferedLoggingService(IFileHelper fileHelper, BufferedConfiguration configuration) : base(fileHelper, configuration)
        {
            ServiceName = "BufferedLoggingService";
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details.
        /// </summary>
        /// <param name="isDisposing">
        /// Indicates whether the dispose is deterministic.
        /// </param>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
            }

 	        base.Dispose(isDisposing);
        }

        /// <summary>
        /// Adds a new logging entry to the proxy service.
        /// </summary>
        /// <remarks>
        /// Will add a new log entry in a thread safe manner.
        /// </remarks>
        /// <param name="logEntry">
        /// The log entry to add.
        /// </param>
        public void AddLogEntry(LogEntry logEntry)
        {
            logEntry.ShouldNotBeNull();

            this.AddDataItem(logEntry);
        }

        /// <summary>
        /// Persists log messages to a file.
        /// </summary>
        /// <param name="dataItems">
        /// The collection of log messages to persist.
        /// </param>
        protected override void WriteData(object[] dataItems)
        {
            StreamWriter writer = null;
            try
            {
                writer = this.GetFileStream(WorkingPath, TargetPath);
                if (writer != null)
                {
                    var container = new LogContainer(dataItems);
                    var converted = SerializeLogContainer(container);

                    writer.WriteLine(converted);
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Dispose();
                }
            }
        }

        /// <summary>
        /// Serializes a collection of <see cref="LogEntry"/> objects to JSON.
        /// </summary>
        /// <param name="logContainer">
        /// The log entry to seralize.
        /// </param>
        /// <returns>
        /// Json representation of the log entry.
        /// </returns>
        private static string SerializeLogContainer(LogContainer logContainer)
        {
            return JsonSerializer.SerializeObject(logContainer);
        }
    }
}
