
namespace Ignite.Framework.Micro.Common.Services.Logging
{
    using System;
    using System.IO;
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.FileManagement;
    using Ignite.Framework.Micro.Common.Services.Data;

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
            using (var stream = this.GetFileStream(WorkingPath, TargetPath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<LogEntries>");

                    foreach (var item in dataItems)
                    {
                        var dataItem = item as LogEntry;
                        if (dataItem != null)
                        {
                            writer.WriteLine("<LogEntry>");

                            writer.WriteLine("<LogEntryId>");
                            writer.WriteLine(dataItem.LogEntryId);
                            writer.WriteLine("</LogEntryId>");

                            writer.WriteLine("</LogEntry>");

                            writer.Flush();
                        }
                    }

                    writer.WriteLine("</LogEntries>");
                    writer.Flush();
                }
            }
        }
    }
}
