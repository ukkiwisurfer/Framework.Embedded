
namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.IO;
    
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.FileManagement;

    using Json.NETMF;

    /// <summary>
    /// Captures data items and buffers them before writing them to disk.
    /// </summary>
    /// <remarks>
    /// The batch size determines how many entries are buffered before being written
    /// to disk.
    /// </remarks>
    public class BufferedDataCaptureService : BufferedDataService
    {

        /// <summary>
        /// Initialises an instance of the <see cref="BufferedDataCaptureService"/> class. 
        /// </summary>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// Configuration details for buffered data persistence. 
        /// </param>
        public BufferedDataCaptureService(IFileHelper fileHelper, BufferedConfiguration configuration) : base(fileHelper, configuration)
        {
            ServiceName = "BufferedDataCaptureService";
        }

        /// <summary>
        /// Initialises an instance of the <see cref="BufferedDataCaptureService"/> class. 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// Configuration details for buffered data persistence. 
        /// </param>
        public BufferedDataCaptureService(ILogger logger, IFileHelper fileHelper, BufferedConfiguration configuration) : base(logger, fileHelper, configuration)
        {
            ServiceName = "BufferedDataCaptureService";
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
        /// Adds a new data item entry to the proxy service.
        /// </summary>
        /// <remarks>
        /// Will add a new data item in a thread safe manner.
        /// </remarks>
        /// <param name="dataItem">
        /// The data item to add.
        /// </param>
        public void AddDataEntry(DataItem dataItem)
        {
            dataItem.ShouldNotBeNull();

            this.AddDataItem(dataItem);
        }

        /// <summary>
        /// Persists data items to a file.
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
                    var container = new DataItemContainer(dataItems);
                    var converted = SerializeDataContainer(container);

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
        /// Serializes a collection of <see cref="DataItem"/> objects to JSON.
        /// </summary>
        /// <param name="container">
        /// The log entry to seralize.
        /// </param>
        /// <returns>
        /// Json representation of the log entry.
        /// </returns>
        private static string SerializeDataContainer(DataItemContainer container)
        {
            return JsonSerializer.SerializeObject(container);
        }
    }
}
