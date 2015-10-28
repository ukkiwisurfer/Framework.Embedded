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

namespace Ignite.Framework.Micro.Common.Services.Networking
{
    using System;
    using System.IO;
    using System.Threading;

    using Microsoft.SPOT;

    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// A service that publishes details about the status of the device.
    /// </summary>
    /// <remarks>
    /// Publishes a periodic heartbeat message indicating the status of
    /// various capablities of the device. 
    /// <para></para>
    /// The heartbeat message contains: 
    /// 1. Machine name
    /// 2. IP Address
    /// 3. Timestamp
    /// 4. Free memory
    /// </remarks>
    public class StatusService : ThreadedService
    {
        private readonly IMessagePublisher m_Publisher;
        private readonly string m_MachineName;
        private readonly string m_IPAddress;

        /// <summary>
        /// See <see cref="ThreadedService.IsServiceActive"/> for more details.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return true; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="StatusService"/> class.
        /// </summary>
        /// <param name="publisher">
        /// Provides the ability to publish status messages on. 
        /// </param>
        /// <param name="ipAddress">
        /// The IP address of the device.
        /// </param>
        /// <param name="machineName">
        /// The name associated with the device.
        /// </param>
        public StatusService(IMessagePublisher publisher, string ipAddress, string machineName) : base(typeof(StatusService))
        {
            m_Publisher = publisher;
            m_IPAddress = ipAddress;
            m_MachineName = machineName;
        }

        /// <summary>
        /// Performs the actual work of sending a heartbeat message.
        /// </summary>
        protected override void DoWork()
        {
            try
            {
                PublishHeartbeat();
            }
            finally
            {
            }
        }

        /// <summary>
        /// On opening the service publish a startup message.
        /// </summary>
        protected override void OnOpening()
        {
            PublishStartupMessage();
        }

        /// <summary>
        /// On closing down the service publish a shutdown message.
        /// </summary>
        protected override void OnClosing()
        {
            PublishShutdownMessage();
        }

        /// <summary>
        /// When a timeout event occurs, treat it as a oppertunity to perform work.
        /// </summary>
        /// <param name="signalled">
        /// The index of the event that signalled.
        /// </param>
        protected override void DetermineIfWorkDetected(int signalled)
        {
            // If a timeout occurred then treat it as a work detected event.
            if (signalled == WaitHandle.WaitTimeout)
            {
                this.DoWork();
            }
        }

        /// <summary>
        /// Publishes a heartbeat message.
        /// </summary>
        private void PublishHeartbeat()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    AddHeader(writer);

                    writer.WriteLine("<HeartBeat>");

                    AddDeviceMetadata(writer);
                    AddMemoryUsage(writer);

                    writer.WriteLine("</HeartBeat>");
                    writer.Flush();

                    PublishMessage(stream);
                }
            }
        }

        /// <summary>
        /// Publishes a startup message.
        /// </summary>
        private void PublishStartupMessage()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    AddHeader(writer);

                    writer.WriteLine("<Startup>");

                    AddDeviceMetadata(writer);
                    AddMemoryUsage(writer);

                    writer.WriteLine("</Startup>");
                    writer.Flush();

                    PublishMessage(stream);
                }
            }
        }

        /// <summary>
        /// Publishes a shutdown message.
        /// </summary>
        private void PublishShutdownMessage()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    AddHeader(writer);

                    writer.WriteLine("<Shutdown>");

                    AddDeviceMetadata(writer);
                    AddMemoryUsage(writer);

                    writer.WriteLine("</Shutdown>");
                    writer.Flush();

                    PublishMessage(stream);
                }
            }
        }

        /// <summary>
        /// Creates a standard XML header for outgoing control messages.
        /// </summary>
        /// <param name="writer">
        /// The writer used to output to the underlying stream.
        /// </param>
        private void AddHeader(StreamWriter writer)
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }

        /// <summary>
        /// Adds device metadata for outgoing control messages.
        /// </summary>
        /// <param name="writer">
        /// The writer used to output to the underlying stream.
        /// </param>
        private void AddDeviceMetadata(StreamWriter writer)
        {
            writer.WriteLine("<Device>");
            writer.Write("<MachineName>");
            writer.Write(m_MachineName);
            writer.WriteLine("</MachineName>");
            writer.Write("<IPAddress>");
            writer.Write(m_IPAddress);
            writer.WriteLine("</IPAddress>");
            writer.WriteLine("</Device>");

            writer.Write("<Timestamp>");
            writer.Write(DateTime.UtcNow.ToString("u"));
            writer.WriteLine("</Timestamp>");
        }

        /// <summary>
        /// <summary>
        /// Adds metadata on how much free memory is available for outgoing control messages.
        /// </summary>
        /// <param name="writer">
        /// The writer used to output to the underlying stream.
        /// </param>
        private void AddMemoryUsage(StreamWriter writer)
        {
            var freeMemory = Debug.GC(false);

            writer.WriteLine("<Memory>");
            writer.Write("<FreeMemory>");
            writer.Write(freeMemory.ToString());
            writer.WriteLine("</FreeMemory>");
            writer.WriteLine("</Memory>");
        }

        /// <summary>
        /// Publishes a message to a MQ queue.
        /// </summary>
        /// <param name="stream">
        /// The data stream to be sent.
        /// </param>
        private void PublishMessage(MemoryStream stream)
        {
            if (!m_Publisher.IsConnected) m_Publisher.Connect();
            m_Publisher.Publish(stream.ToArray());
        }
      
    }
}
