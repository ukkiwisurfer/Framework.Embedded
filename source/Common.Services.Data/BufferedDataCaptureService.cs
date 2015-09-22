
namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.IO;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.FileManagement;

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
        public BufferedDataCaptureService(IFileHelper fileHelper, BufferedConfiguration configuration)
            : base(fileHelper, configuration)
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
        public BufferedDataCaptureService(ILogger logger, IFileHelper fileHelper, BufferedConfiguration configuration)
            : base(logger, fileHelper, configuration)
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
            using (var stream = this.GetFileStream(WorkingPath, TargetPath))
            {
                using (var writer = new StreamWriter(stream))
                {                             
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<DataItems>");

                    foreach (var item in dataItems)
                    {
                        var dataItem = item as DataItem;
                        if (dataItem != null)
                        {
                            writer.WriteLine("<DataItem>");

                            writer.WriteLine("<CaptureTimeStamp>");
                            writer.WriteLine(dataItem.CaptureTimestamp.ToString("s"));
                            writer.WriteLine("</CaptureTimeStamp>");

                            writer.WriteLine("<Payload>");
                            writer.WriteLine(Convert.ToBase64String(dataItem.Payload));
                            writer.WriteLine("</Payload>");

                            writer.WriteLine("</DataItem>");

                            writer.Flush();
                        }
                    }

                    writer.WriteLine("</DataItems>");
                    writer.Flush();
                }
            }
        }
    }
}
