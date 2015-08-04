namespace Ignite.Infrastructure.Micro.Common.IO.Networking.Messaging
{
    using System;

    using Ignite.Infrastructure.Micro.Common.IO.Networking;

    using Microsoft.SPOT;

    using Amqp.Framing;
    using Amqp;

    using Json.NETMF;

    using Trace = Amqp.Trace;

    /// <summary>
    /// Processes incoming messages and dispatches them via an
    /// AMQP endpoint.
    /// </summary>
    public class AmqpMessageHandler : IMessageHandler
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly string m_ServiceName;
        private Connection m_Connection;
        private Session m_Session;
        private SenderLink m_Sender;

        private bool m_IsOpen;

        private string m_ClientId;
        /// <summary>
        /// The identifier for the MQTT client.
        /// </summary>
        public string ClientId
        {
            get { return m_ClientId; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpMessageHandler"/> class.
        /// </summary>
        /// <param name="serviceName">
        /// The name of the service to associated with this message handler.
        /// </param>
        /// <param name="address">
        /// Thw AMQP endpoint connection details.
        /// </param>
        public AmqpMessageHandler(string serviceName, QueueEndpointAddress address)
        {
            m_ServiceName = serviceName;
            m_Address = address;

            Trace.TraceLevel = TraceLevel.Frame;
            Trace.TraceListener = WriteTrace;

            Connection.DisableServerCertValidation = true;
        }

        /// <summary>
        /// Opens a connection to the AMQP server.
        /// </summary>
        public void Open()
        {
            var address = new Address(m_Address.GetUrl());

            m_ClientId = Guid.NewGuid().ToString();
            m_Connection = new Connection(address);
            m_Session = new Session(m_Connection);
            m_Sender = new SenderLink(m_Session, "messages", m_Address.TargetName);

            m_IsOpen = true;
        }

        /// <summary>
        /// Closes the connection to the AMQP server.
        /// </summary>
        public void Close()
        {
            try
            {
                m_Connection.Close();
                m_Session.Close();
                m_Sender.Close();
            }
            finally
            {
                m_Sender = null;
                m_Session = null;
                m_Connection = null;

                m_IsOpen = false;
            }
        }

        /// <summary>
        /// See <see cref="IMessageHandler.HandleMessage"/> for more details.
        /// </summary>
        /// <param name="payload">
        /// The raw payload to be processed.
        /// </param>
        public void HandleMessage(byte[] payload)
        {
            var message = new Message(payload);
            message.Properties = new Properties() { GroupId = m_ServiceName };
            message.ApplicationProperties = new ApplicationProperties();

            m_Sender.Send(message);
        }

        /// <summary>
        /// Amqp tracing to console method.
        /// </summary>
        /// <param name="format">
        /// Formatting string.
        /// </param>
        /// <param name="args">
        /// Arguments to pass to the formatting string.
        /// </param>
        static void WriteTrace(string format, params object[] args)
        {
            string message = args == null ? format : Fx.Format(format, args);
            Debug.Print(message);
        }

        /// <summary>
        /// Serializes an object to JSON.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry to seralize.
        /// </param>
        /// <returns>
        /// Json representation of the log entry.
        /// </returns>
        private static string SerializeLogEntry(object logEntry)
        {
            return JsonSerializer.SerializeObject(logEntry);
        }
    }
}
