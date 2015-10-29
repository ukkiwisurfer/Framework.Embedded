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

using Ignite.Framework.Micro.Common.Data;

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
    /// <para></para>
    /// 
    /// CHUNK ID:   {12 bytes}
    /// CHUNK SIZE: {4 bytes}
    /// CHUNK TYPE: {4 bytes}
    /// CHUNK DATA: {N bytes}
    /// <para></para>
    /// 
    /// HEADER CHUNK
    ///   ID:   OWLDATA
    ///   SIZE: 24
    ///   TYPE: {HEADER}
    ///   DATA:
    ///         PRODUCT: {INTUITION} {20 bytes}
    ///         DEVICE:  {IP} {4 byes}
    /// 
    /// DATA CHUNK
    ///   ID:   SENSOR HEADER
    ///   SIZE: 34
    ///   TYPE: {METADATA}
    ///   DATA:
    ///         ITEM COUNT         {2 bytes}
    ///         METADATA OFFSET    {8 bytes}
    ///         DATA OFFSET        {8 bytes}
    ///         NEXT HEADER OFFSET {8 bytes}
    /// 
    /// PAYLOAD CHUNK
    ///   ID:   OWL SENSOR METADATA
    ///   SIZE: 30 bytes
    ///   TYPE: {SENSOR METADATA}
    ///   DATA:
    ///         SENSORID          {12 bytes}
    ///         RSSI              {6 bytes}
    ///         LQI               {6 bytes}
    ///         BATTERY LEVEL     {2 bytes}
    ///         CHANNEL NUMBER    {2 bytes}
    /// 
    /// PAYLOAD CHUNK
    ///   ID:   OWL SENSOR DATA
    ///   SIZE: 26 bytes
    ///   TYPE: {ELECTRICITY CONSUMPTION}
    ///   DATA:
    ///         SENSORID          {12 bytes}
    ///         TIMESTAMP         {8 bytes}
    ///         UNIT OF MEASURE   {2 bytes}
    ///         CURRENT UNITS     {8 bytes}
    ///         DAY UNITS         {8 bytes}
    /// 
    /// </remarks>
    /// <param name="dataItems">
    /// The collection of log messages to persist.
    /// </param>    /// </remarks>
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
        /// Writes the data to a file.
        /// </summary>
        /// <param name="dataItems">
        /// The collection of items 
        /// </param>
        protected override void WriteData(object[] dataItems)
        {
            foreach (var item in dataItems)
            {
                var dataItem = item as DataItem;
                if (dataItem != null)
                {
                    using (var stream = this.GetFileStream(WorkingPath, TargetPath))
                    {
                        if (stream != null)
                        {
                            var builder = new DataStreamBuilder(stream);

                            builder.SetIPAddress(m_IPAddress);
                            builder.SetTimestamp(dataItem.CaptureTimestamp);
                            builder.SetPayload(dataItem.Payload);

                            builder.Build();
                        }
                    }
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
