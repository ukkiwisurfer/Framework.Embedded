
namespace Ignite.Infrastructure.Micro.Common.Services.Logging.Providers
{
    using System;
    using System.Text;

    using Ignite.Infrastructure.Micro.Common.IO.Networking;
    using Ignite.Infrastructure.Micro.Common.IO.Networking.Messaging;
    using Ignite.Infrastructure.Micro.Common.Logging;
    using Ignite.Infrastructure.Micro.Common.Networking;
    using Json.NETMF;

    using uPLibrary.Networking.M2Mqtt;
    using uPLibrary.Networking.M2Mqtt.Messages;

    /// <summary>
    /// A MQTT client.
    /// </summary>
    public class MqttProvider : IMessageBrokerClient
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly string m_ServiceName;
        private readonly Encoding m_Encoding;
        private MqttClient m_Client;
        
        private bool m_IsOpen;
        /// <summary>
        /// See <see cref="IMessageBrokerClient.IsOpen"/> for more details.
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
        /// <param name="encoding"></param>
        public MqttProvider(string serviceName, QueueEndpointAddress address, Encoding encoding)
        {
            m_ServiceName = serviceName;
            m_Address = address;
            m_Encoding = encoding;
        }

        /// <summary>
        /// Opens a conneciton to a MQTT server.
        /// </summary>
        public void Open()
        {
            m_ClientId = Guid.NewGuid().ToString();

            try
            {
                m_Client = new MqttClient(m_Address.HostName, m_Address.Port, false, null);
                m_Client.Connect(m_ClientId, m_Address.Username, m_Address.Password);

                m_IsOpen = true;
            }
            catch (Exception ex)
            {
                m_IsOpen = false;
            }
        }

        /// <summary>
        /// Closes the connection to the MQTT server.
        /// </summary>
        public void Close()
        {
            try
            {
                m_Client.Disconnect();
            }
            finally
            {
                m_IsOpen = false;
                m_Client = null;
            }
        }

        /// <summary>
        /// Sends a collection of log messages to a MQTT server.
        /// </summary>
        /// <param name="logMessages"></param>
        public void SendMessages(object[] logMessages)
        {
            if (m_IsOpen)
            {
                foreach (var logMessage in logMessages)
                {
                    var castedMessage = (LogEntry)logMessage;
                    var payload = SerializeLogEntry(castedMessage);

                    m_Client.Publish(m_Address.TargetName, payload, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
                }
            }
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
        private byte[] SerializeLogEntry(LogEntry logEntry)
        {
            var payload = JsonSerializer.SerializeObject(logEntry);
            return m_Encoding.GetBytes(payload);
        }
    }
}
