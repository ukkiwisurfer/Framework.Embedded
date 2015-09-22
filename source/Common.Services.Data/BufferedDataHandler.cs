
namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.Text;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// An adapter that wraps the <see cref="BufferedDataCaptureService"/>.
    /// </summary>
    public class BufferedDataHandler : IMessageHandler
    {
        private readonly BufferedDataCaptureService m_Service;
        /// <summary>
        /// The service to capture the data messages.
        /// </summary>
        protected BufferedDataCaptureService Service
        {
            get {  return m_Service;}
        }

        /// <summary>
        /// Initialises an instance  of the <see cref="BufferedDataHandler"/> class.
        /// </summary>
        /// <param name="service"></param>
        public BufferedDataHandler(BufferedDataCaptureService service)
        {
            service.ShouldNotBeNull();

            m_Service = service;
        }

        /// <summary>
        /// See <see cref="IMessageHandler.HandleMessage"/> for more detais.
        /// </summary>
        /// <param name="message">
        /// The raw message to be processed.
        /// </param>
        public void HandleMessage(byte[] message)
        {
          ProcessMessage(message);
        }

        /// <summary>
        /// Converts incoming data 
        /// </summary>
        /// <param name="message">
        /// The raw message to be sent.
        /// </param>
        protected virtual void ProcessMessage(byte[] message)
        {
            var dataItem = new DataItem();
            dataItem.CaptureTimestamp = DateTime.UtcNow;
            dataItem.Payload = message;

            Service.AddDataEntry(dataItem);
        }
    }
}
