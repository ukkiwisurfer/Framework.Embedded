

namespace Ignite.Framework.Micro.Common.Services.Logging.Providers
{
    using System;
    using Amqp;
    using Amqp.Framing;

    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Messaging.MessageBus;
    using Ignite.Framework.Micro.Common.Networking;
    
    using Json.NETMF;
    using Microsoft.SPOT;
    
    using Trace = Amqp.Trace;

    /// <summary>
    /// AMQP client.
    /// </summary>
    public class AmqpProvider : IMessageBrokerClient
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly string m_ServiceName;
        private Connection m_Connection;
        private Session m_Session;
        private SenderLink m_Sender;

        private bool m_IsOpen;
        /// <summary>
        /// Indicates whether the connection is open.
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
        /// Initialises an instance of the <see cref="AmqpProvider"/> class.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="address"></param>
        public AmqpProvider(string serviceName, QueueEndpointAddress address)
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
            try
            {
                m_Connection = new Connection(address);
                m_Session = new Session(m_Connection);
                m_Sender = new SenderLink(m_Session, "logging", m_Address.TargetName);

                m_IsOpen = true;
            }
            catch (Exception)
            {
                m_IsOpen = false;
            }

        }

        /// <summary>
        /// Closes the connection to the AMQP server.
        /// </summary>
        public void Close()
        {
            try
            {
                if (m_Connection != null) m_Connection.Close();
                if (m_Session != null) m_Session.Close();
                if (m_Sender != null) m_Sender.Close();
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
        /// Sends <see cref="LogEntry"/> log messages to a AMQP message broker.
        /// </summary>
        /// <param name="logMessages"> 
        /// A collection of log entry messages to send to a AMQP message broker.
        /// </param>
        public void SendMessages(object[] logMessages)
        {
            foreach (var logMessage in logMessages)
            {
                var castedMessage = (LogEntry)logMessage;

                var payload = SerializeLogEntry(castedMessage);

                var message = new Message(payload);
                message.Properties = new Properties() { GroupId = m_ServiceName };
                message.ApplicationProperties = new ApplicationProperties();
                message.ApplicationProperties["logMessageId"] = castedMessage.LogEntryId;

                m_Sender.Send(message);
            }
        }

        static void WriteTrace(string format, params object[] args)
        {
            string message = args == null ? format : Fx.Format(format, args);
            Debug.Print(message);
        }

        /// <summary>
        /// Serializes a <see cref="LogEntry"/> object to JSON.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry to seralize.
        /// </param>
        /// <returns>
        /// Json representation of the log entry.
        /// </returns>
        private static string SerializeLogEntry(LogEntry logEntry)
        {
            return JsonSerializer.SerializeObject(logEntry);
        }
    }
}
