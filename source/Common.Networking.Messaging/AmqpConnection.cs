namespace Ignite.Infrastructure.Micro.Common.IO.Networking.Messaging
{
    using System;

    using Amqp.Framing;
    using Amqp;

    using Ignite.Infrastructure.Micro.Common.Assertions;
    using Ignite.Infrastructure.Micro.Common.Messaging;

    using Trace = Amqp.Trace;

    /// <summary>
    /// Processes incoming messages and dispatches them via an
    /// AMQP endpoint.
    /// </summary>
    public class AmqpConnection : IQueuedConnection
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly string m_ServiceName;
        private Connection m_Connection;
        private Session m_Session;
        private SenderLink m_Sender;

        private bool m_IsOpen;
        /// <summary>
        /// See <see cref="IQueuedConnection.IsOpen"/> for more details.
        /// </summary>
        public bool IsOpen
        {
            get { return m_IsOpen; }
        }

        private string m_ClientId;
        /// <summary>
        /// The identifier for the MQTT client.
        /// </summary>
        public string ClientId
        {
            get { return m_ClientId; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpConnection"/> class.
        /// </summary>
        /// <param name="registration">
        /// Details required to connect to the queued message server.
        /// </param>
        public AmqpConnection(RegistrationData registration)
        {
            registration.ShouldNotBeNull();

            m_ServiceName = registration.ServiceName;
            m_Address = registration.Address;

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
        /// See <see cref="IQueuedConnection.SendMessage"/> for more details.
        /// </summary>
        /// <param name="payload">
        /// The raw payload to be processed.
        /// </param>
        public void SendMessage(byte[] payload)
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
            //Debug.Print(message);
        }
    }
}
