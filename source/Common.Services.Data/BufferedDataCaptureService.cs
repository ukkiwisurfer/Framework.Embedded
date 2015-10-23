//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 



namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.IO;
    using System.Text;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.Contract.FileManagement;

    /// <summary>
    /// Captures data items and buffers them before writing them to disk.
    /// </summary>
    /// <remarks>
    /// The batch size determines how many entries are buffered before being written
    /// to disk.
    /// </remarks>
    public class BufferedDataCaptureService : BufferedDataService
    {
        private readonly string m_IPAddress;

        /// <summary>
        /// Initialises an instance of the <see cref="BufferedDataCaptureService"/> class. 
        /// </summary>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// Configuration details for buffered data persistence. 
        /// </param>
        /// <param name="ipAddress">
        /// The IP address where this service is hosted.
        /// </param>
        public BufferedDataCaptureService(IFileHelper fileHelper, BufferedConfiguration configuration, string ipAddress)  : base(fileHelper, configuration)
        {
            ipAddress.ShouldNotBeEmpty();

            ServiceName = "BufferedDataCaptureService";
            m_IPAddress = ipAddress;
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
        /// <param name="ipAddress">
        /// The IP address where this service is hosted.
        /// </param>
        public BufferedDataCaptureService(ILogger logger, IFileHelper fileHelper, BufferedConfiguration configuration, string ipAddress) : base(logger, fileHelper, configuration)
        {
            ipAddress.ShouldNotBeEmpty();

            ServiceName = "BufferedDataCaptureService";
            m_IPAddress = ipAddress;
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
                    writer.WriteLine("<DataCapture>");

                    AddDeviceMetadata(writer);

                    writer.WriteLine("<DataItems>");
                    foreach (var item in dataItems)
                    {
                        var dataItem = item as DataItem;
                        if (dataItem != null)
                        {
                            writer.WriteLine("<DataItem>");

                            writer.Write("<CaptureTimeStamp>");
                            writer.Write(dataItem.CaptureTimestamp.ToString("u"));
                            writer.WriteLine("</CaptureTimeStamp>");

                            writer.WriteLine("<Payload>");
                            writer.WriteLine(Encoding.UTF8.GetChars(dataItem.Payload));
                            writer.WriteLine("</Payload>");

                            writer.WriteLine("</DataItem>");

                            writer.Flush();
                        }
                    }
                    writer.WriteLine("</DataItems>");

                    writer.WriteLine("</DataCapture>");
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Adds device metadata for outgoing control messages.
        /// </summary>
        /// <param name="writer">
        /// The writer used to output to the underlying stream.
        /// </param>
        private void AddDeviceMetadata(StreamWriter writer)
        {
            writer.WriteLine("<CaptureDevice>");
            writer.Write("<IPAddress>");
            writer.Write(m_IPAddress);
            writer.WriteLine("</IPAddress>");
            writer.WriteLine("</CaptureDevice>");
        }
    }
}
