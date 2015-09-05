
namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    using System;

    using Amqp;
    using Amqp.Framing;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Provides a message publishing capability to a AMQP server.
    /// </summary>
    public class AmqpMessagePublisher : IMessagePublisher, IMessageHandler, IDisposable
    {
        private readonly AmqpConnection m_Connection;
        private readonly SenderLink m_Sender;
        private readonly string m_TopicName;
        private bool m_IsDisposed;

        /// <summary>
        /// INitialises an instance of the publisher.
        /// </summary>
        /// <param name="connection">
        /// The AMQP connection to use.
        /// </param>
        /// <param name="topicName">
        /// The topic name to publish to.
        /// </param>
        public AmqpMessagePublisher(AmqpConnection connection, string topicName)
        {
            connection.ShouldNotBeNull();

            m_Connection = connection;
            m_TopicName = topicName;
            m_Sender = new SenderLink(m_Connection.Session, "messages", topicName);

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
        /// Publishes a message to a topic.
        /// </summary>
        /// <param name="payload"></param>
        public virtual void Publish(byte[] payload)
        {
            var message = new Message(payload);
            message.Properties = new Properties() { GroupId = m_TopicName };
            message.ApplicationProperties = new ApplicationProperties();

            m_Sender.Send(message);
        }

        /// <summary>
        /// Returns the unique identifier of the client.
        /// </summary>
        public string ClientId
        {
            get { return m_Connection.ClientId; }
        }

        /// <summary>
        /// Indicates whether the connection to the AMQP server is established.
        /// </summary>
        public bool IsConnected
        {
            get { return m_Connection.IsConnected; }
        }

        /// <summary>
        /// Attempts to connect to a AMQP server.
        /// </summary>
        public void Connect()
        {
            m_Connection.Connect();
        }
        
        /// <summary>
        /// Disconnects from a AMQP server.
        /// </summary>
        public void Disconnect()
        {
            m_Sender.Close();
            m_Connection.Disconnect();
        }

        /// <summary>
        /// See <see cref="IMessageHandler.HandleMessage"/> for more details.
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(byte[] message)
        {
            Publish(message);
        }
    }
}
