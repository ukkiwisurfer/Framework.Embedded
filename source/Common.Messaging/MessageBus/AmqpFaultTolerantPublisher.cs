using System;

namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Provides a recoverable AMQP publisher
    /// </summary>
    public class AmqpFaultTolerantPublisher : IMessagePublisher, IMessageHandler, IDisposable
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly AmqpBuilder m_Builder;
        private readonly string m_TopicName;
        private readonly string m_LinkName;
        private AmqpMessagePublisher m_Publisher;
        private AmqpConnection m_Connection;

        public string ClientId { get; private set; }
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initialises an instance of a <see cref="AmqpFaultTolerantPublisher"/> class.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="address"></param>
        public AmqpFaultTolerantPublisher(AmqpBuilder builder, QueueEndpointAddress address, string topicName, string linkName)
        {
            m_Builder = builder;
            m_Address = address;
            m_TopicName = topicName;
            m_LinkName = linkName;

            Initialise();
        }

        /// <summary>
        /// Initialises the publisher.
        /// </summary>
        public void Initialise()
        {
            m_Connection = m_Builder.BuildAmqpConnection(m_Address);
            m_Publisher = m_Builder.BuildAmqpPublisher(m_Connection, m_TopicName, m_LinkName);
        }

       
        public void Connect()
        {
            m_Publisher.Connect();
        }

        public void Disconnect()
        {
            m_Publisher.Disconnect();
        }

        public void Publish(byte[] payload)
        {
            m_Publisher.Publish(payload);
        }

        public void HandleMessage(byte[] message)
        {
            m_Publisher.HandleMessage(message);
        }

        public void Dispose()
        {
            m_Publisher.Dispose();
        }
    }
}
