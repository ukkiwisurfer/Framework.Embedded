namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    using System;
    using Amqp;
    using Amqp.Framing;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Trace = Amqp.Trace;

    /// <summary>
    /// Processes incoming messages and dispatches them via an
    /// AMQP endpoint.
    /// </summary>
    public class AmqpConnection : IQueuedConnection, IDisposable
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly string m_ServiceName;
        private Connection m_Connection;
        private Session m_Session;
        private bool m_IsDisposed;

        internal Session Session
        {
            get {  return m_Session; }
        }

        private bool m_IsConnected;
        /// <summary>
        /// See <see cref="IQueuedConnection.IsConnected"/> for more details.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
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

            //Trace.TraceLevel = TraceLevel.Frame;
            //Trace.TraceListener = WriteTrace;

            //Connection.DisableServerCertValidation = true;
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases any unamanaged resources.
        /// </summary>
        /// <param name="isDisposing">
        /// Indicates whether the disposal is deterministic or not.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!m_IsDisposed)
            {
                if (isDisposing)
                {
                   Disconnect();
                }

                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Opens a connection to the AMQP server.
        /// </summary>
        public void Connect()
        {
            m_ClientId = Guid.NewGuid().ToString();

            var address = new Address(m_Address.GetUrl());
            //var address = new Address(@"amqp://owl:owl@192.168.1.111:5672");

            m_Connection = new Connection(address);
            m_Session = new Session(m_Connection);

            m_IsConnected = true;
        }

        /// <summary>
        /// Closes the connection to the AMQP server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                m_Connection.Close();
                m_Session.Close();
            }
            finally
            {
                m_Session = null;
                m_Connection = null;

                m_IsConnected = false;
            }
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
