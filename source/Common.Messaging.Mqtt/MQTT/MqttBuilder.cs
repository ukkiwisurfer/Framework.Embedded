

namespace Ignite.Framework.Micro.Common.Messaging.MQTT
{
    using System;

    using Microsoft.SPOT;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Builds AMQP components.
    /// </summary>
    public class MqttBuilder : IMessageBrokerFactory
    {
        private readonly MqttConnection m_Connection;
        private readonly RegistrationData m_EndpointAddress;
        private bool m_IsDisposed;

     
        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class.
        /// </summary>
        /// <param name="endpointAddress"></param>
        public MqttBuilder(RegistrationData endpointAddress)
        {
            endpointAddress.ShouldNotBeNull();

            m_EndpointAddress = endpointAddress;

            //m_Connection = new MqttConnection(endpointAddress);
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
                    //m_Connection.Dispose();
                }

                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Releases a disposable resources.
        /// </summary>
        /// <param name="resource"></param>
        private void DisposeResource(object resource)
        {
            var disposable = resource as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Builds an AMQP publisher.
        /// </summary>
        /// <param name="topicName">
        /// The name of the AMQP topic to publish 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to send messages on.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessagePublisher"/> class.
        /// </returns>
        public IMessagePublisher BuildPublisher(string topicName, string linkName)
        {
            var connection = new MqttConnection(m_EndpointAddress);
            var publisher = new MqttMessagePublisher(connection, topicName, linkName);
            
            return publisher;
        }

        /// <summary>
        /// Builds an AMQP subscriber.
        /// </summary>
        /// <param name="topicName">
        /// The name of the AMQP topic to subscribe to. 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to receive messages on.
        /// </param>
        /// <param name="messageHandler">
        /// Processes any incoming message payloads.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessageSubscriber"/> class.
        /// </returns>
        public IMessageSubscriber BuildSubscriber(string topicName, string linkName, IMessageHandler messageHandler)
        {
            var connection = new MqttConnection(m_EndpointAddress);
            var subscriber = new MqttMessageSubscriber(connection, topicName, linkName, messageHandler);

            return subscriber;
        }

        /// <summary>
        /// Builds an AMQP subscriber.
        /// </summary>
        /// <param name="topicName">
        /// The name of the AMQP topic to subscribe to. 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to receive messages on.
        /// </param>
        /// <param name="messageHandler">
        /// Processes any incoming message payloads.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessageSubscriber"/> class.
        /// </returns>
        public IMessageSubscriber BuildSubscriber(string topicName, string linkName, IMessageHandler messageHandler, int windowSize)
        {
            var connection = new MqttConnection(m_EndpointAddress);
            var subscriber = new MqttMessageSubscriber(connection, topicName, linkName, messageHandler);

            return subscriber;
        }
    }
}
