
namespace Ignite.Framework.Micro.Common.Services.Networking
{
    using System;
    using System.IO;
    using System.Threading;

    using Ignite.Framework.Micro.Common.Contract.Messaging;
    
    using Microsoft.SPOT;

    /// <summary>
    /// Service that listens for heartbeat messages from a server.
    /// </summary>
    /// <remarks>
    /// If after a certain period of time no heartbeat message has been
    /// detected, the service attmpts to force a reboot of the device.
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
        /// <param name="publisher"></param>
        /// <param name="ipAddress"></param>
        /// <param name="machineName"></param>
        public StatusService(IMessagePublisher publisher, string ipAddress, string machineName) : base()
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
        /// <param name="signalled"></param>
        /// <returns></returns>
        protected override void DetermineIfWorkDetected(int signalled)
        {
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
            var freeMemory = Debug.GC(false);

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
            var freeMemory = Debug.GC(false);

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
            writer.WriteLine("</MachineName");
            writer.Write("<IPAddress>");
            writer.Write(m_IPAddress);
            writer.WriteLine("</IPAddress");
            writer.WriteLine("</Device>");

            writer.Write("<Timestamp>");
            writer.Write(DateTime.UtcNow.ToString("s"));
            writer.WriteLine("</Timestamp>");
        }

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
